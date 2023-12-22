using OpenTK_Renderer;

Console.WriteLine("Running Test Project");
using (var window = new MainRenderWindow(1440, 1080, "Main Window"))
{
    window.Run();
}