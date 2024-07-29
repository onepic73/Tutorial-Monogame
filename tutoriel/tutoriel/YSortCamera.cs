using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tutoriel
{
    public class YSortCamera
    {
        public Vector2 Position {  get; set; }

        public YSortCamera(Vector2 position)
        {
            Position = position;
        }

        public void Follow(Rectangle target, Vector2 screenSize) 
        {
            Position = new Vector2(
                target.X - (screenSize.X /2), 
                target.Y - (screenSize.Y / 2)
            );
        }

        public void Draw(SpriteBatch spriteBatch, List<Sprite> sprites)
        {
           List<Sprite> sortedSprites = sprites.OrderBy(obj => obj.Rect.Bottom).ToList();

           foreach (Sprite sprite in sortedSprites) 
           {
                sprite.CameraPos = this.Position;
                sprite.Draw(spriteBatch);    
           }
        }
    }
}
