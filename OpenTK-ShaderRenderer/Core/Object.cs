using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Object
{
    public Model Model;
    public Shader Shader;

    public Object(Model model, Shader shader, Action<Object>? onInitialized = null)
    {
        Model = model;
        Shader = shader;

        onInitialized?.Invoke(this);
    }

    public void Initialize(Vector3[] pointLights)
    {
            Shader.SetUniform("material.diffuse", 0);
            Shader.SetUniform("material.specular", 1);
            Shader.SetUniform("material.shininess", 32.0f);
                
            // Directional light
            Shader.SetUniform("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            Shader.SetUniform("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            Shader.SetUniform("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
            Shader.SetUniform("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));
            
            // Point lights
            for (var i = 0; i < pointLights.Length; i++)
            {
                Shader.SetUniform($"pointLights[{i}].position", pointLights[i]);
                Shader.SetUniform($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                Shader.SetUniform($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                Shader.SetUniform($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                Shader.SetUniform($"pointLights[{i}].constant", 1.0f);
                Shader.SetUniform($"pointLights[{i}].linear", 0.09f);
                Shader.SetUniform($"pointLights[{i}].quadratic", 0.032f);
            }
            
            // Spot light
            Shader.SetUniform("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
            Shader.SetUniform("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            Shader.SetUniform("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
            Shader.SetUniform("spotLight.constant", 1.0f);
            Shader.SetUniform("spotLight.linear", 0.09f);
            Shader.SetUniform("spotLight.quadratic", 0.032f);
            Shader.SetUniform("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            Shader.SetUniform("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
    }

    public void Render(Camera camera, Matrix4 view, Matrix4 projection)
    {
        // TODO Set runtime lighting
        Shader.SetUniform("spotLight.position", camera.Position);
        Shader.SetUniform("spotLight.direction", camera.Front);
        
        // Render model
        Shader.Use();
        Shader.SetUniform<Matrix4>("view", view);
        Shader.SetUniform<Matrix4>("projection", projection);

        Shader.SetUniform("viewPos", camera);
        
        Model.Draw(Shader);
    }
}
