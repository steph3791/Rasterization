using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
// ReSharper disable All

namespace Rasterization;

public class Rasterizer
{
    private readonly int _sizeX;
    private readonly int _sizeY;
    
    private readonly Object _parent;
    private readonly List<Light> _lightSources;

    private readonly Animator _animator;
    private readonly float _rotationDegrees = 0f;
    private readonly Vector3 _camera;
    private readonly float[][] _zBuffer;

    private byte[] _pixels;
    private float _deltaTime = 0;
    
    public Rasterizer(int sizeX, int sizeY, Object parent, Animator animator)
    {
        _sizeX = sizeX;
        _sizeY = sizeY;
        _animator = animator;
        _camera = new Vector3(0, 0, -6);
        _zBuffer = new float[_sizeX][];
        _pixels = new byte[_sizeX * _sizeY * 3];

        for (int i = 0; i < _zBuffer.Length; i++)
        {
            _zBuffer[i] = new float[_sizeY];
            _zBuffer[i] = Enumerable.Repeat(float.MaxValue, _sizeY).ToArray();
        }

        _lightSources = [];
        CreateLightSources();
        _parent = parent;
    }

    private void CreateLightSources()
    {
        _lightSources.Add(new Light(Config.DefaultLightSource, new Vector3(1, 1, 1)));
    }

    public WriteableBitmap Render()
    {
        var bitmap = new WriteableBitmap(_sizeX, _sizeY, 96, 96, PixelFormats.Rgb24, null);
        _pixels = new byte[_sizeX * _sizeY * 3];
        for (int i = 0; i < _zBuffer.Length; i++)
        {
            _zBuffer[i] = new float[_sizeY];
            _zBuffer[i] = Enumerable.Repeat(float.MaxValue, _sizeY).ToArray();
        }
        
        float near = 0.1f;
        float far = 100f;
        var M = Matrix4x4.CreateRotationY(float.DegreesToRadians(_rotationDegrees));
        var V = Matrix4x4.CreateLookAt(_camera, Vector3.Zero, new Vector3(0, -1, 0));
        var P = Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(90), (float)_sizeX / _sizeY, near, far);
        var VP = V * P;

        if (Config.Animate)
        {
            _deltaTime = _animator.GetDeltaTime();
        }
        RenderSceneGraph(_parent, M, VP, near, far);
        
