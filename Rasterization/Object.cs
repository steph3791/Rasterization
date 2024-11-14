using System.Data;
using System.Numerics;
using RayTracing;

namespace Rasterization;

public class Object
{
    private SceneGraphNode _node;
    private TextureUtil.Texture? _texture;
    private AnimatedProperty<object>[]? _animatedProperties;


    public Object(SceneGraphNode node, TextureUtil.Texture? texture, params AnimatedProperty<object>[] animatedProperties)
    {
        _node = node;
        _texture = texture;
        _animatedProperties = animatedProperties;
    }

    public SceneGraphNode Node => _node;

    public TextureUtil.Texture? Texture => _texture;
    public AnimatedProperty<object>[]? AnimatedProperties => _animatedProperties;

    
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
            
            Vector3 topLeft = GetColor(s0, t0);          // C[|s|, |t|]
            Vector3 topRight = GetColor(s0 + 1, t0);     // C[|s| + 1, |t|]
            Vector3 bottomLeft = GetColor(s0, t0 + 1);   // C[|s|, |t| + 1]
            Vector3 bottomRight = GetColor(s0 + 1, t0 + 1); // C[|s| + 1, |t| + 1]

            Vector3 left = Vector3.Lerp(topLeft, bottomLeft, fracY);
            Vector3 right = Vector3.Lerp(topRight, bottomRight, fracY);

            // Interpolate vertically between the two results
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

    public class AnimatedProperty<T>
    {
        private T _value;
        private readonly Func<T, float, (T value, Matrix4x4 matrix)> _animationUpdate;

        public T Value
        {
            get => _value;
            set => _value = value;
        }
        
        // Constructor
        public AnimatedProperty(T value, Func<T, float, (T value, Matrix4x4 matrix)> animationUpdate)
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