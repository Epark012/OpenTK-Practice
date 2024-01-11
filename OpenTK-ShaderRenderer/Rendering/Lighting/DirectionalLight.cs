using OpenTK.Mathematics;

namespace OpenTK_Renderer.Rendering.Lighting;

public class DirectionalLight : Light
{
    public Vector3 Direction;
    public Vector3 Ambient;
    public Vector3 Diffuse;
    public Vector3 Specular;

    public float Intensity;

    public DirectionalLight(Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular, float intensity = 1f)
    {
        Direction = direction;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;

        Intensity = intensity;
    }
    
    public override void Update(Shader shader)
    {
        // TODO Update for dynamic lighting
        
        // Update intensity
        shader.SetUniform("dirIntensity", Intensity);
        shader.SetUniform("dirLight.direction", Direction);
    }
}