using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rasterization;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    List<Vertex> _vertices = new List<Vertex>();
    List<(int A, int B, int C)> _tris = new List<(int A, int B, int C)>();

    public MainWindow()
    {
        CompositionTarget.Rendering += (sender, args) => { RenderMesh(); };

        InitializeComponent();
    }

    private void RenderMesh()
    {
        MeshGenerator.AddCube(_vertices, _tris,
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 1));

        double C = Width/2 - 100; //good default is width/2
        Point a;
        Point b;
        Point c;
        for (int i = 0; i < _tris.Count; i++)
        {
            Vertex a_vertex = _vertices[_tris[i].A];
            Vertex b_vertex = _vertices[_tris[i].B];
            Vertex c_vertex = _vertices[_tris[i].C];

            // a_vertex = VertexShader(a_vertex);
            // b_vertex = VertexShader(a_vertex);
            // c_vertex = VertexShader(a_vertex);
            //
            // a_vertex = Project(a_vertex);
            // b_vertex = Project(a_vertex);
            // c_vertex = Project(a_vertex);
            
            a = ProjectTo2D(a_vertex, C);
            b = ProjectTo2D(b_vertex, C);
            c = ProjectTo2D(c_vertex, C);
            Console.WriteLine($"A: {a}, B: {b}, C: {c}");
            Polygon polygon = new Polygon();
            PointCollection col =
            [
                a, b, c
            ];
            polygon.Points = col;
            polygon.Stroke = Brushes.Black;
            Canvas.Children.Add(
                polygon);
        }
    }

    private Point ProjectTo2D(Vertex vertex, double c)
    {
        double x = c * vertex.Position.X / vertex.Position.Z + Width / 2;
        double y = c * vertex.Position.Y / vertex.Position.Z + Height / 2;
        return new Point(x, y);
    }

    private Vertex Project(Vertex v)
    {
        return 1 / v.Position.W * v;
    }
    

    private Vertex VertexShader(Vertex v)
    {
        Vector4 pos = new Vector4(v.Position.X, v.Position.Y, 0, v.Position.Z + 4);
        Vertex copy = new Vertex(pos, v.WorldCoordinates, v.Color, v.ST, v.Normal);
        return copy;
    }
}