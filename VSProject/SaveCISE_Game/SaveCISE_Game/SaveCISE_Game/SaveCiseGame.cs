using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SaveCISE_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SaveCiseGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Stage stage;
        KeyboardState oldKeyboardState;
        ButtonState oldLeftMouseState;

        public SaveCiseGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            Content.RootDirectory = "Content";
            oldKeyboardState = new KeyboardState();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            stage = new Stage();
            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ContentStore.addTexture("spr_beginButton", Content.Load<Texture2D>("Sprites/spr_beginButton"));
            ContentStore.addTexture("spr_quitButton", Content.Load<Texture2D>("Sprites/spr_quitButton"));
            ContentStore.addTexture("bg_mainScreen", Content.Load<Texture2D>("Backgrounds/bg_mainScreen"));

            buildScenes();
        }

        private void buildScenes()
        {
            Scene openingScene = new Scene();
            
            Sprite bg = new Sprite(ContentStore.getTexture("bg_mainScreen"));
            openingScene.setBackground(bg);

            stage.addScene(openingScene);

            Button playButton = new Button(200,250, new Sprite(ContentStore.getTexture("spr_beginButton"),200,50,3,3));
            openingScene.add(playButton);

            GameAction nextSceneAction = new ChangeSceneGameAction(stage, 1);
            playButton.setMouseReleasedAction(nextSceneAction);

            Button quitButton = new Button(200, 310, new Sprite(ContentStore.getTexture("spr_quitButton"), 200, 50, 3, 3));
            openingScene.add(quitButton);

            GameAction quitAction = new QuitGameAction(this);
            quitButton.setMouseReleasedAction(quitAction);

            // Build "How to Play" scene here
            Scene instructionScene = new Scene();

            stage.addScene(instructionScene);

            // Build Play Scene Here

        }

        /*
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Our game is not large enough for this to have bearing
        }*/

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            doKeyboardEvents();
            doMouseEvents();

            base.Update(gameTime);
        }

        private void doMouseEvents()
        {
            MouseState ms = Mouse.GetState();
            if (ms.LeftButton != oldLeftMouseState)
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    stage.leftMousePressed(ms.X, ms.Y);
                }
                else
                {
                    stage.leftMouseReleased(ms.X, ms.Y);
                }
            }
            else
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    //dragging
                }
                else
                {
                    stage.mouseOver(ms.X, ms.Y);
                }
            }
            oldLeftMouseState = ms.LeftButton;
        }

        private void doKeyboardEvents()
        {
            KeyboardState ks = Keyboard.GetState();

            // Allows the game to exit
            if (ks.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            List<Keys> oldPressedKeys = oldKeyboardState.GetPressedKeys().ToList<Keys>();
            Keys[] pressedKeys = ks.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                if (!oldKeyboardState.IsKeyDown(key))
                {
                    stage.keyPressed(key);
                }
                else
                {
                    oldPressedKeys.Remove(key);
                }
            }
            foreach (Keys key in oldPressedKeys)
            {
                stage.keyReleased(key);
            }

            oldKeyboardState = ks;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            stage.draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
