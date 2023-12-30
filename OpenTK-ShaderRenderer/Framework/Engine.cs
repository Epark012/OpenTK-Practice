namespace OpenTK_Renderer;

public class Engine : IDisposable
{
    private MainRenderWindow _mainRenderWindow;
    
    public Engine()
    {
        _mainRenderWindow = new MainRenderWindow(1440, 1080, "Main Window");
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
