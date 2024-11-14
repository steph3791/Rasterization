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

    private Object _parent;
    // SceneGraphNode _sceneGraphNode;
    
    private const int Size = 450;
    
    
    public MainWindow()
    {
        InitializeComponent();
        Width = Height = Size;

        _animator = new Animator(16);
        
        // RenderObject();
        RenderSceneGraph();
        
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
        _parent = new Object(new SceneGraphNode(), null, new Object.AnimatedProperty<object>(45f, (f, deltaTime) =>
        {
            f = ((float)f + 0.05f * deltaTime) % 360;
            return (f, Matrix4x4.CreateRotationY(float.DegreesToRadians((float) f)));
        }));
        CreateSceneGraph();
        _rasterizer = new Rasterizer(Size, Size, _parent, _animator);
        _animator.RegisterAnimation(Render);
        _animator.Start();
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
        WriteableBitmap fireData = TextureUtil.LoadTexture("tile1.png");
        byte[] pixels = TextureUtil.PreloadTextureData(fireData);
        TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);
        
        SceneGraphNode cube = new SceneGraphNode();
        Object.AnimatedProperty<object> p1 = new Object.AnimatedProperty<object>(45f, (f, deltaTime) =>
        {
            f = ((float)f + 0.05f * deltaTime) % 360;
            return (f, Matrix4x4.CreateRotationX(float.DegreesToRadians((float) f)));
        });
        Object cubeObject = new Object(cube, fireTexture, p1);
        _parent.Node.Children.Add((cubeObject, CreateTransformation(0, new Vector3(1,0,0))));
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0,1,0));

        SceneGraphNode sphere = new SceneGraphNode();
        Object sphereObject = new Object(sphere, null);
        _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(0,1,1))));
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 4, new Vector3(1,0,0));
        
        
        // SceneGraphNode sphere1 = new SceneGraphNode();
        // _sceneGraphNode.Children.Add((sphere1, CreateTransformation(1f, new Vector3(-2f,0.5f,0))));
        // MeshGenerator.AddSphere(sphere1.Vertices, sphere1.Tris, 4, new Vector3(1,0,0));
    }

    private Matrix4x4 CreateTransformation(float rotation, Vector3 translation)
    {
        var M = Matrix4x4.CreateRotationX(float.DegreesToRadians(rotation));
        var T = Matrix4x4.CreateTranslation(translation);
        return M*T ;

    }
    

    private void Render()
    {
        Image.Source = _rasterizer.Render();

    }
    
}