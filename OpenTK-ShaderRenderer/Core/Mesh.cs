using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Mesh
{
    public Vertex[] Vertices;
    public uint[] Indices;
    public TextureInfo[] Textures;

    private int _vao, _vbo, _ebo;

    public Mesh(Vertex[] vertices, uint[] indices, TextureInfo[] textures)
    {
        Vertices = vertices;
        Indices = indices;
        Textures = textures;

        var message = "A mesh is created\n" +
                      $"Vertices count - [{Vertices.Length}]\n" +
                      $"Indices count - [{Indices.Length}]\n" +
                      $"Textures count - [{Textures.Length}]";

        Console.WriteLine(message);

        foreach (var textureInfo in textures)
        {
            Console.WriteLine(textureInfo.Type);
        }
        
        UpdateBuffer();
    }

    /// <summary>
    /// Update buffer
    /// </summary>
    private void UpdateBuffer()
    {
        // Vao
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        // Vbo
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Vertex.Stride, Vertices, BufferUsageHint.StaticDraw);
        
        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 3 * sizeof(float));

        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Stride, 6 * sizeof(float));
        
        GL.EnableVertexAttribArray(3);
        GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 8 * sizeof(float));
        
        GL.EnableVertexAttribArray(4);
        GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 11 * sizeof(float));

        // Clear vertex array
        GL.BindVertexArray(0);
    }

    /// <summary>
    /// Draw this mesh with shader
    /// </summary>
    /// <param name="shader">Shader to use for this mesh</param>
    public void Draw(Shader shader)
    {
        shader.Use();
        GL.BindVertexArray(_vao);
        
        uint diffuseNr = 1;
        uint specularNr = 1;
        uint normalNr = 1;
        uint heightNr = 1;
        for (int i = 0; i < Textures.Length; i++)
        {
            // retrieve texture number (the N in diffuse_textureN)
            var number = "";
            var type = Textures[i].Type;
            number = type switch
            {
                "texture_diffuse" => diffuseNr++.ToString(),
                "texture_specular" => specularNr++.ToString(),
                "texture_normal" => normalNr++.ToString(),
                "texture_height" => heightNr++.ToString(),
                _ => number
            };

            // now set the sampler to the correct texture unit
            shader.SetUniform(type + number, i);

            GL.ActiveTexture(TextureUnit.Texture0 + i);
            GL.BindTexture(TextureTarget.Texture2D, Textures[i].Id);
        }

        // Draw mesh
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        
        // Clear binding texture to default
        GL.BindVertexArray(0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
}

public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoord;
    public Vector3 Tangent;
    public Vector3 Bitangent;

    public static int Stride => Vector3.SizeInBytes * 4 + Vector2.SizeInBytes;
}