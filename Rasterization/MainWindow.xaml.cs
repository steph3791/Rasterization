using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;

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
        WireframeCubeWithoutBackfaceCulling();
        // WireframeCubeWithBackfaceCulling();
        // WireframeSphereWithoutBackfaceCulling();
        // WireframeSphereWithBackfaceCulling();

        // AmbientLight();
        // DiffuseLight();
        // SpecularLight();
        // CombinedLight();

        // TexturedCube();
        // TexturedSphere();

        // NoZBuffer();
        // ZBuffer();
        
        // Scene1();
        // Scene2();
        
        // ShowCaseBilinear(true);
        // ShowCaseBilinear(false);
    }


    #region Wireframes

    private void WireframeCubeWithoutBackfaceCulling()
    {
        Config.RenderWireframe = true;
        Config.DoBackfaceCulling = false;
        Config.CameraPosition = new Vector3(0, 0, -4f);

        Object cubeObject = new Object(new SceneGraphNode(), null);
        _parent.Node.Children.Add((cubeObject, CreateTransformation()));
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0, 1, 0));
    }

    private void WireframeCubeWithBackfaceCulling()
    {
        Config.RenderWireframe = true;
        Config.DoBackfaceCulling = true;
        Config.CameraPosition = new Vector3(0, 0, -4f);

        Object cubeObject = new Object(new SceneGraphNode(), null);
        _parent.Node.Children.Add((cubeObject, CreateTransformation()));
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0, 1, 0));
    }

    private void WireframeSphereWithoutBackfaceCulling()
    {
        Config.RenderWireframe = true;
        Config.DoBackfaceCulling = false;
        Config.CameraPosition = new Vector3(0, 0, -4f);

        Object sphere = new Object(new SceneGraphNode(), null);
        _parent.Node.Children.Add((sphere, CreateTransformation()));
        MeshGenerator.AddSphere(sphere.Node.Vertices, sphere.Node.Tris, 5, new Vector3(0, 1, 0));
    }

    private void WireframeSphereWithBackfaceCulling()
    {
        Config.RenderWireframe = true;
        Config.DoBackfaceCulling = true;
        Config.CameraPosition = new Vector3(0, 0, -4f);

        Object sphere = new Object(new SceneGraphNode(), null);
        _parent.Node.Children.Add((sphere, CreateTransformation()));
        MeshGenerator.AddSphere(sphere.Node.Vertices, sphere.Node.Tris, 5, new Vector3(0, 1, 0));
    }

    #endregion

    #region Lighting

    private void AmbientLight()
    {
        Config.DiffuseLighting = false;
        Config.SpecularLighting = false;
        Config.CameraPosition = new Vector3(0, 0, -6f);
        Config.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.25f);
        Object cubeObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(1, 0, 0));
        Object sphereObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 5, new Vector3(0, 0, 1));
        _parent.Node.Children.Add((cubeObject, CreateTransformation(translationY: 2f)));
        _parent.Node.Children.Add((sphereObject, CreateTransformation(scale: 1.5f, translationY: -2f)));
    }

    private void DiffuseLight()
    {
        Config.DiffuseLighting = true;
        Config.SpecularLighting = false;
        Config.CameraPosition = new Vector3(0, 0, -6f);
        Config.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
        Config.DefaultLightSource = new Vector3(-6, 5, -6);

        Object cubeObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(1, 0, 0));
        Object sphereObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 5, new Vector3(0, 0, 1));
        _parent.Node.Children.Add((cubeObject, CreateTransformation(translationY: 2f)));
        _parent.Node.Children.Add((sphereObject, CreateTransformation(scale: 1.5f, translationY: -2f)));
    }

    private void SpecularLight()
    {
        Config.DiffuseLighting = false;
        Config.SpecularLighting = true;
        Config.CameraPosition = new Vector3(0, 0, -6f);
        Config.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
        Config.DefaultLightSource = new Vector3(-6, 5, -6);

        Object cubeObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(1, 0, 0));
        Object sphereObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 5, new Vector3(0, 0, 1));
        _parent.Node.Children.Add((cubeObject, CreateTransformation(translationY: 2f)));
        _parent.Node.Children.Add((sphereObject, CreateTransformation(scale: 1.5f, translationY: -2f)));
    }

    private void CombinedLight()
    {
        Config.DiffuseLighting = true;
        Config.SpecularLighting = true;
        Config.CameraPosition = new Vector3(0, 0, -6f);
        Config.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
        Config.DefaultLightSource = new Vector3(-6, 5, -6);

        Object cubeObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(1, 0, 0));
        Object sphereObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 5, new Vector3(0, 0, 1));
        _parent.Node.Children.Add((cubeObject, CreateTransformation(translationY: 2f)));
        _parent.Node.Children.Add((sphereObject, CreateTransformation(scale: 1.5f, translationY: -2f)));
    }

    #endregion

    #region Textures

    private void TexturedCube()
    {
        Config.CameraPosition = new Vector3(0, 0, -4f);
        WriteableBitmap tile = TextureUtil.LoadTexture("tile.png");
        byte[] pixels = TextureUtil.PreloadTextureData(tile);
        TextureUtil.Texture tileTexture = new TextureUtil.Texture(pixels, tile.PixelWidth, tile.PixelWidth);

        SceneGraphNode cube = new SceneGraphNode();
        Object cubeObject = new Object(cube, tileTexture);
        _parent.Node.Children.Add((cubeObject, CreateTransformation()));
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0, 1, 0));
    }

    private void TexturedSphere()
    {
        Config.CameraPosition = new Vector3(0, 0, -4f);

        WriteableBitmap fireData = TextureUtil.LoadTexture("water.png");
        byte[] pixels = TextureUtil.PreloadTextureData(fireData);
        TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);

        SceneGraphNode sphere = new SceneGraphNode();
        Object sphereObject = new Object(sphere, fireTexture);
        _parent.Node.Children.Add((sphereObject, CreateTransformation()));
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 32, new Vector3(0, 1, 0));
    }

    #endregion

    #region ZBuffer

    private void NoZBuffer()
    {
        Config.UseZBuffer = false;
        Object cubeObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(1, 0, 0));
        Object sphereObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 5, new Vector3(0, 0, 0.5f));
        _parent.Node.Children.Add((cubeObject, CreateTransformation(translationY: 1f)));
        _parent.Node.Children.Add((sphereObject, CreateTransformation(scale: 1.5f, translationY: -0.5f)));
    }

    private void ZBuffer()
    {
        Config.UseZBuffer = true;
        Object cubeObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(1, 0, 0));
        Object sphereObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 5, new Vector3(0, 0, 0.5f));
        _parent.Node.Children.Add((cubeObject, CreateTransformation(translationY: 1f)));
        _parent.Node.Children.Add((sphereObject, CreateTransformation(scale: 1.5f, translationY: -0.5f)));
    }

    #endregion

    #region Hierarchies

    private void Scene1()
    {
        WriteableBitmap fireData = TextureUtil.LoadTexture("tile.png");
        byte[] pixels = TextureUtil.PreloadTextureData(fireData);
        TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels, fireData.PixelWidth, fireData.PixelWidth);

        Object cubeObject = new Object(new SceneGraphNode(), fireTexture);
        MeshGenerator.AddSingleColorCube(cubeObject.Node.Vertices, cubeObject.Node.Tris, new Vector3(0, 1, 0));

        Object sphereObject = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(sphereObject.Node.Vertices, sphereObject.Node.Tris, 4, new Vector3(1, 0, 0));

        Object group = new Object(new SceneGraphNode(), null);

        _parent.Node.Children.Add((cubeObject, CreateTransformation(rotationDir: Direction.X)));
        _parent.Node.Children.Add((group, CreateTransformation()));

        group.Node.Children.Add((sphereObject, CreateTransformation(translationY: 3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationY: -3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationX: 3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationX: -3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationZ: 3)));
        group.Node.Children.Add((sphereObject, CreateTransformation(translationZ: -3)));
    }

    private void Scene2()
    {
        Config.DefaultLightSource = new Vector3(0, 0, 0);
        Config.Lights.Add(new Light(new Vector3(1,0,0),Config.DefaultLightSourceColor));
        Config.Lights.Add(new Light(new Vector3(-1,0,0),Config.DefaultLightSourceColor));
        Config.Lights.Add(new Light(new Vector3(0,0,1),Config.DefaultLightSourceColor));
        Config.Lights.Add(new Light(new Vector3(0,0,-1),Config.DefaultLightSourceColor));
        Config.Lights.Add(new Light(new Vector3(0,1,0),Config.DefaultLightSourceColor));
        Config.Lights.Add(new Light(new Vector3(0,-1,0),Config.DefaultLightSourceColor));
        
        WriteableBitmap tile = TextureUtil.LoadTexture("water.png");
        byte[] pixels = TextureUtil.PreloadTextureData(tile);
        TextureUtil.Texture waterTexture = new TextureUtil.Texture(pixels, tile.PixelWidth, tile.PixelWidth);
        
        WriteableBitmap tile1 = TextureUtil.LoadTexture("fire.png");
        byte[] pixels1 = TextureUtil.PreloadTextureData(tile1);
        TextureUtil.Texture fireTexture = new TextureUtil.Texture(pixels1, tile1.PixelWidth, tile1.PixelWidth);
        
        WriteableBitmap tile2 = TextureUtil.LoadTexture("dirt.png");
        byte[] pixels2 = TextureUtil.PreloadTextureData(tile2);
        TextureUtil.Texture dirtTexture = new TextureUtil.Texture(pixels2, tile2.PixelWidth, tile2.PixelWidth);
        
        //Main Group for Y-rotation
        Object group = new Object(new SceneGraphNode(), null);
        _parent.Node.Children.Add((group, CreateTransformation(updateRotationSpeed: 0.00005f, rotationDir: Direction.Y)));
        
        //Centre Orb
        Object orb = new Object(new SceneGraphNode(), null);
        MeshGenerator.AddSphere(orb.Node.Vertices, orb.Node.Tris, 4, new Vector3(1, 1, 1));
        group.Node.Children.Add((orb, CreateTransformation()));
        
        //SubGroup1
        Object group1 = new Object(new SceneGraphNode(), null);
        Object dirt = new Object(new SceneGraphNode(), dirtTexture);
        MeshGenerator.AddSphere(dirt.Node.Vertices, dirt.Node.Tris, 4, new Vector3(0, 1, 0));
        
        //Instantiate Dirt in Circle
        float radius = 4f;
        float numObjects = 12f;
        float increment = 360f / numObjects;
        for (int i = 0; i < numObjects; i++)
        {
            float angle = float.DegreesToRadians(i * increment);
            float x = radius * MathF.Cos(angle);
            float y = radius * MathF.Sin(angle);
            group1.Node.Children.Add((dirt,
                CreateTransformation(rotationDir: Direction.None, scale: 0.25f, translationY: x, translationZ: y)));
        }
        group.Node.Children.Add((group1, CreateTransformation(updateRotationSpeed: 0.0005f, rotationDir: Direction.X)));

        //SubGroup2
        Object group2 = new Object(new SceneGraphNode(), null);
        Object water = new Object(new SceneGraphNode(), waterTexture);
        MeshGenerator.AddSphere(water.Node.Vertices, water.Node.Tris, 4, new Vector3(1, 0, 0));
        
        //Instantiate Water in circle
        radius = 4f;
        numObjects = 12f;
        increment = 360f / numObjects;
        for (int i = 0; i < numObjects; i++)
        {
            float angle = float.DegreesToRadians(i * increment);
            float x = radius * MathF.Cos(angle);
            float y = radius * MathF.Sin(angle);
            group2.Node.Children.Add((water,
                CreateTransformation(rotationDir: Direction.None, scale: 0.25f, translationX: x, translationZ: y)));
        }
        _parent.Node.Children.Add((group2, CreateTransformation(updateRotationSpeed: 0.00045f,rotationDir: Direction.Y)));

        //SubGroup3
        Object group3 = new Object(new SceneGraphNode(), null);
        Object fire = new Object(new SceneGraphNode(), fireTexture);
        MeshGenerator.AddSphere(fire.Node.Vertices, fire.Node.Tris, 4, new Vector3(0, 0, 1));
        
        //Instantiate fire in Circle
        radius = 4f;
        numObjects = 12f;
        increment = 360f / numObjects;
        for (int i = 0; i < numObjects; i++)
        {
            float angle = float.DegreesToRadians(i * increment);
            float x = radius * MathF.Cos(angle);
            float y = radius * MathF.Sin(angle);
            group3.Node.Children.Add((fire,
                CreateTransformation(rotationDir: Direction.None, scale: 0.25f, translationX: x, translationY: y)));
        }
        group.Node.Children.Add((group3, CreateTransformation(updateRotationSpeed: 0.0005f,rotationDir: Direction.Z)));
    }

    #endregion

    #region Bilinear
    private void ShowCaseBilinear(bool bilinear)
    {
        Config.Animate = false;
        Config.BilinearFiltering = bilinear;
        Config.DefaultLightSource = Config.CameraPosition with { Y = 4f };
        WriteableBitmap tile = TextureUtil.LoadTexture("portal.png");
        byte[] pixels = TextureUtil.PreloadTextureData(tile);
        TextureUtil.Texture texture = new TextureUtil.Texture(pixels, tile.PixelWidth, tile.PixelWidth);

        Object obj = new Object(new SceneGraphNode(), texture);
        MeshGenerator.AddSingleColorCube(obj.Node.Vertices, obj.Node.Tris, new Vector3(0, 1, 0));
        _parent.Node.Children.Add((obj, CreateTransformation(translationZ: -3.75f)));
    }

    
    #endregion
    
    private SceneGraphNode.TransformationProperty<object> CreateTransformation(Direction rotationDir = Direction.Y,
        float startRotation = 0, float updateRotationSpeed = 0.001f, float scale = 1, float translationX = 0f,
        float translationY = 0f, float translationZ = 0f)
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
                return (rotation, s * r * t);
            });
    }

    private void Render()
    {
        WriteableBitmap bitmap = _rasterizer.Render();
        Image.Source = bitmap;
        if (!saved)
        {
            SaveWriteableBitmapAsPng(bitmap, "render");
            saved = true;
        }
    }

    private bool saved;

    private void SaveWriteableBitmapAsPng(WriteableBitmap bitmap, string presetName)
    {
        Console.WriteLine($"Saving Image for: {presetName}");
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{presetName}.png");

        string directory = Path.GetDirectoryName(path);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path);

        string newFilePath = path;
        int counter = 1;

        while (File.Exists(newFilePath))
        {
            newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}_{counter}{extension}");
            counter++;
        }

        PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
        pngEncoder.Frames.Add(BitmapFrame.Create(bitmap));
        using (var fileStream = new FileStream(newFilePath, FileMode.Create))
        {
            pngEncoder.Save(fileStream);
        }
    }

    private enum Direction
    {
        X,
        Y,
        Z,
        None
    }
}