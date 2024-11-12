using System.Numerics;

namespace Rasterization;

public class SceneGraphNode
{
    List<Vertex> _vertices = new List<Vertex>();
    List<(int A, int B, int C)> _tris = new List<(int A, int B, int C)>();
    List<(SceneGraphNode Node, Matrix4x4 Transformation)> _children;

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

    public List<(SceneGraphNode Node, Matrix4x4 Transformation)> Children
    {
        get => _children;
        set => _children = value ?? throw new ArgumentNullException(nameof(value));
    }

    void Render(Matrix4x4 modelMatrix, Matrix4x4 viewProjectionMatrix, Rasterizer rasterizer)
    {
        var mNormal = rasterizer.CalculateNormalMatrix(modelMatrix);
        var mvp = modelMatrix * viewProjectionMatrix;
        rasterizer.Render();
        foreach(var child in Children)
            child.Node.Render(child.Transformation * modelMatrix, viewProjectionMatrix, rasterizer);
    }   
}