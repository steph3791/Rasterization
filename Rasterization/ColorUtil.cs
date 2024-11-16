using System.Numerics;
using System.Windows.Media;

namespace RayTracing;

public class ColorUtil
{
    private const float GammaCorrection = 2.2f;
    
    public static Vector3 ToLinearNormlized(Vector3 color)
    {
        Vector3 normalized = new Vector3(color.X / 255, color.Y / 255, color.Z / 255);
        float linearR = ApplyGammaCorrectionSRGBToLinear(normalized.X);
        float linearG = ApplyGammaCorrectionSRGBToLinear(normalized.Y);
        float linearB = ApplyGammaCorrectionSRGBToLinear(normalized.Z);
        return new Vector3(linearR, linearG, linearB);
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