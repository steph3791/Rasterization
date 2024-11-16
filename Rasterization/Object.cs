using System.Data;
using System.Numerics;
using RayTracing;
// ReSharper disable All

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
        float textureX = s * _texture.Value.Width;
        float textureY = t * _texture.Value.Height;
        
        if (Config.BilinearFiltering)
        {
            int s0 = (int) MathF.Floor(textureX);
            int t0 = (int) MathF.Floor(textureY);

            float fracX = textureX - s0;
            float fracY = textureY - t0;
            
            Vector3 topLeft = GetColor(s0, t0);             // C[|s|, |t|]
            Vector3 topRight = GetColor(s0 + 1, t0);        // C[|s| + 1, |t|]
            Vector3 bottomLeft = GetColor(s0, t0 + 1);      // C[|s|, |t| + 1]
            Vector3 bottomRight = GetColor(s0 + 1, t0 + 1); // C[|s| + 1, |t| + 1]

            Vector3 left = Vector3.Lerp(topLeft, bottomLeft, fracY);
            Vector3 right = Vector3.Lerp(topRight, bottomRight, fracY);

            return Vector3.Lerp(left, right, fracX);
        }
        return GetColor((int)MathF.Floor(textureX), (int)MathF.Floor(textureY));
    }

    private Vector3 GetColor(int textureX, int textureY)
    {
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