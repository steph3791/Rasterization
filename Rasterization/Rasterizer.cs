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
    
    List<Vertex> _vertices;
    List<(int A, int B, int C)> _tris;

    public Rasterizer(int sizeX, int sizeY, List<Vertex> vertices, List<(int A, int B, int C)> tris)
    {
        _sizeX = sizeX;
        _sizeY = sizeY;
        _vertices = vertices;
        _tris = tris;
    }

    public List<Polygon> Render()
    {
        var bitmap = new WriteableBitmap(_sizeX, _sizeY, 96, 96, PixelFormats.Rgb24, null);
        byte[] pixels = new byte[_sizeX * _sizeY * 3];
        
        List<Polygon> polygons = new List<Polygon>();

        double C = _sizeX / 2 - 100; //good default is width/2
        Point a;
        Point b;
        Point c;
        for (int i = 0; i < _tris.Count; i++)
        {
            Vertex a_vertex = _vertices[_tris[i].A];
            Vertex b_vertex = _vertices[_tris[i].B];
            Vertex c_vertex = _vertices[_tris[i].C];

            a_vertex = VertexShader(a_vertex);
            b_vertex = VertexShader(b_vertex);
            c_vertex = VertexShader(c_vertex);
            
            a_vertex = Project(a_vertex);
            b_vertex = Project(b_vertex);
            c_vertex = Project(c_vertex);

            a = ProjectTo2D(a_vertex, C);
            b = ProjectTo2D(b_vertex, C);
            c = ProjectTo2D(c_vertex, C);

            // for (int y = 0; y < _sizeY; y++)
            // {
            //     for (int x = 0; x < _sizeX; x++)
            //     {
            //         (float u, float v) = GetUVCoordinates(new Vector4(x, y, 0,1), (a_vertex, b_vertex, c_vertex));
            //         if (u >= 0 && v >= 0 && (u + v) < 1)
            //         {
            //             int index = y * (_sizeX * 3) + x * 3; //FIXME sizeX or sizeY ???
            //             pixels[index] = 0;
            //             pixels[index + 1] = 0;
            //             pixels[index + 2] = 0;
            //         }
            //     }
            // }
            
            Polygon polygon = new Polygon();
            PointCollection col =
            [
                a, b, c
            ];
            polygon.Points = col;
            polygon.Stroke = Brushes.Black;
            polygons.Add(polygon);
        }
        return polygons;
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

    private Point ProjectTo2D(Vertex vertex, double c)
    {

        double x = vertex.Position.X * c + c;
        double y = vertex.Position.Y * c + _sizeY/2;
        return new Point(x, y);
    }

    private Vertex Project(Vertex v)
    {
        return (1 / v.Position.W) * v;
    }


    private Vertex VertexShader(Vertex v)
    {
        Vector4 pos = v.Position with {Z = 0, W = v.Position.Z + 4};
        Vertex copy = v with {Position = pos};
        return copy;
    }
}