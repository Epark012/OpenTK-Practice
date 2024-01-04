using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTK_Renderer;

/// <summary>
/// Partial class for interaction with render window
/// </summary>
public partial class MainRenderWindow
{
    // Camera
    private bool _firstMove = true;
    private Vector2 _lastPos;

    // Game view
    private bool _gameViewActive;
    
    /// <summary>
    /// Process Mouse Input
    /// </summary>
    private void ProcessMouseInput()
    {
        var mouse = MouseState;

        if (_firstMove) // This bool variable is initially set to true.
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
            return;
        }

        // Calculate the offset of the mouse position
        var deltaX = mouse.X - _lastPos.X;
        var deltaY = mouse.Y - _lastPos.Y;
        _lastPos = new Vector2(mouse.X, mouse.Y);

        // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
        const float sensitivity = 0.2f;

        _camera.Yaw += deltaX * sensitivity;
        _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
    }
    
    /// <summary>
    /// Process Keyboard Input
    /// </summary>
    private void ProcessKeyboardInput(FrameEventArgs e)
    {
        if (!KeyboardState.IsAnyKeyDown)
        {
            return;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        var input = KeyboardState;

        const float cameraSpeed = 1.5f;

        if (input.IsKeyDown(Keys.W))
        {
            _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
        }

        if (input.IsKeyDown(Keys.S))
        {
            _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
        }
        if (input.IsKeyDown(Keys.A))
        {
            _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
        }
        if (input.IsKeyDown(Keys.D))
        {
            _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
        }
        if (input.IsKeyDown(Keys.Space))
        {
            _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
        }
        if (input.IsKeyDown(Keys.LeftShift))
        {
            _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
        }
    }

    /// <summary>
    /// Process input 
    /// </summary>
    /// <param name="args">Frame event args</param>
    private void ProcessInput(FrameEventArgs args)
    {
        if (!_gameViewActive)
        {
            return;
        }
        
        // Process input
        ProcessKeyboardInput(args);

        // Get the mouse state
        ProcessMouseInput();
    }
}
