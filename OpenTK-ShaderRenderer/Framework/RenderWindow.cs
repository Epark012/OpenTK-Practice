using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace OpenTK_Renderer;

public class RenderWindow : GameWindow
{
    protected RenderSetting RenderSetting;

    public RenderWindow(int width, int height, string title, int targetFrame = 60) : base(new GameWindowSettings() { UpdateFrequency = targetFrame }, new NativeWindowSettings{ Title = title, ClientSize = (width, height)})
    {
        RenderSetting = new RenderSetting();
    }
}

/// <summary>
/// Setting struct for rendering
/// </summary>
public struct RenderSetting
{
    /// <summary>
    /// Value either drawing skybox or not
    /// </summary>
    public bool RenderSkybox { get; set; } = true;
    
    /// <summary>
    /// Default background color
    /// </summary>
    public readonly Color4 DefaultBackGroundColor = new (0.2f, 0.3f, 0.3f, 1.0f);
    
    /// <summary>
    /// Default cubemap textures path
    /// </summary>
    public string[] DefaultCubeMapPath = {
        "Resources/Image/CubeMap/right.jpg", "Resources/Image/CubeMap/left.jpg", "Resources/Image/CubeMap/top.jpg",
        "Resources/Image/CubeMap/bottom.jpg", "Resources/Image/CubeMap/front.jpg", "Resources/Image/CubeMap/back.jpg"
    };

    public RenderSetting() { }
}
