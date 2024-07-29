using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tutoriel
{
    internal class AnimationManager
    {
        int numFrames;
        int numColumns;
        Vector2 size;
        Vector2 pos;
        Vector2 distanceFrame;

        int currentFrame;
        double timePerFrame;
        double timeCounter;

        int rowPos;
        int colPos;

        public AnimationManager(int numFrames, int numColumns, Vector2 size, double timePerFrame, Vector2 distanceFrame, Vector2 pos) 
        {
            this.numFrames = numFrames;
            this.numColumns = numColumns;
            this.size = size;
            this.timePerFrame = timePerFrame;
            this.pos = pos;
            this.distanceFrame = distanceFrame;


            currentFrame = 0;
            timeCounter = 0;

            rowPos = 1; 
            colPos = 0;
        }

        public void Update()
        {
            timeCounter++;
            if (timeCounter > timePerFrame) 
            {
                timeCounter = 0;
                NextFrame();
            }
        }

        private void NextFrame()
        {
            currentFrame++;
            colPos++;
            if (currentFrame > numFrames)
            {
                ResetAnimation();
            }

            if (colPos > numColumns) 
            {
                colPos = 0;
                rowPos++;
            }

        }

        private void ResetAnimation()
        {
            currentFrame = 0;
            rowPos = 0;
            colPos = 0;
        }

        public Rectangle GetFrame()
        {
            Update();
            int posY = colPos > numColumns ? (int)pos.Y + (int)distanceFrame.Y : (int)pos.Y;
            if (numFrames > numColumns)
            {
                int divider = numFrames / numColumns;
                currentFrame = currentFrame > numFrames / 2 ? currentFrame / 2 : currentFrame; 
            }
            return new Rectangle(
                (int)pos.X + (currentFrame * (int)distanceFrame.X),
                posY, 
                (int)size.X, (int)size.Y
            );
        }
    }
}
