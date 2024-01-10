using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTK_Renderer;

public class RenderWindow : GameWindow
{
    protected RenderSetting RenderSetting;

    /// <summary>
    /// Constructor setting window state
    /// </summary>
    /// <param name="title">Title of window</param>
    /// <param name="windowState">Window state for window</param>
    /// <param name="targetFrameRate">Target frame rate to render</param>
    public RenderWindow(WindowState windowState, string title, int targetFrameRate = 60) : base(new GameWindowSettings() {UpdateFrequency = targetFrameRate}, new NativeWindowSettings{ Title = title, WindowState = windowState})
    {
        RenderSetting = new RenderSetting();
    }
    
    /// <summary>
    /// Constructor setting window size
    /// </summary>
    /// <param name="width">Width of window</param>
    /// <param name="height">Height of window</param>
    /// <param name="title">Title of window</param>
    /// <param name="targetFrameRate">Target frame rate to render</param>
    public RenderWindow(int width, int height, string title, int targetFrameRate = 60) : base(new GameWindowSettings() { UpdateFrequency = targetFrameRate }, new NativeWindowSettings{ Title = title, ClientSize = (width, height)})
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
