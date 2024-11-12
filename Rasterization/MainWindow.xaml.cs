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
    Rasterizer _rasterizer;
    Animator _animator;
    
    private const int Size = 450;
    
    
    public MainWindow()
    {
        CompositionTarget.Rendering += (sender, args) =>
        {
            CreateMesh();
        };
        InitializeComponent();
        Width = Height = Size;

        _animator = new Animator(16);
        _rasterizer = new Rasterizer(Size, Size, _vertices, _tris, _animator);

        _animator.RegisterAnimation(Render);
        _animator.Start();
        // Render();
    }
    

    private void CreateMesh()
    {
        // MeshGenerator.AddCube(_vertices, _tris,
        //     new Vector3(1, 0, 0),
        //     new Vector3(0, 1, 0),
        //     new Vector3(0, 0, 1),
        //     new Vector3(0, 0, 0),
        //     new Vector3(1, 1, 0),
        //     new Vector3(1, 0, 1));
        MeshGenerator.AddCube(_vertices, _tris,
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0));
        // MeshGenerator.AddSphere(_vertices,_tris, 3, new Vector3(0, 0, 1));
    }
    

    private void Render()
    {
        Image.Source = _rasterizer.Render();

    }
    
}