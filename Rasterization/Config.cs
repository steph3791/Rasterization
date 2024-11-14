using System.Numerics;

namespace Rasterization;

public class Config
{
    public static bool Animate = true;
    public static bool BilinearFiltering = true;
    public static Vector3 CameraPosition = new Vector3(0, 0, -6);
    public static Vector3 DefaultLightSource = new Vector3(-6, 2, -8);
}