        bitmap.WritePixels(new Int32Rect(0, 0, _sizeX, _sizeY), _pixels, _sizeX * 3, 0);
        return bitmap;
    }

    private void RenderSceneGraph(Object obj, Matrix4x4 M, Matrix4x4 VP, float near, float far)
    {
        Render(obj, M, M*VP, near, far);
        foreach (var graph in obj.Node.Children)
        {
            var m = graph.transformations.GetTransformation(_deltaTime) * M;
            RenderSceneGraph(graph.child, m, VP, near, far);
        }
    }
    
    private void Render(Object obj, Matrix4x4 M, Matrix4x4 MVP,
        float near, float far)
    {
        List<Vertex> vertices = obj.Node.Vertices;
        List<(int A, int B, int C)> tris = obj.Node.Tris;
        for (int i = 0; i < tris.Count; i++)
        {
            //Vertices A,B,C
            Vertex a = vertices[tris[i].A];
            Vertex b = vertices[tris[i].B];
            Vertex c = vertices[tris[i].C];

            //Vertex Shader
            a = VertexShader(a, M, MVP);
            b = VertexShader(b, M, MVP);
            c = VertexShader(c, M, MVP);

            //Project vertices through Scaling with 1/position.w
            a = Project(a);
            b = Project(b);
            c = Project(c);

            Vertex a2D = ProjectTo2D(a, _sizeX / 2f);
            Vertex b2D = ProjectTo2D(b, _sizeX / 2f);
            Vertex c2D = ProjectTo2D(c, _sizeX / 2f);

            if (!isBackFacing(a2D.Position, b2D.Position, c2D.Position))
            {
                Vertex ab = b2D - a2D;
                Vertex ac = c2D - a2D;
                var boundingCoords = GetBoundingCoordinates(a2D.Position, b2D.Position, c2D.Position);
                Parallel.For(boundingCoords.minY, boundingCoords.maxY + 1, y =>
                {
                    for (int x = boundingCoords.minX; x <= boundingCoords.maxX; x++)
                    {
                        Vector2 AQ = new Vector2(x - a2D.Position.X, y - a2D.Position.Y);
                        var (u, v) = GetBarycentricCoordinates(AQ, ab, ac);
                        if (u >= 0 && v >= 0 && (u + v) < 1)
                        {
                            Vertex Q = a + u * ab + v * ac;
                            Q = TransformQToCameraSpace(Q, near, far, M, MVP);
                            if (x < _zBuffer.Length && x>=0 && y < _zBuffer[x].Length && y>=0 && _zBuffer[x][y] > Q.Position.Z)
                            {
                                _zBuffer[x][y] = Q.Position.Z;
                                Vector3 color = FragmentShader(Q, obj);
                                int index = y * (_sizeX * 3) + x * 3;
                                _pixels[index] = (byte)Math.Clamp(color.X * 255, 0, 255);
                                _pixels[index + 1] = (byte)Math.Clamp(color.Y * 255, 0, 255);
                                _pixels[index + 2] = (byte)Math.Clamp(color.Z * 255, 0, 255);
                            }
                        }
                    }
                });
            }
        }
    }

    private (float u, float v) GetBarycentricCoordinates(Vector2 AQ, Vertex AB, Vertex AC)
    {
        float matrix00 = AC.Position.Y;
        float matrix01 = -AC.Position.X;
        float matrix10 = -AB.Position.Y;
        float matrix11 = AB.Position.X;

        float factor1 = 1f / (AB.Position.X * AC.Position.Y - AB.Position.Y * AC.Position.X);

        float u = factor1 * (matrix00 * AQ.X + matrix01 * AQ.Y);
        float v = factor1 * (matrix10 * AQ.X + matrix11 * AQ.Y);
        return (u, v);
    }

    private Vector3 FragmentShader(Vertex Q, Object obj)
    {
        Vector3 ambientLightColor = new Vector3(0.1f, 0.1f, 0.1f); // Adjust as needed
        Vector3 diffuseColor = obj.GetDiffuseRenderColor(Q);
        Vector3 ambient = ambientLightColor * diffuseColor;

        Vector3 color = Vector3.Zero;
        Vector3 n = Vector3.Normalize(Q.Normal);
        Vector3 viewDir = Vector3.Normalize(_camera - Q.WorldCoordinates);

        for (int i = 0; i < _lightSources.Count; i++)
        {
            Light light = _lightSources[i];
            Vector3 lightPos = light.position;
            Vector3 lightDir = Vector3.Normalize(lightPos - Q.WorldCoordinates);

            // Diffuse component
            float cos = Vector3.Dot(n, lightDir);

            Vector3 diffuseIllumination = Vector3.Zero;
            if (cos > 0)
            {
                diffuseIllumination = light.color * diffuseColor * cos;
            }

            // Specular component
            Vector3 reflectDir = Vector3.Reflect(-lightDir, n);
            float cosReflected = Vector3.Dot(reflectDir, viewDir);
            Vector3 specularIllumination = Vector3.Zero;
            if (cos > 0 && cosReflected > 0)
            {
                float specularStrength = MathF.Pow(cosReflected,25);
                specularIllumination = light.color * specularStrength;
            }

            color += diffuseIllumination + specularIllumination;
        }

        color += ambient;
        return color;
    }

    private Vertex TransformQToCameraSpace(Vertex Q, float near, float far, Matrix4x4 M, Matrix4x4 MVP)
    {
        float z = far * near / (far + (near - far) * Q.Position.Z);
        return z * Q;
    }

    private Vertex ProjectTo2D(Vertex vertex, float c)
    {
        float x = vertex.Position.X * c + c;
        float y = vertex.Position.Y * c + _sizeY / 2;
        return vertex with { Position = vertex.Position with { X = x, Y = y } };
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
            Position = transformedPosition, WorldCoordinates = tranformedWorldCoords,
            Normal = Vector3.Normalize(transformedNormal)
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

    private bool isBackFacing(Vector4 a, Vector4 b, Vector4 c)
    {
        Vector3 AB = new Vector3(b.X - a.X, b.Y - a.Y, 0);
        Vector3 AC = new Vector3(c.X - a.X, c.Y - a.Y, 0);

        return Vector3.Cross(AB, AC).Z > 0;
    }

    private (int minX, int maxX, int minY, int maxY) GetBoundingCoordinates(Vector4 a, Vector4 b, Vector4 c)
    {
        int minX = (int)MathF.Min(a.X, MathF.Min(b.X, c.X));
        int minY = (int)MathF.Min(a.Y, MathF.Min(b.Y, c.Y));
        int maxX = (int)MathF.Max(a.X, MathF.Max(b.X, c.X));
        int maxY = (int)MathF.Max(a.Y, MathF.Max(b.Y, c.Y));

        return (minX, maxX, minY, maxY);
    }
}