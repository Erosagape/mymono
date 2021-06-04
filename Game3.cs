using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace monotest
{
    class Game3:Game
    {
        enum TankAIState
        {
            Chasing,
            Caught,
            Wander
        }
        enum MouseAIState
        {
            Evading,
            Wander
        }
        const float MaxCatSpeed = 7.5f;
        const float MaxTankSpeed = 5.0f;
        const float MaxMouseSpeed = 8.5f;
        const float TankTurnSpeed = 0.10f;
        const float MouseTurnSpeed = 0.20f;
        const float TankChaseDistance = 250.0f;
        const float TankCaughtDistance = 60.0f;
        const float TankMysteresis = 15.0f;
        const float MouseEvadeDistance = 200.0f;
        const float MouseMysteresis = 60.0f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        
        Texture2D tankTexture;
        Vector2 tankTextureCenter;
        Vector2 tankPosition;
        TankAIState tankState = TankAIState.Wander;
        float tankOrientation;
        Vector2 tankWanderDirection;

        Texture2D catTexture;
        Vector2 catTextureCenter;
        Vector2 catPosition;

        Texture2D mouseTexture;
        Vector2 mouseTextureCenter;
        Vector2 mousePosition;
        MouseAIState mouseState = MouseAIState.Wander;
        float mouseOrientation;
        Vector2 mouseWanderDirection;

        Random random = new Random();

        public Game3()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 320;
            graphics.PreferredBackBufferHeight = 480;
        }
        protected override void Initialize()
        {
            base.Initialize();
            Viewport vp = graphics.GraphicsDevice.Viewport;
            tankPosition = new Vector2(vp.Width / 4, vp.Height / 2);
            catPosition = new Vector2(vp.Width / 2, vp.Height / 2);
            mousePosition = new Vector2(3 - vp.Width / 4, vp.Height / 2);
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Fonts/Score");
            tankTexture = Content.Load<Texture2D>("tank");
            catTexture = Content.Load<Texture2D>("cat");
            mouseTexture = Content.Load<Texture2D>("mouse");

            tankTextureCenter = new Vector2(tankTexture.Width / 2, tankTexture.Height / 2);
            catTextureCenter = new Vector2(catTexture.Width / 2, catTexture.Height / 2); 
            mouseTextureCenter = new Vector2(mouseTexture.Width / 2, mouseTexture.Height / 2);
        }
        private Vector2 ClampToViewport(Vector2 vector)
        {
            Viewport vp = graphics.GraphicsDevice.Viewport;
            vector.X = MathHelper.Clamp(vector.X, vp.X, vp.X + vp.Width);
            vector.Y = MathHelper.Clamp(vector.Y, vp.Y, vp.Y + vp.Height);
            return vector;
        }
        private void UpdateMouse()
        {
            float distanceFromCat = Vector2.Distance(mousePosition, catPosition);
            if (distanceFromCat > MouseEvadeDistance + MouseMysteresis)
            {
                mouseState = MouseAIState.Wander;
            } else if (distanceFromCat< MouseEvadeDistance - MouseMysteresis)
            {
                mouseState = MouseAIState.Evading;
            }
            float currentMouseSpeed;
            if (mouseState == MouseAIState.Evading)
            {
                Vector2 seekPosition = 2 * mousePosition - catPosition;
                mouseOrientation = TurnToFace(mousePosition, seekPosition, mouseOrientation, MouseTurnSpeed);
                currentMouseSpeed = MaxMouseSpeed;
            } else
            {
                Wander(mousePosition, ref mouseWanderDirection, ref mouseOrientation, MouseTurnSpeed);
                currentMouseSpeed = .25f * MaxMouseSpeed;
            }
            Vector2 heading = new Vector2(
                (float)Math.Cos(mouseOrientation),(float)Math.Sin(mouseOrientation)
                );
            mousePosition += heading * currentMouseSpeed;
        }
        private void UpdateTank()
        {
            float tankChaseThreshold = TankChaseDistance;
            float tankCaughtThreshold = TankCaughtDistance;
            if(tankState==TankAIState.Wander)
            {
                tankChaseThreshold -= TankMysteresis / 2;
            } else if (tankState ==TankAIState.Chasing)
            {
                tankChaseThreshold += TankMysteresis / 2;
                tankCaughtThreshold -= TankMysteresis / 2;
            } else if(tankState==TankAIState.Caught)
            {
                tankCaughtThreshold +=TankMysteresis / 2;
            }
            float distanceFromCat = Vector2.Distance(tankPosition, catPosition);
            if (distanceFromCat > tankChaseThreshold)
            {
                tankState = TankAIState.Wander;
            } else if(distanceFromCat > tankCaughtThreshold)
            {
                tankState = TankAIState.Chasing;
            } else
            {
                tankState = TankAIState.Caught;
            }
            float currentTankSpeed;
            if (tankState == TankAIState.Chasing)
            {
                tankOrientation = TurnToFace(tankPosition, catPosition, tankOrientation, TankTurnSpeed);
                currentTankSpeed = MaxTankSpeed;
            } else if (tankState == TankAIState.Wander)
            {
                Wander(tankPosition, ref tankWanderDirection, ref tankOrientation, TankTurnSpeed);
                currentTankSpeed = .25f * MaxTankSpeed;
            } else
            {
                currentTankSpeed = 0.0f;
            }
            Vector2 heading = new Vector2(
                (float)Math.Cos(tankOrientation),(float)Math.Sin(tankOrientation)
                );
            tankPosition += heading * currentTankSpeed;
        }
        private void Wander(Vector2 position,ref Vector2 wanderDirection,ref float orientation,float turnSpeed)
        {
            wanderDirection.X += MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
            wanderDirection.Y += MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
            if(wanderDirection!=Vector2.Zero)
            {
                wanderDirection.Normalize();
            }
            orientation = TurnToFace(position, position + wanderDirection, orientation, .15f * turnSpeed);
            Vector2 screnCenter = Vector2.Zero;
            screnCenter.X = graphics.GraphicsDevice.Viewport.Width / 2;
            screnCenter.Y = graphics.GraphicsDevice.Viewport.Height / 2;
            float distanceFromScreenCenter = Vector2.Distance(screnCenter, position);
            float MaxDistanceFromScreenCenter = Math.Min(screnCenter.Y, screnCenter.X);
            float normalizeDistance = distanceFromScreenCenter / MaxDistanceFromScreenCenter;
            float turnToCenterSpeed = .3f * normalizeDistance*turnSpeed;
            orientation = TurnToFace(position, screnCenter, orientation, turnToCenterSpeed);
        }
        private static float TurnToFace(Vector2 position,Vector2 faceThis,float currentAngle,float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;
            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = WrapAngle(desiredAngle - currentAngle);
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            return WrapAngle(currentAngle + difference);
        }
        private static float WrapAngle(float radians)
        {
            while (radians < MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians < MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        void HandleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();
            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            Vector2 catMovement = Vector2.Zero;
            float smoothStop = 1;
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                catMovement.X -= 1.0f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                catMovement.X += 1.0f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                catMovement.Y -= 1.0f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                catMovement.Y += 1.0f;
            }

            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            if (currentMouseState.LeftButton == ButtonState.Pressed && mousePosition != catPosition)
            {
                catMovement = mousePosition - catPosition;
                float delta = MaxCatSpeed - MathHelper.Clamp(catMovement.Length(), 0, MaxCatSpeed);
                smoothStop = 1 - delta / MaxCatSpeed;
            }

            if (catMovement != Vector2.Zero)
            {
                catMovement.Normalize();
            }

            catPosition += catMovement * MaxCatSpeed * smoothStop;
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // draw the tank, cat and mouse...
            spriteBatch.Draw(tankTexture, tankPosition, null, Color.White,
        tankOrientation, tankTextureCenter, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(catTexture, catPosition, null, Color.White,
        0.0f, catTextureCenter, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(mouseTexture, mousePosition, null, Color.White,
        mouseOrientation, mouseTextureCenter, 1.0f, SpriteEffects.None, 0.0f);

            // and then draw some text showing the tank's and mouse's current state.
            // to make the text stand out more, we'll draw the text twice, once black
            // and once white, to create a drop shadow effect.
            Vector2 shadowOffset = Vector2.One;

            spriteBatch.DrawString(spriteFont, "Tank State: \n" + tankState.ToString(),
        new Vector2(10, 10) + shadowOffset, Color.Black);
            spriteBatch.DrawString(spriteFont, "Tank State: \n" + tankState.ToString(),
        new Vector2(10, 10), Color.White);

            spriteBatch.DrawString(spriteFont, "Mouse State: \n" + mouseState.ToString(),
        new Vector2(10, 90) + shadowOffset, Color.Black);
            spriteBatch.DrawString(spriteFont, "Mouse State: \n" + mouseState.ToString(),
        new Vector2(10, 90), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        protected override void Update(GameTime gameTime)
        {
            // handle input will read the controller input, and update the cat
            // to move according to the user's whim.
            HandleInput();

            // UpdateTank will run the AI code that controls the tank's movement...
            UpdateTank();

            // ... and UpdateMouse does the same thing for the mouse.
            UpdateMouse();

            // Once we've finished that, we'll use the ClampToViewport helper function
            // to clamp everyone's position so that they stay on the screen.
            tankPosition = ClampToViewport(tankPosition);
            catPosition = ClampToViewport(catPosition);
            mousePosition = ClampToViewport(mousePosition);

            base.Update(gameTime);
        }
    }
}