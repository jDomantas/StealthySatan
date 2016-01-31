using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StealthySatan.Entities;
using System;
using System.IO;

namespace StealthySatan
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector CurrentCamera;

        Map gameMap;

        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;

        int CurrentLevel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();

            IsMouseVisible = true;
        }
        
        private void LoadLevel(int index)
        {
            using (var file = File.OpenRead($"levels/level{index}.png"))
            {
                Texture2D map = Texture2D.FromStream(GraphicsDevice, file);
                gameMap = new Map(map.Width, map.Height, map);
            }

            string[] objects = File.ReadAllLines($"levels/level{index}.txt");
            foreach (var line in objects)
            {
                string[] p = line.Split(' ');
                if (line.StartsWith("Policeman"))
                    gameMap.AddEntity(new Policeman(gameMap, new Vector(int.Parse(p[1]), int.Parse(p[2])), bool.Parse(p[3]), bool.Parse(p[4]), bool.Parse(p[5])));
                else if (line.StartsWith("Patrol"))
                    gameMap.AddEntity(new Policeman(gameMap, new Vector(int.Parse(p[1]), int.Parse(p[2])), int.Parse(p[3]), int.Parse(p[4]), bool.Parse(p[5]), false, false));
                else if (line.StartsWith("Civilian"))
                    gameMap.AddEntity(new Civilian(gameMap, new Vector(int.Parse(p[1]), int.Parse(p[2])), bool.Parse(p[3])));
                else if (line.StartsWith("Staircase"))
                    gameMap.AddPairOfStairaces(new Vector(int.Parse(p[1]), int.Parse(p[2])), new Vector(int.Parse(p[3]), int.Parse(p[4])));
                else if (line.StartsWith("Spawn"))
                    gameMap.Spawn = new Vector(int.Parse(p[1]), int.Parse(p[2]));
                else if (line.StartsWith("Text"))
                    gameMap.PlayText = true;
            }

            gameMap.PlayerEntity.Respawn();

            MediaPlayer.Stop();
            MediaPlayer.Volume = 0.4f;
            if (gameMap.PlayText)
                MediaPlayer.Play(Resources.Audio.CalmMusic);
            else
                MediaPlayer.Play(Resources.Audio.IntenseMusic);

            MediaPlayer.IsRepeating = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            CurrentLevel = 0;
            //using (var file = System.IO.File.OpenRead("level1.png"))
            LoadLevel(CurrentLevel);

            CurrentCamera = Vector.Zero;
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // init pixel to be 1x1 white texture
            Resources.Graphics.Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Resources.Graphics.Pixel.SetData(new Color[] { Color.White });

            // init triangle to have filled bottom left half
            const int triangleSize = 1024;
            Resources.Graphics.Triangle = new Texture2D(GraphicsDevice, triangleSize, triangleSize);
            Color[] triangleData = new Color[triangleSize * triangleSize];
            for (int x = 0; x < triangleSize; x++)
                for (int y = 0; y < triangleSize; y++)
                    triangleData[x + y * triangleSize] = x <= y ? Color.White : new Color(0, 0, 0, 0);
            Resources.Graphics.Triangle.SetData(triangleData);

            // init fading triangle
            Resources.Graphics.FadingTriangle = new Texture2D(GraphicsDevice, triangleSize, triangleSize);
            for (int x = 0; x < triangleSize; x++)
                for (int y = 0; y < triangleSize; y++)
                {
                    int val = 255 - 100 * y / triangleSize;
                    triangleData[x + y * triangleSize] = x <= y ? new Color(val, val, val, val) : new Color(0, 0, 0, 0);
                }
            Resources.Graphics.FadingTriangle.SetData(triangleData);

            // init fading rectangle
            Resources.Graphics.FadingRectangle = new Texture2D(GraphicsDevice, 1, triangleSize);
            Color[] rectangleData = new Color[triangleSize];
            for (int i = 0; i < triangleSize; i++)
            {
                int val = 255 - 100 * i / triangleSize;
                triangleData[i] = new Color(val, val, val, val);
            }
            Resources.Graphics.FadingRectangle.SetData(triangleData);

            // load all other content
            Resources.Load(Content);

            
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputHandler.UpdateInternalState();

            gameMap?.Update();
            if (gameMap?.Won == true)
            {
                CurrentLevel++;
                if (CurrentLevel < 5)
                    LoadLevel(CurrentLevel);
                else
                    gameMap = null;
            }

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(56, 40, 41));

            if (gameMap != null)
            {
                double centerX = (gameMap.PlayerEntity.Position.X + gameMap.PlayerEntity.Width / 2) * Map.ViewScale;
                double centerY = (gameMap.PlayerEntity.Position.Y + gameMap.PlayerEntity.Height / 2) * Map.ViewScale;

                if (gameMap.PlayerEntity.Facing == Entities.Entity.Direction.Left)
                    centerX -= 4 * Map.ViewScale;
                else
                    centerX += 4 * Map.ViewScale;

                CurrentCamera = new Vector(centerX, centerY) * 0.1 + CurrentCamera * 0.9;


                Matrix m = Matrix.CreateTranslation(new Vector3(
                    -(float)CurrentCamera.X + graphics.PreferredBackBufferWidth / 2,
                    -(float)CurrentCamera.Y + graphics.PreferredBackBufferHeight / 2,
                    0));

                spriteBatch.Begin(transformMatrix: m);
                gameMap.Draw(spriteBatch, CurrentCamera);
                spriteBatch.End();

                spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: m);
                gameMap.DrawBlend(spriteBatch);
                spriteBatch.End();

                if (gameMap.NextText < 4 && gameMap.CurrentTextFrame > -1 && Resources.Graphics.Monologue[gameMap.NextText][gameMap.CurrentTextFrame] != null)
                {
                    int w = Resources.Graphics.Monologue[gameMap.NextText][gameMap.CurrentTextFrame].Width / 2;
                    int h = Resources.Graphics.Monologue[gameMap.NextText][gameMap.CurrentTextFrame].Height / 2;

                    spriteBatch.Begin();

                    spriteBatch.Draw(Resources.Graphics.Monologue[gameMap.NextText][gameMap.CurrentTextFrame],
                        new Rectangle(ScreenWidth / 2 - w / 2, ScreenHeight / 2 - h / 2, w, h), Color.White);
                    spriteBatch.End();
                }
            }


            base.Draw(gameTime);
        }
    }
}
