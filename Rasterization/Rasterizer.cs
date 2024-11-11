﻿using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Rasterization;

public class Rasterizer
{
    private readonly int _sizeX;
    private readonly int _sizeY;

    List<Vertex> _vertices;
    List<(int A, int B, int C)> _tris;

    private Animator _animator;
    private float _rotationDegrees = 45f;
    private float _rotationSpeed = 0.05f;

    public Rasterizer(int sizeX, int sizeY, List<Vertex> vertices, List<(int A, int B, int C)> tris, Animator animator)
    {
        _sizeX = sizeX;
        _sizeY = sizeY;
        _vertices = vertices;
        _tris = tris;
        _animator = animator;
    }

    public WriteableBitmap Render()
    {
        var bitmap = new WriteableBitmap(_sizeX, _sizeY, 96, 96, PixelFormats.Rgb24, null);
        byte[] pixels = new byte[_sizeX * _sizeY * 3];
        
        var M = Matrix4x4.CreateRotationY(float.DegreesToRadians(_rotationDegrees));
        var V = Matrix4x4.CreateLookAt(new Vector3(0, 0, -4), Vector3.Zero, new Vector3(0, -1, 0));

        float near = 0.1f;
        float far = 100f;
        var P = Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(90), (float)_sizeX / _sizeY, near, far);
        var MVP = M * V * P;
        
        _rotationDegrees = (_rotationDegrees + _rotationSpeed * _animator.GetDeltaTime()) % 360;
        for (int i = 0; i < _tris.Count; i++)
        {
            //Vertices A,B,C
            Vertex a = _vertices[_tris[i].A];
            Vertex b = _vertices[_tris[i].B];
            Vertex c = _vertices[_tris[i].C];

            //Vertex Shader
            a = VertexShader(a, M, MVP);
            b = VertexShader(b, M, MVP);
            c = VertexShader(c, M, MVP);
            
            //Project vertices through Scaling with 1/position.w
            a = Project(a);
            b = Project(b);
            c = Project(c);

            Vector2 posA = ProjectTo2D(a, _sizeX / 2f);
            Vector2 posB = ProjectTo2D(b, _sizeX / 2f);
            Vector2 posC = ProjectTo2D(c, _sizeX / 2f);

            var boundingCoords = GetBoundingCoordinates(posA, posB, posC);

            //Rasterize pixels of triangle
            if (!isBackFacing(posA, posB, posC))
            {
                Parallel.For(boundingCoords.minY, boundingCoords.maxY + 1, y =>
                {
                    for (int x = boundingCoords.minX; x <= boundingCoords.maxX; x++)
                    {
                        Vertex Q = new Vertex(new Vector3(x, y, 0), Vector3.Zero, new Vector2(0, 0), -Vector3.UnitZ);
                        Q = Rasterize(Q, (posA, posB, posC), (a.Color, b.Color, c.Color));
                        if (Q.ST.X >= 0 && Q.ST.Y >= 0 && (Q.ST.X + Q.ST.Y) < 1)
                        {
                            int index = y * (_sizeX * 3) + x * 3;
                            pixels[index] = (byte) Math.Clamp(Q.Color.X * 255, 0, 255);
                            pixels[index + 1] = (byte) Math.Clamp(Q.Color.Y * 255, 0, 255);
                            pixels[index + 2] = (byte) Math.Clamp(Q.Color.Z * 255, 0, 255);
                        }
                    }
                });
            }
        }
        bitmap.WritePixels(new Int32Rect(0, 0, _sizeX, _sizeY), pixels, _sizeX * 3, 0);
        return bitmap;
    }

    private Vertex Rasterize(Vertex Q, (Vector2 a, Vector2 b, Vector2 c) triangle, (Vector3 colorA, Vector3 colorB, Vector3 colorC) colors)
    {
        Vector2 AB = triangle.b - triangle.a;
        Vector2 AC = triangle.c - triangle.a;
        Vector2 AQ = new Vector2(Q.Position.X - triangle.a.X, Q.Position.Y - triangle.a.Y);

        float matrix00 = AC.Y;
        float matrix01 = -AC.X;
        float matrix10 = -AB.Y;
        float matrix11 = AB.X;

        float factor1 = 1f / (AB.X * AC.Y - AB.Y * AC.X);

        float u = factor1 * (matrix00 * AQ.X + matrix01 * AQ.Y);
        float v = factor1 * (matrix10 * AQ.X + matrix11 * AQ.Y);
        
        Vector3 colorQ = u * colors.colorB + v * colors.colorC + (1 - u - v) * colors.colorA;

        Vertex newQ = Q with {ST = new Vector2(u, v), Color = colorQ};
        return newQ;
    }

    // private Vector3 GetColorAtPoint(Vector2 Q, (Vector2 a, Vector2 b, Vector2 c) triangle,
    //     (Vector3 colorA, Vector3 colorB, Vector3 colorC) colors)
    // {
    //     (float u, float v) = Rasterize(Q, triangle);
    //
    //     return colorQ;
    // }

    private Vector2 ProjectTo2D(Vertex vertex, float c)
    {
        float x = vertex.Position.X * c + c;
        float y = vertex.Position.Y * c + _sizeY / 2;
        return new Vector2(x, y);
    }

    private Vertex Project(Vertex v)
    {
        return (1f / v.Position.W) * v;
    }
    
    private Vertex VertexShader(Vertex v, Matrix4x4 M, Matrix4x4 MVP)
    {
        Vector4 transformedPosition = Vector4.Transform(v.Position, MVP);
        Vector3 tranformedWorldCoords = Vector3.Transform(v.WorldCoordinates, M);
        Vector3 transformedNormal = Vector3.TransformNormal(v.Normal, CalculateNormalMatrix(M));
        return v with
        {
            Position = transformedPosition, WorldCoordinates = tranformedWorldCoords, Normal = transformedNormal
        };
    }
    
    public Matrix4x4 CalculateNormalMatrix(Matrix4x4 M)
    {
        float detM = M.GetDeterminant();
        if (!Matrix4x4.Invert(M, out Matrix4x4 inverse))
        {
            throw new InvalidOperationException("Matrix is not invertible, cannot compute normal matrix.");
        }
        Matrix4x4 inverseTransposed = Matrix4x4.Transpose(inverse);
        Matrix4x4 normal = inverseTransposed * detM;

        return normal;
    }

    private bool isBackFacing(Vector2 a, Vector2 b, Vector2 c)
    {
        Vector3 AB = new Vector3(b.X - a.X, b.Y - a.Y, 0);
        Vector3 AC = new Vector3(c.X - a.X, c.Y - a.Y, 0);

        return Vector3.Cross(AB, AC).Z > 0;
    }

    private (int minX, int maxX, int minY, int maxY) GetBoundingCoordinates(Vector2 a, Vector2 b, Vector2 c)
    {
        int minX = (int) MathF.Min(a.X, MathF.Min(b.X, c.X));
        int minY = (int) MathF.Min(a.Y, MathF.Min(b.Y, c.Y));
        int maxX = (int) MathF.Max(a.X, MathF.Max(b.X, c.X));
        int maxY = (int) MathF.Max(a.Y, MathF.Max(b.Y, c.Y));

        return (minX, maxX, minY, maxY);

    }
}