using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace OpenTK_Renderer.Rendering;

public class CubeMap : IDisposable
{
    private float[] _cubeVertices = {
         // positions          // normals
        -0.5f, -0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
         0.5f, -0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
         0.5f,  0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
         0.5f,  0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
        -0.5f,  0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
        -0.5f, -0.5f, -0.5f,   0.0f,  0.0f, -1.0f,
                              
        -0.5f, -0.5f,  0.5f,   0.0f,  0.0f, 1.0f,
         0.5f, -0.5f,  0.5f,   0.0f,  0.0f, 1.0f,
         0.5f,  0.5f,  0.5f,   0.0f,  0.0f, 1.0f,
         0.5f,  0.5f,  0.5f,   0.0f,  0.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,   0.0f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,   0.0f,  0.0f, 1.0f,
                              
        -0.5f,  0.5f,  0.5f,  -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,  -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,  -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,  -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,  -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,  -1.0f,  0.0f,  0.0f,
                              
         0.5f,  0.5f,  0.5f,   1.0f,  0.0f,  0.0f,
         0.5f,  0.5f, -0.5f,   1.0f,  0.0f,  0.0f,
         0.5f, -0.5f, -0.5f,   1.0f,  0.0f,  0.0f,
         0.5f, -0.5f, -0.5f,   1.0f,  0.0f,  0.0f,
         0.5f, -0.5f,  0.5f,   1.0f,  0.0f,  0.0f,
         0.5f,  0.5f,  0.5f,   1.0f,  0.0f,  0.0f,
                              
        -0.5f, -0.5f, -0.5f,   0.0f, -1.0f,  0.0f,
         0.5f, -0.5f, -0.5f,   0.0f, -1.0f,  0.0f,
         0.5f, -0.5f,  0.5f,   0.0f, -1.0f,  0.0f,
         0.5f, -0.5f,  0.5f,   0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,   0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,   0.0f, -1.0f,  0.0f,
                              
        -0.5f,  0.5f, -0.5f,   0.0f,  1.0f,  0.0f,
         0.5f,  0.5f, -0.5f,   0.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,   0.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,   0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,   0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,   0.0f,  1.0f,  0.0f
    };
    
    float[] _skyboxVertices = {
         // positions          
        -1.0f,  1.0f, -1.0f,
        -1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        -1.0f,  1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f,  1.0f
    };

    private int _cubeVao, _cubeVbo, _skyboxVao, _skyboxVbo;

    public int ID { get; set; }

    private Shader _cubeMapShader;
    private Shader _skyboxShader;

    string[] _defaultCubeMapFaces = new[]
    {
        "Resources/Image/CubeMap/right.jpg",
        "Resources/Image/CubeMap/left.jpg",
        "Resources/Image/CubeMap/top.jpg",
        "Resources/Image/CubeMap/bottom.jpg",
        "Resources/Image/CubeMap/front.jpg",
        "Resources/Image/CubeMap/back.jpg"
    };
    
    public CubeMap(string[]? paths)
    {
        paths ??= _defaultCubeMapFaces;
        
        // Initialize shaders and buffers
        Initialize();

        // Load cubemap texture
        ID = LoadCubemapTexture(paths);
    }

    private void Initialize()
    {
        // Cubemap
        {
            // Vao
            _cubeVao = GL.GenVertexArray();
            GL.BindVertexArray(_cubeVao);
        
            // Vbo
            _cubeVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _cubeVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _cubeVertices.Length * sizeof(float), _cubeVertices, BufferUsageHint.StaticDraw);

            // Position
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        
            // Normal
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        
            GL.BindVertexArray(0);
        }

        // Skybox
        {
            // Vao
            _skyboxVao = GL.GenVertexArray();
            GL.BindVertexArray(_skyboxVao);

            // Vbo
            _skyboxVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _skyboxVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _skyboxVertices.Length * sizeof(float), _skyboxVertices, BufferUsageHint.StaticDraw);

            // Position
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.BindVertexArray(0);
        }
        
        // Create shader
        _cubeMapShader = new Shader("Resources/Shader/CubeMap.vert", "Resources/Shader/CubeMap.frag");
        _skyboxShader = new Shader("Resources/Shader/Skybox.vert", "Resources/Shader/Skybox.frag");

        _cubeMapShader.Use();
        _cubeMapShader.SetUniform("skybox", 0);

        _skyboxShader.Use();
        _skyboxShader.SetUniform("skybox", 0);

    }

    private int LoadCubemapTexture(string[] paths)
    {
        var id = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, id);
        
        // cube texture is drawn on different uv layout
        StbImage.stbi_set_flip_vertically_on_load(0);

        for (var i = 0; i < paths.Length; i++)
        {
            using Stream stream = File.OpenRead(paths[i]);
            Console.WriteLine($"Creating a cube map face with - {paths[i]}");
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

        StbImage.stbi_set_flip_vertically_on_load(1);
        
        return id;
    }

    /// <summary>
    /// Use cubemap texture
    /// </summary>
    private void Use()
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, ID);
    }

    /// <summary>
    /// Render cubemap 
    /// </summary>
    public void RenderCubeMap(Matrix4 view, Matrix4 projection, Vector3 cameraPosition )
    {
        _cubeMapShader.Use();
        
        _cubeMapShader.SetUniform("model", Matrix4.Identity);
        _cubeMapShader.SetUniform("view", view);
        _cubeMapShader.SetUniform("projection", projection);
        _cubeMapShader.SetUniform("cameraPos", cameraPosition);
        
        // Cube map
        Render(_cubeVao);
    }

    /// <summary>
    /// Render skybox
    /// </summary>
    public void RenderSkybox(Matrix4 view, Matrix4 projection)
    {
        GL.DepthFunc(DepthFunction.Lequal);
        _skyboxShader.Use();
        
        // Remove transition from view matrix
        var viewMat = new Matrix4(new Matrix3(view));
        _skyboxShader.SetUniform("view", viewMat);
        _skyboxShader.SetUniform("projection", projection);
        
        // Sky box
        Render(_skyboxVao);
        GL.DepthFunc(DepthFunction.Less);
    }

    /// <summary>
    /// Render by Vao
    /// </summary>
    /// <param name="vao">Vao to render</param>
    private void Render(int vao)
    {
        GL.BindVertexArray(vao);
        Use();
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(_cubeVao);
        GL.DeleteVertexArray(_skyboxVao);
        GL.DeleteBuffer(_cubeVbo);
        GL.DeleteBuffer(_skyboxVbo);
    }
}
