using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace monotest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        
        private SpriteBatch _spriteBatch;
        private Texture2D background;
        private Texture2D shuttle;
        private Texture2D earth;
        private Texture2D arrow;
        private Texture2D blue;
        private Texture2D green;
        private Texture2D red;
        private SpriteFont fontScore;

        private Vector2 shuttlePos;

        private int score = 0;
        private float angle = 0;

        private float redAngle = 0;
        private float greenAngle = 0;
        private float blueAngle = 0;

        private float blueSpeed = 0.025f;
        private float greenSpeed = 0.017f;
        private float redSpeed = 0.022f;
        private float shuttleSpeed = 100f;
        
        private float distance = 100;
        
        private AnimatedSprite1 animatedSprite;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            shuttlePos = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("Images/stars");
            shuttle = Content.Load<Texture2D>("Images/shuttle");
            earth = Content.Load<Texture2D>("Images/earth");
            arrow = Content.Load<Texture2D>("Images/arrow");
            blue = Content.Load<Texture2D>("Images/blue");
            green = Content.Load<Texture2D>("Images/green");
            red = Content.Load<Texture2D>("Images/red");

            fontScore = Content.Load<SpriteFont>("Fonts/Score");
            Texture2D texture = Content.Load<Texture2D>("Images/smiley");
            animatedSprite = new AnimatedSprite1(texture, 4, 4);
        }

        protected override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            score++;
            
            angle += 0.01f;
            
            blueAngle += blueSpeed;
            redAngle += redSpeed;
            greenAngle += greenSpeed;
            
            animatedSprite.Update();
            updateShuttle(kstate, gameTime);

            base.Update(gameTime);
        }
        private void updateShuttle(KeyboardState key,GameTime gTime) 
        {
            if (key.IsKeyDown(Keys.Right))
                shuttlePos.X += shuttleSpeed * (float)gTime.ElapsedGameTime.TotalSeconds;
            if (key.IsKeyDown(Keys.Left))
                shuttlePos.X -= shuttleSpeed * (float)gTime.ElapsedGameTime.TotalSeconds;
            if (key.IsKeyDown(Keys.Up))
                shuttlePos.Y -= shuttleSpeed * (float)gTime.ElapsedGameTime.TotalSeconds;
            if (key.IsKeyDown(Keys.Down))
                shuttlePos.Y += shuttleSpeed * (float)gTime.ElapsedGameTime.TotalSeconds;

            if (shuttlePos.X > _graphics.PreferredBackBufferWidth - shuttle.Width / 2)
                shuttlePos.X = _graphics.PreferredBackBufferWidth - shuttle.Width / 2;
            else if (shuttlePos.X < shuttle.Width / 2)
                shuttlePos.X = shuttle.Width / 2;

            if (shuttlePos.Y > _graphics.PreferredBackBufferHeight - shuttle.Height / 2)
                shuttlePos.Y = _graphics.PreferredBackBufferHeight - shuttle.Height / 2;
            else if (shuttlePos.Y < shuttle.Height / 2)
                shuttlePos.Y = shuttle.Height / 2;
        }
        private void drawArrow()
        {
            Vector2 location = new Vector2(300, 240);
            Vector2 origin = new Vector2(arrow.Width/2, arrow.Height);
            Rectangle srcRect = new Rectangle(0, 0, arrow.Width, arrow.Height);

            _spriteBatch.Draw(arrow, location,srcRect, Color.White,angle,origin,1.0f,SpriteEffects.None,1);
        }
        private void drawUI()
        {
            _spriteBatch.DrawString(fontScore, "Score :" + score,new Vector2(10, 10),Color.White);
        }
        private void drawSpace()
        {
            _spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
        }
        private void drawPlanet()
        {
            _spriteBatch.Draw(earth, new Vector2(200, 140), Color.White);
        }
        private void drawShuttle()
        {
            _spriteBatch.Draw(shuttle, shuttlePos, Color.White);
        }
        private void drawSmiley()
        {
            animatedSprite.Draw(_spriteBatch, new Vector2(100, 100));
        }
        private void drawBlender()
        {
            Vector2 center = new Vector2(100, 400);
            Vector2 bluePos = new Vector2((float)Math.Cos(blueAngle)*distance, (float)Math.Sin(blueAngle) * distance);
            Vector2 greenPos = new Vector2((float)Math.Cos(greenAngle) * distance, (float)Math.Sin(greenAngle) * distance);
            Vector2 redPos = new Vector2((float)Math.Cos(redAngle) * distance, (float)Math.Sin(redAngle) * distance);

            _spriteBatch.Draw(blue, center + bluePos, Color.White);
            _spriteBatch.Draw(green, center + greenPos, Color.White);
            _spriteBatch.Draw(red, center + redPos, Color.White);
        }
        private void drawScene()
        {
            drawSpace();
            drawPlanet();
            drawShuttle();
        }
        private void drawAnimate()
        {
            drawSmiley();
            drawArrow();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.Additive);
            drawScene();
            drawUI();
            drawAnimate();
            drawBlender();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
