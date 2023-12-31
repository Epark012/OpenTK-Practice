﻿using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OpenTK_Renderer
{
    public struct TextureInfo
    {
        public uint Id;
        public string Type;
        public string Path;

        public override string ToString()
        {
            return $"Texture Info\n ID : {Id}\n Type : {Type}\n Path : {Path}";
        }
    };
    
    public class Texture
    {
        public int ID { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">path for loading texture</param>
        public Texture(string path)
        {
            // Generate handle
            ID = GL.GenTexture();
            Use();

            // Bind the handle
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ID);

            StbImage.stbi_set_flip_vertically_on_load(1);

            using (Stream stream = File.OpenRead(path))
            {
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        private void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }
    }
}
