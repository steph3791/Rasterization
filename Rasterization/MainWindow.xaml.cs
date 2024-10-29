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

    private const int Size = 450;
    public MainWindow()
    {
        CompositionTarget.Rendering += (sender, args) => { RenderMesh(); };
        InitializeComponent();
        Width = Height = Size;
        RenderMesh();
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
        
        Rasterizer rasterizer = new Rasterizer(Size, Size, _vertices, _tris);
        Image.Source = rasterizer.Render();
    }
    
}