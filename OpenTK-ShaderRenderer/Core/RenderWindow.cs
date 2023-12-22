using OpenTK.Windowing.Desktop;

namespace OpenTK_Renderer;

public partial class RenderWindow : GameWindow
{
    protected RenderSetting RenderSetting;

    public RenderWindow(int width, int height, string title, int fps = 60) : base(new GameWindowSettings() { UpdateFrequency = fps }, new NativeWindowSettings{ Title = title, ClientSize = (width, height) })
    {
        RenderSetting = new RenderSetting();
    }
}

/// <summary>
/// Setting struct for rendering
/// </summary>
public struct RenderSetting
{
    public bool RenderSkybox;

    public RenderSetting()
    {
        RenderSkybox = true;
    }
}
