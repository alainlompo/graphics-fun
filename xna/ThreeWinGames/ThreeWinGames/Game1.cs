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

namespace ThreeWinGames
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Change this value to any desired number to display more or less sprites on the screen
        const int NUMBER_OF_SPRITES = 65;

        // Change this value to impact the horizontal component of a sprite SPEED
        // Notice that the speeds are randomly commputed
        const int X_MAX_SPEED = 185;
        // Change this value to impact the vertical component of a sprite SPEED
        const int Y_MAX_SPEED = 130;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D[] spriteTextures;
        Texture2D obstacleTexture;
        Vector2[] positions = new Vector2[NUMBER_OF_SPRITES];
        Vector2[] speeds = new Vector2[NUMBER_OF_SPRITES];
        Vector2 obstaclePosition;
        bool isStartOfGame = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        Rectangle getObstacleBoundingBox()
        {
            return new Rectangle(
                (int)obstaclePosition.X,
                (int)obstaclePosition.Y,
                obstacleTexture.Width,
                obstacleTexture.Height
                );
        }

        Rectangle getSpriteBoundingBox(int spriteIndex)
        {
            return new Rectangle(
                (int)positions[spriteIndex].X,
                (int)positions[spriteIndex].Y,
                Math.Max(spriteTextures[0].Width,spriteTextures[1].Width),
                Math.Max(spriteTextures[0].Height, spriteTextures[1].Height)
                );
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initializes the positions and speeds
            Random randomizer = new Random();
            for (int i = 0; i < NUMBER_OF_SPRITES; i++)
            {
                positions[i] = new Vector2(randomizer.Next(0, graphics.GraphicsDevice.Viewport.Width) * 1.0f,
                    randomizer.Next(0, graphics.GraphicsDevice.Viewport.Height) * 1.0f);
                
                int[] signs = new int[] { -1, +1 };
                speeds[i] = new Vector2(randomizer.Next(1, X_MAX_SPEED) * 1.0f * signs[randomizer.Next(0, 2)],
                    randomizer.Next(1, Y_MAX_SPEED) * 1.0f * signs[randomizer.Next(0, 2)]);
            }

           

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
            spriteTextures = new Texture2D[2];
            spriteTextures[0] = Content.Load<Texture2D>("red-star");
            spriteTextures[1] = Content.Load<Texture2D>("green-diamond");
            obstacleTexture = Content.Load<Texture2D>("obstacle");
            obstaclePosition = new Vector2((graphics.GraphicsDevice.Viewport.Width - obstacleTexture.Width) / 2,
            (graphics.GraphicsDevice.Viewport.Height - obstacleTexture.Height) / 2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        protected void updatePositions(GameTime gameTime)
        {
            int maxX = graphics.GraphicsDevice.Viewport.Width -Math.Max( spriteTextures[0].Width, spriteTextures[1].Width);
            int minX = 0;
            int maxY = graphics.GraphicsDevice.Viewport.Height - Math.Max( spriteTextures[0].Height, spriteTextures[1].Height);
            int minY = 0;

            for (int i = 0; i < NUMBER_OF_SPRITES; i++)
            {

                positions[i] += speeds[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (positions[i].X > maxX)
                {
                    speeds[i].X *= -1;
                    positions[i].X = maxX;
                }

                else if (positions[i].X < minX)
                {
                    speeds[i].X *= -1;
                    positions[i].X = minX;
                }

                if (positions[i].Y > maxY)
                {
                    speeds[i].Y *= -1;
                    positions[i].Y = maxY;
                }

                else if (positions[i].Y < minY)
                {
                    speeds[i].Y *= -1;
                    positions[i].Y = minY;
                }

                // Bouncing against the obstacle, the sprite is send back in the opposite direction
                // TODO: to improve this later
                if (getSpriteBoundingBox(i).Intersects(getObstacleBoundingBox()))
                {
                    speeds[i].X *= -1;
                    speeds[i].Y *= -1;
                }
                
            }

        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Move the sprite by speed
            updatePositions(gameTime);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Random randomizer = new Random();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(obstacleTexture, obstaclePosition, Color.White);
            Boolean inverser = false;
            for (int i = 0; i < NUMBER_OF_SPRITES; i++)
            {
                // Since we have an obstacle in the center of the screen
                // We want to avoid the case of an initial position already intersecting the obstacle
                // If such is the case we move it somewhere else
                if (isStartOfGame)
                {
                   
                    float xPos = positions[i].X;
                    float yPos = positions[i].Y;
                    if (xPos + Math.Max(spriteTextures[0].Width, spriteTextures[1].Width) >= obstaclePosition.X && xPos <= obstaclePosition.X + obstacleTexture.Width
                        && yPos + Math.Max(spriteTextures[0].Height, spriteTextures[1].Height) >= obstaclePosition.Y && yPos  <= obstaclePosition.Y + obstacleTexture.Height)
                    {
                        positions[i] = new Vector2(1, 1);
                    }


                }

                if (inverser)
                {
                    spriteBatch.Draw(spriteTextures[0], positions[i], Color.White);
                }

                else
                {
                    spriteBatch.Draw(spriteTextures[1], positions[i], Color.White);
                }

                inverser = !inverser;
            }
            spriteBatch.End();
            base.Draw(gameTime);
            isStartOfGame = false;
        }
    }
}
