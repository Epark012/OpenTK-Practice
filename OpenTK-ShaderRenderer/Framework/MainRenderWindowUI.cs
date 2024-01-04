using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using Vector2 = System.Numerics.Vector2;

namespace OpenTK_Renderer;

public partial class MainRenderWindow
{
    private readonly Vector2 _min = new (0, 1);
    private readonly Vector2 _max = new (1, 0);
    
    private void RenderImGuiLayer()
    {
        // Views
        DrawGameView();
        DrawSceneMenu();
        
        // Main menu
        DrawMainMenu();
    }

    private void DrawGameView()
    {
        ImGui.Begin("Window");

        var size = ImGui.GetContentRegionAvail();
        _gameViewFrameBuffer.Resize((int)size.X, (int)size.Y);
        GL.Viewport(0, 0, (int)ClientSize.X, (int)ClientSize.Y);
            
        var pos = ImGui.GetCursorScreenPos();
        ImGui.GetWindowDrawList().AddImage((IntPtr)_gameViewFrameBuffer.BufferTexture, 
            pos, 
            new Vector2(pos.X + size.X, pos.Y + size.Y), 
            _min, 
            _max);

        _gameViewActive = ImGui.IsWindowFocused();
            
        ImGui.End();
    }

    private void DrawSceneMenu()
    {
        ImGui.Begin("Inspector");

        ImGui.End();
    }

    private void DrawMainMenu()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Add"))
                {
                    // Add model
                    
                }
                    
                ImGui.Separator();
                if (ImGui.MenuItem("Close"))
                {
                    // Close application
                    Environment.Exit(1);
                }
                    
                ImGui.EndMenu();
            }
                
            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("#######"))
                {
                    // Add function
                }
                    
                ImGui.EndMenu();
            }
                
            if (ImGui.BeginMenu("Help"))
            {
                if (ImGui.MenuItem("Open Github"))
                {
                        
                }
                    
                ImGui.EndMenu();
            }
                
            ImGui.EndMainMenuBar();
        }
    }
}
