using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Mesh
{
    private readonly Vertex[] _vertices;
    private readonly uint[] _indices;
    private readonly TextureInfo[] _textures;

    private int _vao;

    public Mesh(Vertex[] vertices, uint[] indices, TextureInfo[] textures)
    {
        _vertices = vertices;
        _indices = indices;
        _textures = textures;

        var message = "A mesh is created\n" +
                      $"Vertices count - [{_vertices.Length}]\n" +
                      $"Indices count - [{_indices.Length}]\n" +
                      $"Textures count - [{_textures.Length}]";

        Console.WriteLine(message);
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
        var vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Vertex.Stride, _vertices, BufferUsageHint.StaticDraw);
        
        var ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

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
        for (int i = 0; i < _textures.Length; i++)
        {
            // retrieve texture number (the N in diffuse_textureN)
            var number = "";
            var type = _textures[i].Type;
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
            GL.BindTexture(TextureTarget.Texture2D, _textures[i].Id);
        }

        // Draw mesh
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        
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