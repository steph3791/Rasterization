using System.Numerics;
using RayTracing;

namespace Rasterization;

public class Object
{
    private SceneGraphNode _node;
    private TextureUtil.Texture? _texture;

    public Object(SceneGraphNode node, TextureUtil.Texture? texture)
    {
        _node = node;
        _texture = texture;
    }

    public SceneGraphNode Node => _node;

    public TextureUtil.Texture? Texture => _texture;
    
    public Vector3 GetDiffuseRenderColor(Vertex v)
    {
        return this._texture != null
            ? GetTextureColor(v.ST.X, v.ST.Y)
            : v.Color;
    }
    
    private Vector3 GetTextureColor(float s, float t)
    {
        if (!_texture.HasValue)
        {
            throw new Exception("Texture could not be loaded. Ensure that sphere has assigned a texture.");
        }
        int textureX = (int)(s * _texture.Value.Width);
        int textureY = (int)(t * _texture.Value.Height);
        textureX = Math.Clamp(textureX, 0, _texture.Value.Width-1);
        textureY = Math.Clamp(textureY,0, _texture.Value.Height-1);
        
        int step = _texture.Value.Width * 4;
        byte[] pixelData = _texture.Value.Pixels;
        
        int pixelIndex = textureY * step + textureX * 4;
        byte blue = pixelData[pixelIndex];
        byte green = pixelData[pixelIndex + 1];
        byte red = pixelData[pixelIndex + 2];
        return ColorUtil.ToLinearNormlized(new Vector3(red, green, blue));
    }

}