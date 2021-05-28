using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace monotest
{
    public class AnimatedSprite1
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        public AnimatedSprite1 (Texture2D texture,int rows,int cols)
        {
            Texture = texture;
            Rows = rows;
            Columns = cols;
            currentFrame = 0;
            totalFrames = cols * rows;
        }
        public void Update()
        {
            currentFrame++;
            if (currentFrame == totalFrames)
            {
                currentFrame = 0;
            }
        }
        public void Draw(SpriteBatch spriteBatch,Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle srcRect = new Rectangle(width * column, height * row, width, height);
            Rectangle destRect = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, destRect, srcRect, Color.White);

        }
    }
}