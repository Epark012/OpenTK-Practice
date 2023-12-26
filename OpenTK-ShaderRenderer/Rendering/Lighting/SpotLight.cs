using OpenTK.Mathematics;

namespace OpenTK_Renderer.Rendering.Lighting;

public class SpotLight : Light
{
    public Vector3 Ambient;
    public Vector3 Diffuse;
    public Vector3 Specular;

    public float Constant;
    public float Linear;
    public float Quadratic;

    public float CutOff;
    public float OuterCutOff;

    public SpotLight(Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic, float cutOff, float outerCutOff)
    {
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        
        Constant = constant;
        Linear = linear;
        Quadratic = quadratic;
        
        CutOff = cutOff;
        OuterCutOff = outerCutOff;
    }

    public override void Update()
    {
        // Update for dynamic lighting
    }
}
