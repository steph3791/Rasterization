using System.Numerics;

namespace Rasterization;

public class SceneGraphNode
{
    List<Vertex> _vertices = new List<Vertex>();
    List<(int A, int B, int C)> _tris = new List<(int A, int B, int C)>();
    List<(Object child, TransformationProperty<object> transformations)> _children = new List<(Object, TransformationProperty<object>)>();

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

    public List<(Object child, TransformationProperty<object> transformations)> Children
    {
        get => _children;
        set => _children = value ?? throw new ArgumentNullException(nameof(value));
    }
    
    public class TransformationProperty<T>(T value, Func<T, float, (T value, Matrix4x4 matrix)> animationUpdate)
    {
        private T _value = value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }
        
        public Matrix4x4 GetTransformation(float deltaTime)
        {
            var data = animationUpdate(_value, deltaTime);
            _value = data.value;
            return data.matrix;
        }
    }

}