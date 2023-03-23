using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
namespace Level1
{
    public class Game1 : Game
    {
        private int _characterWidth = 48;
        private int _characterHeight = 48;
        private bool _previousActionKeyPressed;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Character animations
        private Texture2D _idleTexture, _jumpTexture, _actionTexture, _runningTexture;
        private Vector2 _characterPosition;
        private bool _isFacingRight = true;

        // Character states
        private enum CharacterState { Idle, Jumping, Action, Running }
        private CharacterState _characterState;
        private int _idleFrameCount = 9;
        private int _idleFrameWidth = 48;
        private int _currentIdleFrame = 0;
        private double _idleFrameTime = 0;
        private double _idleAnimationSpeed = 100;
        private int _actionFrameCount = 10;
        private int _runningFrameCount = 8;
        private int _currentActionFrame = 0;
        private int _currentRunningFrame = 0;
        private double _actionFrameTime = 0;
        private double _runningFrameTime = 0;
        private double _actionAnimationSpeed = 100;
        private double _runningAnimationSpeed = 100;
        private int _jumpingFrameCount = 3;
        private int _currentJumpingFrame = 0;
        private int _jumpingFrameWidth = 48; // Assuming each frame has a width of 48 pixels
        private float _previousYPosition;
        private Texture2D _tileTexture;
        private int _tileSize = 50;
        private List<Rectangle> _groundTiles;
        private List<Rectangle> _platformTiles;
        private Texture2D _boxTexture;
        private Vector2 _boxPosition;
        private bool _isPushingBox = false;
        private Rectangle _newPlatformRectangle = new Rectangle(500, 300, 200, 40);

        private Rectangle _newPlatformRectangle2 = new Rectangle(100, 300, 200, 40);
        private Texture2D _newPlatformTexture;

        // Gravity and jumping
        private float _gravity = 0.5f;
        private float _jumpForce = 15f;
        private float _characterVelocityY;

        //Animation
        private Rectangle[] standingAnimation = new Rectangle[9];
        private Rectangle[] actionAnimation = new Rectangle[9];
        private Rectangle[] Run = new Rectangle[9];

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Character initial position
            _characterPosition = new Vector2(100, 390);


            _groundTiles = new List<Rectangle>();
            _platformTiles = new List<Rectangle>();

            // Create ground tiles
            for (int i = 0; i < GraphicsDevice.Viewport.Width / _tileSize; i++)
            {
                _groundTiles.Add(new Rectangle(i * _tileSize, GraphicsDevice.Viewport.Height - _tileSize, _tileSize, _tileSize));
            }

            // Create a platform
            for (int i = 5; i < 10; i++)
            {
                _platformTiles.Add(new Rectangle(i * _tileSize, GraphicsDevice.Viewport.Height - _tileSize * 4, _tileSize, _tileSize));
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load your sprites
            _idleTexture = Content.Load<Texture2D>("spritesheets\\IdleRight");
            _jumpTexture = Content.Load<Texture2D>("spritesheets\\jump");
            _actionTexture = Content.Load<Texture2D>("spritesheets\\crouch");
            _runningTexture = Content.Load<Texture2D>("spritesheets\\RunRight");
            _tileTexture = Content.Load<Texture2D>("spritesheets\\GroundTile");
            _boxTexture = Content.Load<Texture2D>("spritesheets\\bOX");
            _newPlatformTexture = Content.Load<Texture2D>("spritesheets\\new platform");
            _boxPosition = new Vector2(200, 390);

        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (_characterState != CharacterState.Jumping && _characterState != CharacterState.Action)
            {
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    _characterPosition.X -= 5;
                    _isFacingRight = false;
                    _characterState = CharacterState.Running;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    _characterPosition.X += 5;
                    _isFacingRight = true;
                    _characterState = CharacterState.Running;
                }
                else
                {
                    _characterState = CharacterState.Idle;
                }
            }

