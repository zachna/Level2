
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Level12
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D groundTile;
        private Texture2D IdleSheetRight;
        private Texture2D IdleSheetLeft;
        private Texture2D RunSheetRight;
        private Texture2D RunSheetLeft;
        private Texture2D CrouchSheet;
        private Texture2D JumpSheet;
        private Rectangle[] standingAnimationLeft = new Rectangle[9];
        private Rectangle[] standingAnimationRight = new Rectangle[9];
        private Rectangle[] RunAnimationLeft = new Rectangle[9];
        private Rectangle[] RunAnimationRight = new Rectangle[9];
        private Rectangle[] CrouchAnimation = new Rectangle[9];
        private Rectangle[] JumpAnimation = new Rectangle[9];

        private float timer = 0f;
        private int threshold = 150;
        private int previousAnimationIndex = 0;
        private int currentAnimationIndex = 1;
        private Keys lastPressedKey;



        private Vector2 position = new Vector2(400, 100);
        private float movementSpeed = 4f;

        private Rectangle groundRect = new Rectangle(0, 550, 800, 50); // ground rectangle

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            IdleSheetRight = Content.Load<Texture2D>("spritesheets\\IdleRight");
            IdleSheetLeft = Content.Load<Texture2D>("spritesheets\\IdleLeft");
            RunSheetRight = Content.Load<Texture2D>("spritesheets\\RunRight");
            RunSheetLeft = Content.Load<Texture2D>("spritesheets\\RunLeft");
            CrouchSheet = Content.Load<Texture2D>("spritesheets\\crouch");
            JumpSheet = Content.Load<Texture2D>("spritesheets\\jump");
            groundTile = Content.Load<Texture2D>("spritesheets\\GroundTile");


            // Load standing animation frames
            standingAnimationLeft[0] = new Rectangle(0, 0, 50, 37);
            standingAnimationLeft[1] = new Rectangle(48, 0, 50, 37);
            standingAnimationLeft[2] = new Rectangle(96, 0, 50, 37);
            standingAnimationLeft[3] = new Rectangle(144, 0, 50, 37);
            standingAnimationLeft[4] = new Rectangle(192, 0, 50, 37);
            standingAnimationLeft[5] = new Rectangle(240, 0, 50, 37);
            standingAnimationLeft[6] = new Rectangle(288, 0, 50, 37);
            standingAnimationLeft[7] = new Rectangle(336, 0, 50, 37);
            standingAnimationLeft[8] = new Rectangle(384, 0, 50, 37);


            standingAnimationRight[0] = new Rectangle(0, 0, 50, 37);
            standingAnimationRight[1] = new Rectangle(48, 0, 50, 37);
            standingAnimationRight[2] = new Rectangle(96, 0, 50, 37);
            standingAnimationRight[3] = new Rectangle(144, 0, 50, 37);
            standingAnimationRight[4] = new Rectangle(192, 0, 50, 37);
            standingAnimationRight[5] = new Rectangle(240, 0, 50, 37);
            standingAnimationRight[6] = new Rectangle(288, 0, 50, 37);
            standingAnimationRight[7] = new Rectangle(336, 0, 50, 37);
            standingAnimationRight[8] = new Rectangle(384, 0, 50, 37);


            //run animation
            RunAnimationRight[0] = new Rectangle(0, 0, 50, 37);
            RunAnimationRight[1] = new Rectangle(48, 0, 50, 37);
            RunAnimationRight[2] = new Rectangle(96, 0, 50, 37);
            RunAnimationRight[3] = new Rectangle(144, 0, 50, 37);
            RunAnimationRight[4] = new Rectangle(192, 0, 50, 37);
            RunAnimationRight[5] = new Rectangle(240, 0, 50, 37);
            RunAnimationRight[6] = new Rectangle(288, 0, 50, 37);
            RunAnimationRight[7] = new Rectangle(336, 0, 50, 37);
            RunAnimationRight[8] = new Rectangle(384, 0, 50, 37);

            RunAnimationLeft[0] = new Rectangle(0, 0, 50, 37);
            RunAnimationLeft[1] = new Rectangle(48, 0, 50, 37);
            RunAnimationLeft[2] = new Rectangle(96, 0, 50, 37);
            RunAnimationLeft[3] = new Rectangle(144, 0, 50, 37);
            RunAnimationLeft[4] = new Rectangle(192, 0, 50, 37);
            RunAnimationLeft[5] = new Rectangle(240, 0, 50, 37);
            RunAnimationLeft[6] = new Rectangle(288, 0, 50, 37);
            RunAnimationLeft[7] = new Rectangle(336, 0, 50, 37);
            RunAnimationLeft[8] = new Rectangle(384, 0, 50, 37);

            //crouch
            CrouchAnimation[0] = new Rectangle(0, 0, 50, 37);
            CrouchAnimation[1] = new Rectangle(48, 0, 50, 37);
            CrouchAnimation[2] = new Rectangle(96, 0, 50, 37);
            CrouchAnimation[3] = new Rectangle(144, 0, 50, 37);
            CrouchAnimation[4] = new Rectangle(192, 0, 50, 37);
            CrouchAnimation[5] = new Rectangle(240, 0, 50, 37);
            CrouchAnimation[6] = new Rectangle(288, 0, 50, 37);
            CrouchAnimation[7] = new Rectangle(336, 0, 50, 37);
            CrouchAnimation[8] = new Rectangle(384, 0, 50, 37);

            JumpAnimation[0] = new Rectangle(0, 0, 50, 37);
            JumpAnimation[1] = new Rectangle(0, 0, 50, 37);
            JumpAnimation[2] = new Rectangle(0, 0, 50, 37);
            JumpAnimation[3] = new Rectangle(48, 0, 50, 37);
            JumpAnimation[4] = new Rectangle(48, 0, 50, 37);
            JumpAnimation[5] = new Rectangle(48, 0, 50, 37);
            JumpAnimation[6] = new Rectangle(96, 0, 50, 37);
            JumpAnimation[7] = new Rectangle(96, 0, 50, 37);
            JumpAnimation[8] = new Rectangle(96, 0, 50, 37);
            


        }


        protected override void Update(GameTime gameTime)
        {
            // Handle user input for movement
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            if (pressedKeys.Length > 0)
            {
                lastPressedKey = pressedKeys[pressedKeys.Length - 1];
            }
            if (position.Y <= 400)
            {
                if (keyboardState.IsKeyDown(Keys.W) && position.Y > groundRect.Top) // Check if player is above ground
                {
                    
                }
                if (keyboardState.IsKeyDown(Keys.S) && position.Y < groundRect.Top - RunAnimationLeft[0].Height) // Check if player is below top of ground sprite
                {
                    position.Y += movementSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.A) && position.X > 0)
                {
                    position.X -= movementSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.D) && position.X < Window.ClientBounds.Width - RunAnimationLeft[0].Width) // Check if player is within bounds of the screen
                {
                    position.X += movementSpeed;
                }
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.W)) 
                {
                    
                }
                if (keyboardState.IsKeyDown(Keys.S) ) 
                {
                    
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    position.X -= movementSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.D) ) 
                {
                    position.X += movementSpeed;
                }
            }


            // Apply gravity and check if player is below ground
            float gravity = 0;
            if (position.Y <= 400)
            {
                gravity = 100f;
            }
           else if (keyboardState.IsKeyDown(Keys.W))
            {

                position.Y -= 40;
               
            }
            else
            {
                gravity = 0;
            }
           if (keyboardState.IsKeyUp(Keys.Space))
            {
                position.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
                
            if (position.Y > groundRect.Top - standingAnimationLeft[0].Height)
            {
                position.Y = groundRect.Top - standingAnimationLeft[0].Height;
            }

            // Update animation
            if (timer > threshold)
            {
                if (currentAnimationIndex == standingAnimationLeft.Length - 1)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex += 1;
                }
                timer = 0;
            }
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }


           
            if (position.Y > groundRect.Top - standingAnimationRight[0].Height)
            {
                position.Y = groundRect.Top - standingAnimationRight[0].Height;
            }

            // Update animation
            if (timer > threshold)
            {
                if (currentAnimationIndex == standingAnimationRight.Length - 1)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex += 1;
                }
                timer = 0;
            }
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            
            if (position.Y > groundRect.Top - RunAnimationRight[0].Height)
            {
                position.Y = groundRect.Top - RunAnimationRight[0].Height;
            }
            if (timer > threshold)
            {
                if (currentAnimationIndex == RunAnimationRight.Length - 1)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex += 1;
                }
                timer = 0;
            }
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            
            if (position.Y > groundRect.Top - RunAnimationLeft[0].Height)
            {
                position.Y = groundRect.Top - RunAnimationLeft[0].Height;
            }
            if (timer > threshold)
            {
                if (currentAnimationIndex == RunAnimationLeft.Length - 1)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex += 1;
                }
                timer = 0;
            }
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (position.Y > groundRect.Top - CrouchAnimation[0].Height)
            {
                position.Y = groundRect.Top - CrouchAnimation[0].Height;
            }
            if (timer > threshold)
            {
                if (currentAnimationIndex == CrouchAnimation.Length - 1)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex += 1;
                }
                timer = 0;
            }
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            if (position.Y > groundRect.Top - JumpAnimation[0].Height)
            {
                position.Y = groundRect.Top - JumpAnimation[0].Height;
            }
            if (timer > threshold)
            {
                if (currentAnimationIndex == JumpAnimation.Length - 1)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex += 1;
                }
                timer = 0;
            }
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            if (pressedKeys.Length > 0)
            {
                lastPressedKey = pressedKeys[pressedKeys.Length - 1];
            }
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            for (int x = 0; x < Window.ClientBounds.Width; x += 50)
            {
                for (int y = 440; y < Window.ClientBounds.Height; y += 50)
                {
                    _spriteBatch.Draw(groundTile, new Vector2(x, y), Color.White);
                }
            }
            if (keyboardState.IsKeyDown(Keys.D) && position.X > 0)
            {
                _spriteBatch.Draw(RunSheetRight, position, RunAnimationRight[currentAnimationIndex], Color.White);
            }
            else if (keyboardState.IsKeyDown(Keys.A) && position.X > 0)
            {
                _spriteBatch.Draw(RunSheetLeft, position, RunAnimationLeft[currentAnimationIndex], Color.White);
            }
            else if (lastPressedKey == Keys.D && keyboardState.IsKeyUp(Keys.D))
            {
                _spriteBatch.Draw(IdleSheetRight, position, standingAnimationRight[currentAnimationIndex], Color.White);
            }
            else if (lastPressedKey == Keys.A && keyboardState.IsKeyUp(Keys.A))
            {
                _spriteBatch.Draw(IdleSheetLeft, position, standingAnimationLeft[currentAnimationIndex], Color.White);
            }
            else if (keyboardState.IsKeyDown(Keys.Space))
            {
                _spriteBatch.Draw(CrouchSheet, position,CrouchAnimation[currentAnimationIndex], Color.White);
            }

            else if (keyboardState.IsKeyDown(Keys.W))
            {
                _spriteBatch.Draw(JumpSheet, position, JumpAnimation[currentAnimationIndex], Color.White);
            }
            else
            {
                _spriteBatch.Draw(IdleSheetRight, position, standingAnimationRight[currentAnimationIndex], Color.White);
            }
            _spriteBatch.Draw(new Texture2D(GraphicsDevice, 1, 1), groundRect, Color.White); // Draw the ground rectangle

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
