using System.Numerics;

namespace Rasterization;

public class SceneGraphNode
{
    List<Vertex> _vertices = new List<Vertex>();
    List<(int A, int B, int C)> _tris = new List<(int A, int B, int C)>();
    List<(Object child, Matrix4x4 Transformation)> _children = new List<(Object, Matrix4x4)>();

    public List<Vertex> Vertices
    {
        get => _vertices;
        set => _vertices = value ?? throw new ArgumentNullException(nameof(value));
    }

    public List<(int A, int B, int C)> Tris
    {
        get => _tris;
        set => _tris = value ?? throw new ArgumentNullException(nameof(value));
    }

    public List<(Object child, Matrix4x4 Transformation)> Children
    {
        get => _children;
        set => _children = value ?? throw new ArgumentNullException(nameof(value));
    }

    void Render(Matrix4x4 modelMatrix, Matrix4x4 viewProjectionMatrix, Rasterizer rasterizer)
    {
        var mNormal = rasterizer.CalculateNormalMatrix(modelMatrix);
        var mvp = modelMatrix * viewProjectionMatrix;
        rasterizer.Render();
        // foreach(var child in Children)
            // child.Node.Render(child.Transformation * modelMatrix, viewProjectionMatrix, rasterizer);
    }   
    
    public class TransformationProperty<T>
    {
        private T _value;
        private readonly Func<T, float, (T value, Matrix4x4 matrix)> _animationUpdate;

        public T Value
        {
            get => _value;
            set => _value = value;
        }
        
        // Constructor
        public TransformationProperty(T value, Func<T, float, (T value, Matrix4x4 matrix)> animationUpdate)
        {
            Console.WriteLine("NEw instance");
            _value = value;
            _animationUpdate = animationUpdate;
        }

        public Matrix4x4 GetTransformation(float deltaTime)
        {
            var data = _animationUpdate(_value, deltaTime);
            Console.WriteLine("Value: " + data.value);
            _value = data.value;
            Console.WriteLine("NEw Value: " + _value);
            return data.matrix;
        }
    }

}