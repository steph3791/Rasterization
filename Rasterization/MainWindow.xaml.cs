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
    
    SceneGraphNode _sceneGraphNode;
    
    private const int Size = 450;
    
    
    public MainWindow()
    {
        InitializeComponent();
        Width = Height = Size;

        _animator = new Animator(16);
        
        RenderObject();
        // RenderSceneGraph();
        
    }

    void RenderObject()
    {
        CreateMesh();
        _rasterizer = new Rasterizer(Size, Size, _vertices, _tris, _animator);
        _animator.RegisterAnimation(Render);
        _animator.Start();
    }

    void RenderSceneGraph()
    {
        _sceneGraphNode = new SceneGraphNode();
        CreateSceneGraph();
        _rasterizer = new Rasterizer(Size, Size, _sceneGraphNode, _animator);
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

    private void CreateSceneGraph()
    {
        SceneGraphNode cube=new SceneGraphNode();
        _sceneGraphNode.Children.Add((cube, CreateTransformation(3f, new Vector3(0,0,0))));
        MeshGenerator.AddSingleColorCube(cube.Vertices, cube.Tris, new Vector3(0,1,0));

        SceneGraphNode sphere = new SceneGraphNode();
        _sceneGraphNode.Children.Add((sphere, CreateTransformation(3f, new Vector3(2,0,0))));
        MeshGenerator.AddSphere(sphere.Vertices, sphere.Tris, 4, new Vector3(1,0,0));
    }

    private Matrix4x4 CreateTransformation(float rotation, Vector3 translation)
    {
        var M = Matrix4x4.CreateRotationY(float.DegreesToRadians(rotation));
        var T = Matrix4x4.CreateTranslation(translation);
        return M * T;

    }
    

    private void Render()
    {
        Image.Source = _rasterizer.Render();

    }
    
}