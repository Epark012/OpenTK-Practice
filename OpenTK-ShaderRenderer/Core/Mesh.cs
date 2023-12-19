using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Mesh
{
    public Vertex[] Vertices;
    public uint[] Indices;
    public Texture[] Textures;

    private int _vao, _vbo, _ebo;
    private Texture[] _defaultTextures;

    public Mesh(Vertex[] vertices, uint[] indices, Texture[] textures)
    {
        _defaultTextures = new[]
        {
            Texture.LoadFromFile("Resources/Image/container.png"),
            Texture.LoadFromFile("Resources/Image/container_specular.png")
        };
        
        Vertices = vertices;
        Indices = indices;

        if (textures.Length == 0)
        {
            Textures = _defaultTextures;
        }
        else
        {
            textures = textures;
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
        
        // TODO: Need to know why this is not working
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
        
        // Apply texture
        var baseUnit = TextureUnit.Texture0;
        for (var i = 0; i < Textures.Length; i++)
        {
            var texture = Textures[i];
            texture.Use(baseUnit + i);
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

    public static int Stride => Vector3.SizeInBytes * 2 + Vector2.SizeInBytes;
}
