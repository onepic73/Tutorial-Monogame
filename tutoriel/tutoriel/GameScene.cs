using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace tutoriel
{
    public class GameScene : IScene
    {
        private ContentManager contentManager;
        private SceneManager sceneManager;
        private GraphicsDeviceManager graphics;

        private const int TILESIZE = 64;
        private Texture2D rectangleTexture;

        private YSortCamera camera;

        List<Sprite> sprites;
        Player player;
        Texture2D playerSpriteSh;

        private Texture2D textureWTile;
        private Texture2D textureCollisions;
        Dictionary<Vector2, int> mg;
        Dictionary<Vector2, int> fg;
        Dictionary<Vector2, int> bg;
        Dictionary<Vector2, int> collisions;

        private List<Rectangle> intersections;

        private SpriteFont font;

        KeyboardState prevKBState;

        Song song;
        SoundEffect soundEffect;

        public GameScene(ContentManager content, SceneManager sceneManager, GraphicsDeviceManager graphics) 
        { 
            contentManager = content;
            this.sceneManager = sceneManager;
            this.graphics = graphics;
            camera = new(Vector2.Zero);
            fg = LoadMap("../../../Data/level1_FG.csv");
            mg = LoadMap("../../../Data/level1_MG.csv");
            bg = LoadMap("../../../Data/level1_BG.csv");
            collisions = LoadMap("../../../Data/level1_collisons.csv");
            intersections = new();
        }
        private Dictionary<Vector2, int> LoadMap(string filePath)
        {
            Dictionary<Vector2, int> result = new();

            StreamReader reader = new(filePath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > -1)
                        {
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }
                y++;
            }

            return result;
        }
        public void Load() 
        {
            rectangleTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

            playerSpriteSh = contentManager.Load<Texture2D>("knight");
            Texture2D enemyTexture = contentManager.Load<Texture2D>("enemy");

            textureWTile = contentManager.Load<Texture2D>("world_tileset");
            textureCollisions = contentManager.Load<Texture2D>("collisions_Tileset");

            float scale = 4f;

            sprites = new List<Sprite>
            {
                new Sprite(enemyTexture, new Vector2(10, 50), scale),
                new Sprite(enemyTexture, new Vector2(500, 200), scale),
                new Sprite(enemyTexture, new Vector2(700, 300), scale)
            };

            player = new Player(playerSpriteSh, new Vector2(200, 200), sprites, scale);
            sprites.Add(player);

            font = contentManager.Load<SpriteFont>("Fonts/PixelOp");

            song = contentManager.Load<Song>("Song/time_for_adventure");

            soundEffect = contentManager.Load<SoundEffect>("Audio/jump");

        }
        public void Update(GameTime gameTime) 
        {
            intersections = getIntersectTilesX(player.Rect);

            foreach (var tile in intersections) 
            {
                if (collisions.TryGetValue(new Vector2(tile.X, tile.Y), out int _val))
                {
                    Rectangle collision = new Rectangle(
                        tile.X * TILESIZE,
                        tile.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    );

                    if (player.velocity.X > 0.0f)
                    {
                        player.position.X = collision.Left - player.Rect.Width;
                    }
                    else if (player.velocity.X < 0.0f)
                    {
                        player.position.X = collision.Right;
                    }
                }
            }

            intersections = getIntersectTilesY(player.Rect);

            foreach (var tile in intersections)
            {
                if (collisions.TryGetValue(new Vector2(tile.X, tile.Y), out int _val))
                {
                    Rectangle collision = new Rectangle(
                        tile.X * TILESIZE,
                        tile.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    );

                    if (player.velocity.Y > 0.0f)
                    {
                        player.position.Y = collision.Top - player.Rect.Height;
                    }
                    else if (player.velocity.Y < 0.0f)
                    {
                        player.position.Y = collision.Bottom;
                    }
                }
            }

            camera.Follow(player.Rect, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                sceneManager.AddScene(new ExitScene(contentManager));
            }

            KeyboardState currentKBState = Keyboard.GetState();
            if (currentKBState.IsKeyDown(Keys.M) && !prevKBState.IsKeyDown(Keys.M))
            {
                soundEffect.Play();
            }

            prevKBState = Keyboard.GetState();

            foreach (var sprite in sprites)
            {
                sprite.Update(gameTime);
            }
        }

        public List<Rectangle> getIntersectTilesX(Rectangle target)
        {
            List<Rectangle> intersections = new();

            int widthTiles = (target.Width - (target.Width % TILESIZE)) / TILESIZE;
            int heightTiles = (target.Height- (target.Height% TILESIZE)) / TILESIZE;

            for (int x = 0; x <= widthTiles; x++)
            {
                for (int y = 0; y <= heightTiles; y++)
                {
                    if (player.lookingRight)
                    {
                        intersections.Add(new Rectangle(
                        ((target.X + target.Width) + x * TILESIZE) / TILESIZE,
                        (target.Y + y * (TILESIZE - 1)) / TILESIZE,
                        TILESIZE,
                        TILESIZE
                        ));
                    }
                    else
                    {
                        intersections.Add(new Rectangle(
                        (target.X + x * TILESIZE) / TILESIZE,
                        (target.Y + y * (TILESIZE - 1)) / TILESIZE,
                        TILESIZE,
                        TILESIZE
                        ));
                    }
                }
            }

            return intersections;
        }

        public List<Rectangle> getIntersectTilesY(Rectangle target)
        {
            List<Rectangle> intersections = new();

            int widthTiles = (target.Width - (target.Width % TILESIZE)) / TILESIZE;
            int heightTiles = (target.Height - (target.Height % TILESIZE)) / TILESIZE;

            for (int x = 0; x <= widthTiles; x++)
            {
                for (int y = 0; y <= heightTiles; y++)
                {
                    intersections.Add(new Rectangle(
                        (target.X + x * (TILESIZE-1)) / TILESIZE,
                        (target.Y + y * TILESIZE) / TILESIZE,
                        TILESIZE,
                        TILESIZE
                    ));
                }
            }

            return intersections;
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            int disp_tilesize = 64;
            int num_tiles_row = 16;
            int pixel_tilesize = 16;

            foreach (var item in bg)
            {
                Rectangle drect = new(
                    (int)item.Key.X * disp_tilesize - (int)player.CameraPos.X,
                    (int)item.Key.Y * disp_tilesize - (int)player.CameraPos.Y,
                    disp_tilesize,
                    disp_tilesize
                );

                int x = item.Value % num_tiles_row;
                int y = item.Value / num_tiles_row;

                Rectangle src = new(
                    x * pixel_tilesize,
                    y * pixel_tilesize,
                    pixel_tilesize,
                    pixel_tilesize
                );

                spriteBatch.Draw(textureWTile, drect, src, Color.White);
            }

            foreach (var item in mg)
            {
                Rectangle drect = new(
                    (int)item.Key.X * disp_tilesize - (int)player.CameraPos.X,
                    (int)item.Key.Y * disp_tilesize - (int)player.CameraPos.Y,
                    disp_tilesize,
                    disp_tilesize
                );

                int x = item.Value % num_tiles_row;
                int y = item.Value / num_tiles_row;

                Rectangle src = new(
                    x * pixel_tilesize,
                    y * pixel_tilesize,
                    pixel_tilesize,
                    pixel_tilesize
                );

                spriteBatch.Draw(textureWTile, drect, src, Color.White);
            }

            foreach (var item in fg)
            {
                Rectangle drect = new(
                    (int)item.Key.X * disp_tilesize - (int)player.CameraPos.X,
                    (int)item.Key.Y * disp_tilesize - (int)player.CameraPos.Y,
                    disp_tilesize,
                    disp_tilesize
                );

                int x = item.Value % num_tiles_row;
                int y = item.Value / num_tiles_row;

                Rectangle src = new(
                    x * pixel_tilesize,
                    y * pixel_tilesize,
                    pixel_tilesize,
                    pixel_tilesize
                );

                spriteBatch.Draw(textureWTile, drect, src, Color.White);
            }

            foreach (var item in collisions)
            {
                Rectangle drect = new(
                    (int)item.Key.X * disp_tilesize - (int)player.CameraPos.X,
                    (int)item.Key.Y * disp_tilesize - (int)player.CameraPos.Y,
                    disp_tilesize,
                    disp_tilesize
                );

                int x = item.Value % num_tiles_row;
                int y = item.Value / num_tiles_row;

                Rectangle src = new(
                    x * pixel_tilesize,
                    y * pixel_tilesize,
                    pixel_tilesize,
                    pixel_tilesize
                );

                spriteBatch.Draw(textureCollisions, drect, src, Color.White);
            }

            foreach (var item in intersections) 
            {
                DrawRectHollow(
                    spriteBatch,
                    new Rectangle(
                        item.X * TILESIZE,
                        item.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE
                    ),
                    4
                );   
            }


            camera.Draw(spriteBatch, sprites);
        }

        public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
        {
            rect.X -= (int)player.CameraPos.X;
            rect.Y -= (int)player.CameraPos.Y;
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.Right - thickness,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
        }
    }
}
