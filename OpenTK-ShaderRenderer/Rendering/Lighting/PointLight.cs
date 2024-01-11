using OpenTK.Mathematics;

namespace OpenTK_Renderer.Rendering.Lighting;

public class PointLight : Light
{
    private Vector3 _position;
    private Vector3 _ambient;
    private Vector3 _diffuse;
    private Vector3 _specular;

    private float _constant;
    private float _linear;
    private float _quadratic;

    public PointLight(Vector3 position, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic)
    {
        _position = position;
        _ambient = ambient;
        _diffuse = diffuse;
        _specular = specular;
        
        _constant = constant;
        _linear = linear;
        _quadratic = quadratic;
    }

    public override void Update(Shader shader)
    {
        // Update for dynamic lighting
    }
}