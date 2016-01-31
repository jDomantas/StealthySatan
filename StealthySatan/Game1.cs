using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace StealthySatan
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector CurrentCamera;

        Map gameMap;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            base.Initialize();

            //using (var file = System.IO.File.OpenRead("level1.png"))
            using (var file = System.IO.File.OpenRead("level0.png"))
            {
                Texture2D map = Texture2D.FromStream(GraphicsDevice, file);
                gameMap = new Map(map.Width, map.Height, map);
            }

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

            MediaPlayer.Volume = 0.4f;
            //MediaPlayer.Play(Resources.Audio.IntenseMusic);
            MediaPlayer.Play(Resources.Audio.CalmMusic);
            MediaPlayer.IsRepeating = true;
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputHandler.UpdateInternalState();

            gameMap.Update();

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

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
            gameMap.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: m);
            gameMap.DrawBlend(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
