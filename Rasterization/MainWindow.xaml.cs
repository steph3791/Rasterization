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
        _parent = new Object(new SceneGraphNode(), null);
        CreateSceneGraph();
        _rasterizer = new Rasterizer(Size, Size, _parent, _animator);
        _animator.RegisterAnimation(Render);
        _animator.Start();
    }


    private void CreateMesh()
    {
        MeshGenerator.AddCube(_vertices, _tris,
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0));
    }

    private void CreateSceneGraph()
    {
        // CreateSingleCube();
        // CreateMultiColorCube();
        // CreateSingleSphere();
        // CreateTexturedCube();
        // CreateTexturedSphere();
        // CreateGroupedScene();
        // CreateScene1();
        CreateMultipleRotation();
    }

    // private void CreateSingleCube()
    // {
    //     SceneGraphNode cube = new SceneGraphNode();
    //     Object cubeObject = new Object(cube, null);
    //     _parent.Node.Children.Add((cubeObject, CreateTransformation(0, new Vector3(0,0,0))));
    //     MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0,1,0));
    // }
    //
    // private void CreateMultiColorCube()
    // {
    //     SceneGraphNode cube = new SceneGraphNode();
    //     Object cubeObject = new Object(cube, null);
    //     _parent.Node.Children.Add((cubeObject, CreateTransformation(0, new Vector3(0,0,0))));
    //     MeshGenerator.AddCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, 
    //         new Vector3(0,1,0),
    //         new Vector3(1,0,0),
    //         new Vector3(0,0,1),
    //         new Vector3(1,1,0),
    //         new Vector3(0,1,1),
    //         new Vector3(1,0,1)
    //         );
    // }
    //
    // private void CreateSingleSphere()
    // {
    //     SceneGraphNode sphere = new SceneGraphNode();
    //     Object sphereObject = new Object(sphere, null);
    //     _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(0,0,0))));
    //     MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris,5, new Vector3(0,1,0));
    // }
    //
    // private void CreateTexturedCube()
    // {
    //     WriteableBitmap fireData = TextureUtil.LoadTexture("tile1.png");
    //     byte[] pixels = TextureUtil.PreloadTextureData(fireData);
    //     TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);
    //     
    //     SceneGraphNode cube = new SceneGraphNode();
    //     Object cubeObject = new Object(cube, fireTexture);
    //     _parent.Node.Children.Add((cubeObject, CreateTransformation(0, new Vector3(0,0,0))));
    //     MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0,1,0));
    // }
    //
    // private void CreateTexturedSphere()
    // {
    //     WriteableBitmap fireData = TextureUtil.LoadTexture("fire2.png");
    //     byte[] pixels = TextureUtil.PreloadTextureData(fireData);
    //     TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);
    //     
    //     SceneGraphNode sphere = new SceneGraphNode();
    //     Object sphereObject = new Object(sphere, fireTexture);
    //     _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(0,0,0))));
    //     MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris,5, new Vector3(0,1,0));
    // }
    //
    // private void CreateScene1()
    // {
    //     WriteableBitmap fireData = TextureUtil.LoadTexture("tile1.png");
    //     byte[] pixels = TextureUtil.PreloadTextureData(fireData);
    //     TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);
    //     
    //     SceneGraphNode cube = new SceneGraphNode();
    //     Object cubeObject = new Object(cube, fireTexture);
    //     _parent.Node.Children.Add((cubeObject, CreateTransformation(0, new Vector3(0,0,0))));
    //     MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0,1,0));
    //     
    //     SceneGraphNode sphere = new SceneGraphNode();
    //     Object sphereObject = new Object(sphere, null);
    //     MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 4, new Vector3(1,0,0));
    //     
    //     _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(0,3,0))));
    //     _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(0,-3,0))));
    //     _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(3,0,0))));
    //     _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(-3,0,0))));
    //     _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(0,0,3))));
    //     _parent.Node.Children.Add((sphereObject, CreateTransformation(0, new Vector3(0,0,-3))));
    //     
    // }
    //
    // private void CreateGroupedScene()
    // {
    //     Object group1 = new Object(new SceneGraphNode(), null, new Object.AnimatedProperty<object>(0f, (f, deltaTime) =>
    //     {
    //         f = ((float)f + 0.01f * deltaTime) % 360;
    //         return (f, Matrix4x4.CreateRotationX(float.DegreesToRadians((float)f)));
    //     }));
    //     Object cube1 = new Object(new SceneGraphNode(), null);
    //     MeshGenerator.AddSingleColorCube(cube1.Node.Vertices, cube1.Node.Tris, new Vector3(0,1,0));
    //     
    //     group1.Node.Children.Add((cube1, CreateTransformation(0, new Vector3(0,0,-2))));
    //     group1.Node.Children.Add((cube1, CreateTransformation(0, new Vector3(0,0,2))));
    //     group1.Node.Children.Add((cube1, CreateTransformation(0, new Vector3(0,2,0))));
    //     group1.Node.Children.Add((cube1, CreateTransformation(0, new Vector3(0,-2,0))));
    //     _parent.Node.Children.Add((group1, CreateTransformation(0, new Vector3(0,0,0))));
    //     
    //     Object group2 = new Object(new SceneGraphNode(), null, new Object.AnimatedProperty<object>(0f, (f, deltaTime) =>
    //     {
    //         f = ((float)f - 0.15f * deltaTime) % 360;
    //         return (f, Matrix4x4.CreateRotationY(float.DegreesToRadians((float)f)));
    //     }));
    //     Object sphere = new Object(new SceneGraphNode(), null);
    //     MeshGenerator.AddSphere(sphere.Node.Vertices, sphere.Node.Tris, 4,new Vector3(1,0,0));
    //     
    //     group2.Node.Children.Add((sphere, CreateTransformation(0, new Vector3(0,0,-4))));
    //     group2.Node.Children.Add((sphere, CreateTransformation(0, new Vector3(0,0,4))));
    //     group2.Node.Children.Add((sphere, CreateTransformation(0, new Vector3(-4,0,0))));
    //     group2.Node.Children.Add((sphere, CreateTransformation(0, new Vector3(4,0,0))));
    //     _parent.Node.Children.Add((group2, CreateTransformation(0, new Vector3(0,0,0))));
    // }

    private void CreateMultipleRotation()
    {
        SceneGraphNode group = new SceneGraphNode();
        Object groupObject = new Object(group, null);

        _parent.Node.Children.Add((groupObject, new SceneGraphNode.TransformationProperty<object>(0f,
            (rotation, deltaTime) =>
            {
                rotation = ((float)rotation + 0.0005f * deltaTime) % 360;
                var r = Matrix4x4.CreateRotationY((float)rotation);
                return (rotation, r);
            })));
        
        SceneGraphNode cube = new SceneGraphNode();
        Object cubeObject = new Object(cube, null, new Object.AnimatedProperty<object>(45f, (f, deltaTime) =>
        {
            f = ((float)f + 0.05f * deltaTime) % 360;
            return (f, Matrix4x4.CreateRotationX(float.DegreesToRadians((float)f)));
        }));
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0, 1, 0));

        groupObject.Node.Children.Add((cubeObject, new SceneGraphNode.TransformationProperty<object>(0f,
            (rotation, deltaTime) =>
            {
                rotation = ((float)rotation + 0.0005f * deltaTime) % 360;
                var r = Matrix4x4.CreateRotationX((float)rotation);
                var t = Matrix4x4.CreateTranslation(new Vector3(0, 0, -2));
                return (rotation, r * t);
            })));
        groupObject.Node.Children.Add((cubeObject, new SceneGraphNode.TransformationProperty<object>(0f,
            (rotation, deltaTime) =>
            {
                rotation = ((float)rotation + 0.0005f * deltaTime) % 360;
                var r = Matrix4x4.CreateRotationY((float)rotation);
                var t = Matrix4x4.CreateTranslation(new Vector3(2, 0, 2));
                return (rotation, r * t);
            })));
    }
    
    private void Render()
    {
        Image.Source = _rasterizer.Render();
    }
}