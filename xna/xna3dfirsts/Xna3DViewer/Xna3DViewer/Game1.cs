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

namespace Xna3DViewer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model myModel;
        
        SoundEffect soundEngine;
        SoundEffectInstance soundEngineInstance;
        SoundEffect soundHyperspaceActivation;

        float aspectRatio;
        float modelRotation = 0.0f;

        Vector3 modelPosition = Vector3.Zero;
        Vector3 cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);
        Vector3 modelVelocity = Vector3.Zero;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            myModel = Content.Load<Model>("Models\\p1_wedge");
            soundEngine = Content.Load<SoundEffect>("Audio\\Waves\\engine_2");
            soundEngineInstance = soundEngine.CreateInstance();
            soundHyperspaceActivation =
                Content.Load<SoundEffect>("Audio\\Waves\\hyperspace_activate");

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if ( GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Get some input
            UpdateInput();

            // Add velocity to the current position
            modelPosition += modelVelocity;

            // Bleed off velocity over time
            modelVelocity *= 0.95f;

            // We stop the automatic rotation, so that we can control it manually
            // modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.1f);

            base.Update(gameTime);
        }


        protected void UpdateInput()
        {
            // Get the game pad state.
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            KeyboardState currentKeyState = Keyboard.GetState();
            //if (currentState.IsConnected)
            //{
                // Rotate the model using the left thumbstick, and scale it down
                    if(currentKeyState.IsKeyDown(Keys.A))
                modelRotation += 0.10f;
                else if (currentKeyState.IsKeyDown(Keys.D))
                modelRotation -= 0.10f;
                else
                modelRotation -= currentState.ThumbSticks.Left.X * 0.10f;

                // Create some velocity if the right trigger is down.
                Vector3 modelVelocityAdd = Vector3.Zero;

                // Find out what direction we should be thrusting, 
                // using rotation.
                modelVelocityAdd.X = -(float)Math.Sin(modelRotation);
                modelVelocityAdd.Z = -(float)Math.Cos(modelRotation);

                // Now scale our direction by how hard the trigger is down.
                if (currentKeyState.IsKeyDown(Keys.W))
                    modelVelocityAdd *= 1;
                else
                modelVelocityAdd *= currentState.Triggers.Right;

                // Finally, add this vector to our velocity.
                modelVelocity += modelVelocityAdd;

                GamePad.SetVibration(PlayerIndex.One,
                    currentState.Triggers.Right,
                    currentState.Triggers.Right);

                //Play engine sound only when the engine is on.
                //if (currentState.Triggers.Right > 0)
                if (currentKeyState.IsKeyDown(Keys.W))
                {

                    if (soundEngineInstance.State == SoundState.Stopped)
                    {
                        soundEngineInstance.Volume = 0.75f;
                        soundEngineInstance.IsLooped = true;
                        soundEngineInstance.Play();
                    }
                    else
                        soundEngineInstance.Resume();
                }
                else if (currentState.Triggers.Right == 0)
                {
                    if (soundEngineInstance.State == SoundState.Playing)
                        soundEngineInstance.Pause();
                }

                // In case you get lost, press A to warp back to the center.
                if (currentState.Buttons.A == ButtonState.Pressed || currentKeyState.IsKeyDown(Keys.Enter))
                {
                    modelPosition = Vector3.Zero;
                    modelVelocity = Vector3.Zero;
                    modelRotation = 0.0f;
                    soundHyperspaceActivation.Play();
                }
            //}

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AntiqueWhite);

            // TODO: Add your drawing code here
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                }
                mesh.Draw();

            }

            base.Draw(gameTime);
        }
    }
}
