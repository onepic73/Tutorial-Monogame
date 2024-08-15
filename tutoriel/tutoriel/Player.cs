using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace tutoriel
{
    internal class Player : Sprite
    {
        List<Sprite> collisionGroup;
        List<Rectangle> idleFrames;
        List<Rectangle> deadFrames;
        List<Rectangle> runningFrames;
        int currentFrame;
        double timePerFrame;
        double timeCounter;
        public bool lookingRight;

        AnimationManager idleAM;
        AnimationManager deadAM;
        AnimationManager runningAM;


        public Player(Texture2D texture, Vector2 vector2, List<Sprite> collisionGroup, float scale = 1f)
            : base(texture, vector2)
        {
            this.collisionGroup = collisionGroup;
            // Initialize the idle animation frames

            idleAM = new AnimationManager(3, 4, new Vector2(13, 19), 14, new Vector2(32, 0), new Vector2(9, 9));
            runningAM = new AnimationManager(15, 8, new Vector2(14, 18), 7, new Vector2(32, 32), new Vector2(8, 74));

            Scale = scale;
            lookingRight = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            velocity = Vector2.Zero;

            if (Keyboard.GetState().GetPressedKeys().Length == 0)
            {
                this.sourceRect = idleAM.GetFrame();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                this.sourceRect = runningAM.GetFrame();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lookingRight = true;
                velocity.X  = 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lookingRight = false;
                velocity.X = -5;
            }
            position.X += velocity.X;

            foreach (var sprite in collisionGroup)
            {
                if (sprite != this && sprite.Rect.Intersects(Rect))
                {
                    position.X -= velocity.X;
                    break;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                velocity.Y = -5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                velocity.Y = 5;
            }
            position.Y += velocity.Y;

            foreach (var sprite in collisionGroup)
            {
                if (sprite != this && sprite.Rect.Intersects(Rect))
                {
                    position.Y -= velocity.Y;
                    break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var effects = lookingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, position - CameraPos, sourceRect, Color.White, 0f, Vector2.Zero, Scale, effects, 0f);
        }
    }
}
