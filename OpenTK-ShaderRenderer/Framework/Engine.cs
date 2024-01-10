using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTK_Renderer;

public class Engine : IDisposable
{
    private MainRenderWindow _mainRenderWindow;
    
    public Engine()
    {
        // TODO get proper resolution 
        _mainRenderWindow = new MainRenderWindow(WindowState.Maximized, "Main Window");
        var area = Monitors.GetMonitorFromWindow(_mainRenderWindow).ClientArea;
        // Console.WriteLine($"Setting - {area.Size.X}, {area.Size.Y}");
    }

    public void Run()
    {
        _mainRenderWindow.Run();
    }

    public void Dispose()
    {
        _mainRenderWindow.Dispose();
    }
}
