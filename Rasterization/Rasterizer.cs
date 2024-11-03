using System.Numerics;
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
    
    private DateTime _lastTime = DateTime.Now;
    
    List<Vertex> _vertices;
    List<(int A, int B, int C)> _tris;

    private Animator _animator;
    private float _rotationDegrees = 0f;
    private float _rotationSpeed = 0.05f;

    public Rasterizer(int sizeX, int sizeY, List<Vertex> vertices, List<(int A, int B, int C)> tris, Animator animator)
    {
        _sizeX = sizeX;
        _sizeY = sizeY;
        _vertices = vertices;
        _tris = tris;
        _animator = animator;
    }

    public List<Polygon> Render()
    {
        var bitmap = new WriteableBitmap(_sizeX, _sizeY, 96, 96, PixelFormats.Rgb24, null);
        byte[] pixels = new byte[_sizeX * _sizeY * 3];
        
        List<Polygon> polygons = new List<Polygon>();
        for (int i = 0; i < _tris.Count; i++)
        {
            Vertex a = _vertices[_tris[i].A];
            Vertex b = _vertices[_tris[i].B];
            Vertex c = _vertices[_tris[i].C];

            var vertices = Transform(a, b, c);

            if (!isBackFacing(vertices.a, vertices.b, vertices.c))
            {
                Polygon polygon = new Polygon();
                PointCollection col =
                [
                    new Point(vertices.a.X, vertices.a.Y),
                    new Point(vertices.b.X, vertices.b.Y), 
                    new Point(vertices.c.X, vertices.c.Y)
                ];
                polygon.Points = col;
                polygon.Stroke = Brushes.Black;
                polygon.Fill = Brushes.Gray;
                polygons.Add(polygon);
            }
        }
        return polygons;
    }

    private (Vector3 a, Vector3 b, Vector3 c) Transform(Vertex a, Vertex b, Vertex c)
    {
        float C = _sizeX / 2; //good default is width/2
        // float deltaTime = _animator.GetDeltaTime();
        var M = Matrix4x4.CreateRotationY(float.DegreesToRadians(_rotationDegrees));
        _rotationDegrees += (_rotationSpeed * _animator.GetDeltaTime()) % 360;
        var V = Matrix4x4.CreateLookAt(new Vector3(0,0,-4), Vector3.Zero, new Vector3(0, -1, 0));

        float near = 0.1f;
        float far = 100f;
        var P = Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(90), (float)_sizeX/_sizeY, near, far);

        var MVP = M * V * P;
        a = VertexShader(a, MVP);
        b = VertexShader(b, MVP);
        c = VertexShader(c, MVP);
            
        a = Project(a);
        b = Project(b);
        c = Project(c);

        Vector3 pointA = ProjectTo2D(a, C);
        Vector3 pointB = ProjectTo2D(b, C);
        Vector3 pointc = ProjectTo2D(c, C);
        
        return (pointA, pointB, pointc);
    }

    private (float u, float v) GetUVCoordinates(Vector4 Q, (Vertex A, Vertex B, Vertex C) triangle)
    {
        Vector4 AB = triangle.B.Position - triangle.A.Position;
        Vector4 AC = triangle.C.Position - triangle.A.Position;
        Vector4 AQ = Q - triangle.A.Position;

        float factor1 = 1f / ((AB.X * AC.Y) - (AC.X * AB.Y));
        Matrix3x2 matrix = new Matrix3x2(AC.Y, -AC.X, -AB.Y, AB.X, 0, 0);

        Matrix3x2 multiplied = matrix * factor1;

        Vector2 uv = Vector2.Transform(new Vector2(Q.X, Q.Y), multiplied);

        return (uv.X, uv.Y) ;
    }

    private Vector3 ProjectTo2D(Vertex vertex, float c)
    {

        float x = vertex.Position.X * c + c;
        float y = vertex.Position.Y * c + _sizeY/2;
        return new Vector3(x,y,0);
    }

    private Vertex Project(Vertex v)
    {
        return (1f / v.Position.W) * v;
    }


    private Vertex VertexShader(Vertex v, Matrix4x4 MVP)
    {
        Vector4 transformedPosition = Vector4.Transform(v.Position, MVP);
        return v with { Position = transformedPosition };
    }

    private bool isBackFacing(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 AB = new Vector3(b.X - a.X, b.Y - a.Y, 0);
        Vector3 AC = new Vector3(c.X - a.X, c.Y - a.Y, 0);

        return Vector3.Cross(AB, AC).Z > 0;
    }
}