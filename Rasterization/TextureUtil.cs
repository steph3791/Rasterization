using System.Numerics;
using System.Windows.Media.Imaging;
using RayTracing;

namespace Rasterization;

public class TextureUtil
{
    public static WriteableBitmap LoadTexture(string path)
    {
        BitmapImage bitmapImage = new BitmapImage(new Uri("Textures/" + path, UriKind.Relative));
        return new WriteableBitmap(bitmapImage);
    }
    
    public static byte[] PreloadTextureData(WriteableBitmap texture)
    {
        int stride = texture.PixelWidth * 4; 
        byte[] pixelData = new byte[stride * texture.PixelHeight];
        texture.CopyPixels(pixelData, stride, 0);
        return pixelData;
    }
    
    public struct Texture(byte[] pixels, int width, int height)
    {
        public byte[] Pixels => pixels;

        public int Width => width;

        public int Height => height;
    }
}

