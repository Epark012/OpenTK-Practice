﻿using OpenTK.Graphics.OpenGL4;

namespace OpenTK_Renderer;

public class FrameBuffer
{
    public int ID { get; set; }

    private readonly int _vao;
    private readonly float[] _screenVertices = 
    {
        // positions   // texCoords
        -1.0f,  1.0f,  0.0f, 1.0f,
        -1.0f, -1.0f,  0.0f, 0.0f,
         1.0f, -1.0f,  1.0f, 0.0f,

        -1.0f,  1.0f,  0.0f, 1.0f,
         1.0f, -1.0f,  1.0f, 0.0f,
         1.0f,  1.0f,  1.0f, 1.0f
    };

    private readonly Shader _screenShader;
    private readonly int _frameBufferTexture;
    
    public FrameBuffer(int width, int height)
    {
        // Screen VaO
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        //  Screen Vbo       
        var _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _screenVertices.Length * sizeof(float), _screenVertices, BufferUsageHint.StaticDraw);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

        // Initialize screen shader
        _screenShader = new Shader("Resources/Shader/Screen.vert", "Resources/Shader/Screen.frag");
        _screenShader.Initialize();
            
        _screenShader.SetUniform("screenTexture", 0);

        // Generate frame buffer
        ID = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
        
        // Create color texture
        _frameBufferTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _frameBufferTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _frameBufferTexture, 0);

        // Generate render buffer
        var rbo = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

        switch (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer))
        {
            case FramebufferErrorCode.FramebufferComplete:
                Console.WriteLine("Frame buffer is compiled correctly");
                break;
            default:
                throw new Exception("Framebuffer is not working");
        }
            
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    /// <summary>
    /// Bind this framebuffer
    /// </summary>
    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
    }
    
    public void Process()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        _screenShader.Use();
        GL.BindVertexArray(_vao);
        GL.Disable(EnableCap.DepthTest);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _frameBufferTexture);
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

        GL.Enable(EnableCap.DepthTest);
    }
}
