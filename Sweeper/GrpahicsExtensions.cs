using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sweeper
{
    public static class GrpahicsExtensions
    {
        public static Texture2D CreateRectangeTexture(this GraphicsDevice graphics, int width, int height, int borderWidth)
        {
            var texture = new Texture2D(graphics, width, height);
            var colorData = new Color[width * height];
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if (i < borderWidth || i >= width - borderWidth || j < borderWidth || j >= height - borderWidth)
                        colorData[i + (j*width)] = Color.Black;
                    else
                        colorData[i + (j * width)] = Color.White;
                }
            }
            texture.SetData(colorData);
            return texture;
        }
    }
}
