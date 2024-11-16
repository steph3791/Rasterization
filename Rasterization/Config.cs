using System.Numerics;

namespace Rasterization;

public class Config
{
    public static bool Animate = true;
    public static bool BilinearFiltering = true;
    public static bool RenderWireframe = false;
    public static bool DoBackfaceCulling = true;
    public static bool SpecularLighting = true;
    public static bool DiffuseLighting = true;
    public static bool UseZBuffer = true;
    public static float kPhongFactor = 25f;
    public static Vector3 AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
    public static Vector3 CameraPosition = new Vector3(0, 0, -6);
    public static Vector3 DefaultLightSource = new Vector3(-6, 2, -8);
    public static Vector3 DefaultLightSourceColor = new Vector3(1, 1, 1);
    public static List<Light> Lights = new List<Light>();
}