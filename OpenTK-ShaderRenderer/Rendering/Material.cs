namespace OpenTK_Renderer.Rendering;

public struct Material
{
    public float Diffuse;
    public float Specular;
    public float Shininess;

    public Material(float diffuse, float specular, float shininess)
    {
        Diffuse = diffuse;
        Specular = specular;
        Shininess = shininess;
    }
}