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
    Rasterizer _rasterizer;
    Animator _animator;
    private Object _parent;
    private const int Size = 450;
    
    public MainWindow()
    {
        InitializeComponent();
        Width = Height = Size;
        _animator = new Animator(16);
        RenderSceneGraph();
    }
    
    void RenderSceneGraph()
    {
        _parent = new Object(new SceneGraphNode(), null);
        CreateSceneGraph();
        _rasterizer = new Rasterizer(Size, Size, _parent, _animator);
        _animator.RegisterAnimation(Render);
        _animator.Start();
    }

    private void CreateSceneGraph()
    {
        // CreateSingleCube();
        // CreateMultiColorCube();
        // CreateSingleSphere();
        // CreateTexturedCube();
        // CreateTexturedSphere();
        CreateScene1();
        // CreateGroupedScene();
    }

    private void CreateSingleCube()
    {
        SceneGraphNode cube = new SceneGraphNode();
        Object cubeObject = new Object(cube, null);
        _parent.Node.Children.Add((cubeObject, CreateTransformation()));
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0,1,0));
    }
    
    private void CreateMultiColorCube()
    {
        SceneGraphNode cube = new SceneGraphNode();
        Object cubeObject = new Object(cube, null);
        _parent.Node.Children.Add((cubeObject, CreateTransformation()));
        MeshGenerator.AddCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, 
            new Vector3(0,1,0),
            new Vector3(1,0,0),
            new Vector3(0,0,1),
            new Vector3(1,1,0),
            new Vector3(0,1,1),
            new Vector3(1,0,1)
            );
    }

    private void CreateSingleSphere()
    {
        SceneGraphNode sphere = new SceneGraphNode();
        Object sphereObject = new Object(sphere, null);
        _parent.Node.Children.Add((sphereObject, CreateTransformation(scale:1.5f)));
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris,5, new Vector3(0,1,0));
    }
    
    private void CreateTexturedCube()
    {
        WriteableBitmap fireData = TextureUtil.LoadTexture("tile1.png");
        byte[] pixels = TextureUtil.PreloadTextureData(fireData);
        TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);
        
        SceneGraphNode cube = new SceneGraphNode();
        Object cubeObject = new Object(cube, fireTexture);
        _parent.Node.Children.Add((cubeObject, CreateTransformation()));
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0,1,0));
    }
    
    private void CreateTexturedSphere()
    {
        WriteableBitmap fireData = TextureUtil.LoadTexture("fire2.png");
        byte[] pixels = TextureUtil.PreloadTextureData(fireData);
        TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);
        
        SceneGraphNode sphere = new SceneGraphNode();
        Object sphereObject = new Object(sphere, fireTexture);
        _parent.Node.Children.Add((sphereObject, CreateTransformation()));
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris,5, new Vector3(0,1,0));
    }
    
    private void CreateScene1()
    {
        WriteableBitmap fireData = TextureUtil.LoadTexture("tile1.png");
        byte[] pixels = TextureUtil.PreloadTextureData(fireData);
        TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);
        
        Object cubeObject = new Object(new SceneGraphNode(), fireTexture);
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0,1,0));

        Object sphereObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 4, new Vector3(1,0,0));
        
        Object group = new Object(new SceneGraphNode(), null);
        
        _parent.Node.Children.Add((cubeObject, CreateTransformation(rotationDir:Direction.X)));
        _parent.Node.Children.Add((group, CreateTransformation()));
        
        group.Node.Children.Add((sphereObject, CreateTransformation(translationY: 3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationY:-3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationX: 3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationX:-3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationZ: 3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationZ:-3)));
    }
    
    private void CreateGroupedScene()
    {
        Object group1 = new Object(new SceneGraphNode(), null);
        Object cube1 = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSingleColorCube(cube1.Node.Vertices, cube1.Node.Tris, new Vector3(0,1,0));
        
        group1.Node.Children.Add((cube1, CreateTransformation(rotationDir: Direction.X, scale:0.5f, translationZ:-2)));
        group1.Node.Children.Add((cube1, CreateTransformation(rotationDir: Direction.X, scale:0.5f, translationZ: 2)));
        group1.Node.Children.Add((cube1, CreateTransformation(rotationDir: Direction.X, scale:0.5f, translationY: 2)));
        group1.Node.Children.Add((cube1, CreateTransformation(rotationDir: Direction.X, scale:0.5f, translationY:-2)));
        _parent.Node.Children.Add((group1, CreateTransformation()));
        
        Object group2 = new Object(new SceneGraphNode(), null);
        Object sphere = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphere.Node.Vertices, sphere.Node.Tris, 4,new Vector3(1,0,0));

        float radius = 4f;
        float numObjects = 12f;
        float increment = 360f / numObjects;
        for (int i = 0; i < numObjects; i++)
        {
            float angle = float.DegreesToRadians(i * increment);
            float x = radius * MathF.Cos(angle);
            float y = radius * MathF.Sin(angle);
            group2.Node.Children.Add((sphere, CreateTransformation(rotationDir: Direction.None, scale: 0.25f, translationX: x, translationZ:y)));
        }
        _parent.Node.Children.Add((group2, CreateTransformation(rotationDir:Direction.Y)));
        
        Object group3 = new Object(new SceneGraphNode(), null);
        group3.Node.Children.Add((sphere, CreateTransformation(rotationDir: Direction.None, scale: 0.25f, translationX: 2, translationZ: -2)));
        group3.Node.Children.Add((sphere, CreateTransformation(rotationDir: Direction.None, scale: 0.25f, translationX:-2, translationZ: 2)));
        group3.Node.Children.Add((sphere, CreateTransformation(rotationDir: Direction.None, scale: 0.25f, translationX: 2, translationZ: 2)));
        group3.Node.Children.Add((sphere, CreateTransformation(rotationDir: Direction.None, scale: 0.25f, translationX:-2, translationZ:-2)));
        _parent.Node.Children.Add((group3, CreateTransformation(rotationDir:Direction.X)));

    }
    
    private SceneGraphNode.TransformationProperty<object> CreateTransformation(Direction rotationDir = Direction.Y, float startRotation = 0, float updateRotationSpeed = 0.001f, float scale = 1, float translationX = 0f, float translationY = 0f, float translationZ = 0f)
    {
        return new SceneGraphNode.TransformationProperty<object>(startRotation,
            (rotation, deltaTime) =>
            {
                rotation = ((float)rotation + updateRotationSpeed * deltaTime) % 360;
                var s = Matrix4x4.CreateScale(scale);
                Matrix4x4 r = Matrix4x4.Identity;
                switch (rotationDir)
                {
                    case Direction.X:
                        r = Matrix4x4.CreateRotationX((float)rotation);
                        break;
                    case Direction.Y:
                        r = Matrix4x4.CreateRotationY((float)rotation);
                        break;
                    case Direction.Z:
                        r = Matrix4x4.CreateRotationZ((float)rotation);
                        break;
                    case Direction.None:
                        break;
                }
                var t = Matrix4x4.CreateTranslation(new Vector3(translationX, translationY, translationZ));
                return (rotation, s*r * t);
            });
    }
    
    private void Render()
    {
        Image.Source = _rasterizer.Render();
    }

    private enum Direction { X,Y,Z, None }
}