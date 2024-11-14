using System.Numerics;
using System.Windows.Media;

namespace RayTracing;

public class ColorUtil
{
    private const float GammaCorrection = 2.2f;
    
    public static readonly Vector3 RED = new Vector3(1, 0, 0);
    public static readonly Vector3 BLUE = new Vector3(0, 0, 1);
    public static readonly Vector3 YELLOW = new Vector3(1, 1, 0);
    public static readonly Vector3 LIGHTCYAN = new Vector3(0.875f, 1, 1);
    public static readonly Vector3 GRAY = new Vector3(0.5f, 0.5f, 0.5f);
    public static readonly Vector3 BLACK = new Vector3(0, 0, 0);
    public static readonly Vector3 WHITE = new Vector3(1, 1, 1);
    
    private static Vector3 ToSrgbColor(float r, float g, float b)
    {
        float R = ApplyGammaCorrectionLinearToSRGB(r);
        float G = ApplyGammaCorrectionLinearToSRGB(g);
        float B = ApplyGammaCorrectionLinearToSRGB(b);
        return new Vector3(R, G, B);
    }
    
    public static Vector3 ToSrgbColor(Vector3 color)
    {
        return ToSrgbColor(color.X, color.Y, color.Z);
    }
    
    public static Vector3 ToLinearNormlized(Vector3 color)
    {
        Vector3 normalized = new Vector3(color.X / 255, color.Y / 255, color.Z / 255);
        float linearR = ApplyGammaCorrectionSRGBToLinear(normalized.X);
        float linearG = ApplyGammaCorrectionSRGBToLinear(normalized.Y);
        float linearB = ApplyGammaCorrectionSRGBToLinear(normalized.Z);
        return new Vector3(linearR, linearG, linearB);
    }
    
    private static float ApplyGammaCorrectionLinearToSRGB(float linearColor)
    {
        // sRGB gamma correction
        if (linearColor <= 0.0031308f)
        {
            return linearColor * 12.92f;
        }
        else
        {
            return MathF.Pow(linearColor, 1.0f /GammaCorrection);
        }
    }

    private static float ApplyGammaCorrectionSRGBToLinear(float srgbColor)
    {
        if (srgbColor <= 0.04045)
        {
            return srgbColor / 12.92f;
        }
        else
        {
            return MathF.Pow(srgbColor, GammaCorrection);
        }
    }
}