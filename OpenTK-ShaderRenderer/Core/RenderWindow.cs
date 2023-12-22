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
    public bool RenderSkybox;

    public RenderSetting()
    {
        RenderSkybox = true;
    }
}
