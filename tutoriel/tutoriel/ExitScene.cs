using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace tutoriel
{
    public class ExitScene : IScene
    {
        private ContentManager contentManager;

        public ExitScene(ContentManager contentManager) 
        {
            this.contentManager = contentManager;
        }

        public void Load() 
        {
        
        }
        public void Update(GameTime gameTime)
        {
        }
        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