            if (keyboardState.IsKeyDown(Keys.W) && _characterState != CharacterState.Jumping && _characterState != CharacterState.Action)
            {
                _characterState = CharacterState.Jumping;
                _characterVelocityY = -_jumpForce;
            }

            if (_characterState == CharacterState.Jumping)
            {
                _characterPosition.Y += _characterVelocityY;
                _characterVelocityY += _gravity;

                // Check if the character is on the ground
                if (_characterPosition.Y > 390)
                {
                    _characterState = CharacterState.Idle;
                    _characterPosition.Y = 390;
                }
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !keyboardState.IsKeyDown(Keys.W))
            {
                _characterState = CharacterState.Action;
            }

            // Animation logic for idle and running states
            if (_characterState == CharacterState.Idle || _characterState == CharacterState.Running)
            {
                double frameTime = gameTime.ElapsedGameTime.TotalMilliseconds;
                int frameCount = (_characterState == CharacterState.Idle) ? _idleFrameCount : _runningFrameCount;
                double animationSpeed = (_characterState == CharacterState.Idle) ? _idleAnimationSpeed : _runningAnimationSpeed;
                int currentFrame = (_characterState == CharacterState.Idle) ? _currentIdleFrame : _currentRunningFrame;
                double frameTimeCounter = (_characterState == CharacterState.Idle) ? _idleFrameTime : _runningFrameTime;

                frameTimeCounter += frameTime;

                if (frameTimeCounter >= animationSpeed)
                {
                    currentFrame = (currentFrame + 1) % frameCount;
                    frameTimeCounter = 0;
                }

                if (_characterState == CharacterState.Idle)
                {
                    _currentIdleFrame = currentFrame;
                    _idleFrameTime = frameTimeCounter;
                }
                else
                {
                    _currentRunningFrame = currentFrame;
                    _runningFrameTime = frameTimeCounter;
                }
            }

            // Update box position based on input
            if (_isPushingBox && keyboardState.IsKeyDown(Keys.A))
            {
                _boxPosition.X -= 5;
            }
            else if (_isPushingBox && keyboardState.IsKeyDown(Keys.D))
            {
                _boxPosition.X += 5;
            }
            else
            {
                _isPushingBox = false;
            }

            // Box and character collision logic
            //UpdateBoxCharacterCollision();

            // Platform and character collision logic
           // UpdatePlatformCharacterCollision();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_boxTexture, _boxPosition, Color.White);

            SpriteEffects spriteEffect = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            switch (_characterState)
            {
                case CharacterState.Idle:
                    Rectangle idleSourceRect = new Rectangle(_idleFrameWidth * _currentIdleFrame, 0, _idleFrameWidth, _idleTexture.Height);
                    _spriteBatch.Draw(_idleTexture, _characterPosition, idleSourceRect, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
                    break;
                case CharacterState.Jumping:
                    Rectangle jumpingSourceRect = new Rectangle(_jumpingFrameWidth * _currentJumpingFrame, 0, _jumpingFrameWidth, _jumpTexture.Height);
                    _spriteBatch.Draw(_jumpTexture, _characterPosition, jumpingSourceRect, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
                    break;

                case CharacterState.Running:
                    Rectangle runningSourceRect = new Rectangle(_idleFrameWidth * _currentRunningFrame, 0, _idleFrameWidth, _runningTexture.Height);
                    _spriteBatch.Draw(_runningTexture, _characterPosition, runningSourceRect, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
                    break;

                case CharacterState.Action:
                    Rectangle actionSourceRect = new Rectangle(_idleFrameWidth * _currentActionFrame, 0, _idleFrameWidth, _actionTexture.Height);
                    _spriteBatch.Draw(_actionTexture, _characterPosition, actionSourceRect, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
                    break;
            }

            foreach (Rectangle groundTile in _groundTiles)
            {
                _spriteBatch.Draw(_tileTexture, groundTile, Color.White);
            }

            _spriteBatch.Draw(_newPlatformTexture, _newPlatformRectangle, Color.White);
            _spriteBatch.Draw(_newPlatformTexture, _newPlatformRectangle2, Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}