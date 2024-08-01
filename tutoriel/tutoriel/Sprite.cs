using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static System.Formats.Asn1.AsnWriter;

namespace tutoriel
{
    public class Sprite
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle sourceRect;
        public Vector2 velocity;

        public Vector2 CameraPos { get; set; }
        public float Scale { get; set; }

        public Rectangle Rect
        {
            get
            {
                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)(sourceRect.Width * Scale),
                    (int)(sourceRect.Height * Scale)
                );
            }

            set
            { 
                position.X = value.X;
                position.Y = value.Y;
            }
        }

        public Sprite(Texture2D texture, Vector2 position, float scale = 1f)
        {
            this.texture = texture;
            this.position = position;
            this.sourceRect = new Rectangle(0, 0, texture.Width, texture.Height); // Default to full texture
            this.Scale = scale;
            velocity = new();
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position - CameraPos, sourceRect, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}
