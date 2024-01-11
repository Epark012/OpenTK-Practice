namespace OpenTK_Renderer.Rendering.Lighting;

public abstract class Light
{
    /// <summary>
    /// Update for dynamic lighting
    /// </summary>
    /// <param name="shader">Shader to update light</param>
    public abstract void Update(Shader shader);
}
