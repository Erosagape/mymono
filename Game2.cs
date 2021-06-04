using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace monotest
{
    class Game2:Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ParticleEngine particleEngine;
        public Game2()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void UnloadContent()
        {
            
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(Content.Load<Texture2D>("Images/circle"));
            textures.Add(Content.Load<Texture2D>("Images/star"));
            textures.Add(Content.Load<Texture2D>("Images/diamond"));
            particleEngine = new ParticleEngine(textures, new Vector2(200, 440));
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            particleEngine.EmitterLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            particleEngine.Update();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            particleEngine.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}