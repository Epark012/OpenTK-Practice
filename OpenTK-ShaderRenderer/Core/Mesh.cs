using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Mesh
{
    public List<Vertex> Vertices;
    public List<uint> Indices;
    public List<Texture> Textures;

    private int _vao, _vbo, _ebo;
    private int _indiceCount;
    
    public Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures)
    {
        Vertices = vertices;
        Indices = indices;
        Textures = textures;

        UpdateBuffer();
    }

            float[] _vertices = {
           // Position            Normal                  Texture coordinates  
           // Up
           0.5f,  0.5f, 0.5f,     0.0f, 0.0f, 1.0f,       1.0f, 1.0f,
           0.5f, -0.5f, 0.5f,     0.0f, 0.0f, 1.0f,       1.0f, 0.0f,
          -0.5f, -0.5f, 0.5f,     0.0f, 0.0f, 1.0f,       0.0f, 0.0f,
          -0.5f,  0.5f, 0.5f,     0.0f, 0.0f, 1.0f,       0.0f, 1.0f,
                        
            // Down                 
           0.5f,  0.5f, -0.5f,    0.0f, 0.0f, -1.0f,      1.0f, 1.0f,
           0.5f, -0.5f, -0.5f,    0.0f, 0.0f, -1.0f,      1.0f, 0.0f,
          -0.5f, -0.5f, -0.5f,    0.0f, 0.0f, -1.0f,      0.0f, 0.0f,
          -0.5f,  0.5f, -0.5f,    0.0f, 0.0f, -1.0f,      0.0f, 1.0f,
                        
            // Foward               
           0.5f, -0.5f, 0.5f,     0.0f, -1.0f, 0.0f,      1.0f, 1.0f,
           0.5f, -0.5f, -0.5f,    0.0f, -1.0f, 0.0f,      1.0f, 0.0f,
          -0.5f, -0.5f, -0.5f,    0.0f, -1.0f, 0.0f,      0.0f, 0.0f,
          -0.5f, -0.5f,  0.5f,    0.0f, -1.0f, 0.0f,      0.0f, 1.0f,
                        
            // Back                 
           0.5f, 0.5f, 0.5f,      0.0f, 1.0f, 0.0f,       1.0f, 1.0f,
           0.5f, 0.5f, -0.5f,     0.0f, 1.0f, 0.0f,       1.0f, 0.0f,
          -0.5f, 0.5f, -0.5f,     0.0f, 1.0f, 0.0f,       0.0f, 0.0f,
          -0.5f, 0.5f,  0.5f,     0.0f, 1.0f, 0.0f,       0.0f, 1.0f,
                        
            // Right                
           0.5f,  0.5f, 0.5f,      1.0f, 0.0f, 0.0f,       1.0f, 1.0f,
           0.5f,  0.5f, -0.5f,     1.0f, 0.0f, 0.0f,       1.0f, 0.0f,
           0.5f, -0.5f, -0.5f,     1.0f, 0.0f, 0.0f,       0.0f, 0.0f,
           0.5f, -0.5f,  0.5f,     1.0f, 0.0f, 0.0f,       0.0f, 1.0f,
                        
            // Left                 
          -0.5f,  0.5f, 0.5f,     -1.0f, 0.0f, 0.0f,      1.0f, 1.0f,
          -0.5f,  0.5f, -0.5f,    -1.0f, 0.0f, 0.0f,      1.0f, 0.0f,
          -0.5f, -0.5f, -0.5f,    -1.0f, 0.0f, 0.0f,      0.0f, 0.0f,
          -0.5f, -0.5f,  0.5f,    -1.0f, 0.0f, 0.0f,      0.0f, 1.0f
        };
    
    /// <summary>
    /// Update buffer
    /// </summary>
    private void UpdateBuffer()
    {
        _indiceCount = Indices.Count;
        
        // Vao
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        // Vbo
        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        
        // TODO: Need to know why this is not working
        // GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Count * Vertex.Stride, Vertices.ToArray(), BufferUsageHint.StaticDraw);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Count * Vertex.Stride, _vertices, BufferUsageHint.StaticDraw);
        
        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indiceCount * sizeof(uint), Indices.ToArray(), BufferUsageHint.StaticDraw);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Stride, 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);
        
        // Clear vertex array
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    /// <summary>
    /// Draw this mesh with shader
    /// </summary>
    /// <param name="shader">Shader to use for this mesh</param>
    public void Draw(Shader shader)
    {
        GL.BindVertexArray(_vao);
        
        // Apply texture
        var baseUnit = TextureUnit.Texture0;
        for (var i = 0; i < Textures.Count; i++)
        {
            var texture = Textures[i];
            texture.Use(baseUnit + i);
        }

        // Draw mesh
        GL.DrawElements(PrimitiveType.Triangles, _indiceCount, DrawElementsType.UnsignedInt, 0);
        
        // Clear binding texture to default
        GL.BindVertexArray(0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
}

public struct Vertex
{
    public Vector3? Position;
    public Vector3? Normal;
    public Vector2? TexCoord;

    public Vertex(Vector3 pos, Vector3 normal, Vector2 tex)
    {
        Position = pos;
        Normal = normal;
        TexCoord = tex;
    }

    public static int Stride => Vector3.SizeInBytes * 2 + Vector2.SizeInBytes;
}
