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
        private Texture2D _idleTexture, _jumpTexture, _actionTexture, _runningTexture;
        private Vector2 _characterPosition;
        private bool _isFacingRight = true;
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
        private bool _isMovingLeft = false;
        private int _jumpingFrameCount = 3;
        private int _currentJumpingFrame = 0;
        private int _jumpingFrameWidth = 48; 
        private float _previousYPosition;
        private Texture2D _tileTexture;
        private int _tileSize = 50;
        private List<Rectangle> _groundTiles;
        private List<Rectangle> _platformTiles;
        private Texture2D _boxTexture;
        private Vector2 _boxPosition;
        private bool _isPushingBox = false;
        private Rectangle _newPlatformRectangle = new Rectangle(500, 300, 200, 40);
        private KeyboardState _previousKeyboardState;
        private Rectangle _newPlatformRectangle2 = new Rectangle(100, 300, 200, 40);
        private Texture2D _newPlatformTexture;
        private bool _isBoxOnPlatform = false;
        private float _gravity = 0.5f;
        private float _jumpForce = 15f;
        private float _characterVelocityY;
        private const int _boxSpeed = 5;
        private Rectangle[] standingAnimation = new Rectangle[9];
        private Rectangle[] actionAnimation = new Rectangle[9];
        private Rectangle[] Run = new Rectangle[9];
        private Vector2 _boxVelocity;
        private bool _isBoxBeingPushed = false;
       

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
          
            _characterPosition = new Vector2(100, 390);


            _groundTiles = new List<Rectangle>();
            _platformTiles = new List<Rectangle>();
            for (int i = 0; i < GraphicsDevice.Viewport.Width / _tileSize; i++)
            {
                _groundTiles.Add(new Rectangle(i * _tileSize, GraphicsDevice.Viewport.Height - _tileSize, _tileSize, _tileSize));
            }

            for (int i = 5; i < 10; i++)
            {
                _platformTiles.Add(new Rectangle(i * _tileSize, GraphicsDevice.Viewport.Height - _tileSize * 4, _tileSize, _tileSize));
            }
            if (_isPushingBox)
            {
                _boxPosition.X += (_isFacingRight ? 1 : -1) * _boxSpeed;
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _idleTexture = Content.Load<Texture2D>("spritesheets\\IdleRight");
            _jumpTexture = Content.Load<Texture2D>("spritesheets\\jump");
            _actionTexture = Content.Load<Texture2D>("spritesheets\\crouch");
            _runningTexture = Content.Load<Texture2D>("spritesheets\\RunRight");
            _tileTexture = Content.Load<Texture2D>("spritesheets\\GroundTile");
            _boxTexture = Content.Load<Texture2D>("spritesheets\\bOX");
            _newPlatformTexture = Content.Load<Texture2D>("spritesheets\\new platform");
            _boxPosition = new Vector2(200, 260);

        }
        private void UpdatePlatformCharacterCollision()
        {
            Rectangle characterRect = new Rectangle(
                (int)_characterPosition.X, (int)_characterPosition.Y, _characterWidth, _characterHeight);

            if (characterRect.Intersects(_newPlatformRectangle))
            {
                Vector2 characterCenter = new Vector2(
                    _characterPosition.X + _characterWidth / 2,
                    _characterPosition.Y + _characterHeight / 2);

                Vector2 platformCenter = new Vector2(
                    _newPlatformRectangle.X + _newPlatformRectangle.Width / 2,
                    _newPlatformRectangle.Y + _newPlatformRectangle.Height / 2);

                Vector2 penetration = new Vector2(
                    characterRect.Width / 2 + _newPlatformRectangle.Width / 2 - System.Math.Abs(characterCenter.X - platformCenter.X),
                    characterRect.Height / 2 + _newPlatformRectangle.Height / 2 - System.Math.Abs(characterCenter.Y - platformCenter.Y));

                if (penetration.X < penetration.Y)
                {
                    _characterPosition.X += (characterCenter.X < platformCenter.X) ? -penetration.X : penetration.X;
                }
                else
                {
                    float prevY = _previousYPosition;
                    _characterPosition.Y += (characterCenter.Y < platformCenter.Y) ? -penetration.Y : penetration.Y;

                    if (_characterPosition.Y < prevY && _characterState == CharacterState.Jumping)
                    {
                        _characterState = CharacterState.Idle;
                    }
                }
            }
        }


        private bool IsCharacterOnGroundOrPlatform()
        {
            Rectangle characterRect = new Rectangle(
                (int)_characterPosition.X, (int)_characterPosition.Y, _characterWidth, _characterHeight);

            if (_characterPosition.Y >= 390)
            {
                return true;
            }

            if (characterRect.Intersects(_newPlatformRectangle))
            {
                return true;
            }

            return false;
        }
        private bool IsCharacterOnGroundOrPlatform2()
        {
            Rectangle characterRect = new Rectangle((int)_characterPosition.X, (int)(_characterPosition.Y + 1), _characterWidth, _characterHeight);

            if (characterRect.Intersects(_newPlatformRectangle) || characterRect.Intersects(_newPlatformRectangle2))
            {
                return true;
            }

            return _characterPosition.Y >= 390;
        }
        private void UpdatePlatform2CharacterCollision()
        {
            Rectangle characterRect = new Rectangle(
                (int)_characterPosition.X, (int)_characterPosition.Y, _characterWidth, _characterHeight);

            Rectangle topHalfPlatformRect = new Rectangle(
                _newPlatformRectangle2.X, _newPlatformRectangle2.Y,
                _newPlatformRectangle2.Width, _newPlatformRectangle2.Height / 2);

            Rectangle bottomHalfPlatformRect = new Rectangle(
                _newPlatformRectangle2.X, _newPlatformRectangle2.Y + _newPlatformRectangle2.Height / 2,
                _newPlatformRectangle2.Width, _newPlatformRectangle2.Height / 2);

            if (_characterVelocityY > 0 && characterRect.Intersects(bottomHalfPlatformRect))
            {
               
                _characterPosition.Y = _newPlatformRectangle2.Y - _characterHeight;
                _characterVelocityY = 0;
                _characterState = CharacterState.Idle;
            }
            else if (characterRect.Intersects(topHalfPlatformRect))
            {
              
                if (_characterVelocityY < 0)
                {
             
                    return;
                }
                else
                { 
                    _characterPosition.Y = _newPlatformRectangle2.Y - _characterHeight;
                    _characterVelocityY = 0;
                    _characterState = CharacterState.Idle;
                }
            }
        }


        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();
            Rectangle playerBoundingBox = new Rectangle((int)_characterPosition.X, (int)_characterPosition.Y, _idleFrameWidth, _idleTexture.Height);
            Rectangle boxBoundingBox = new Rectangle((int)_boxPosition.X, (int)_boxPosition.Y, _boxTexture.Width, _boxTexture.Height);


            if (_characterState != CharacterState.Jumping && _characterState != CharacterState.Action)
            {
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    _characterPosition.X -= 0;
                    _isFacingRight = false;
                    _characterState = CharacterState.Running;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    _characterPosition.X += 0;
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
           
            {

            }
            if (_characterState == CharacterState.Jumping)
            {
                _characterPosition.Y += _characterVelocityY;
                _characterVelocityY += _gravity;

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

         
            

            
            if (_characterState == CharacterState.Jumping)
            {
                _characterPosition.Y += _characterVelocityY;
                _characterVelocityY += _gravity;

                if (_characterPosition.Y > 390)
                {
                    _characterState = CharacterState.Idle;
                    _characterPosition.Y = 390;
                }
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !keyboardState.IsKeyDown(Keys.W) && !_previousActionKeyPressed && _characterState != CharacterState.Jumping)
            {
                _characterState = CharacterState.Action;
                if (playerBoundingBox.Intersects(boxBoundingBox))
                {
                    _isBoxBeingPushed = true;
                    _boxVelocity = _isFacingRight ? new Vector2(10, -5) : new Vector2(-10, -5);
                }
            }

            if (!_isBoxOnPlatform)
            {
                _boxPosition.Y += _gravity + 5;
            }

            if (_isBoxBeingPushed)
            {
                _boxPosition += _boxVelocity;
                _boxVelocity.Y += _gravity;
            }
            else
            {
                _boxVelocity.X = 0;
            }

            bool hasCollided = false;
            foreach (Rectangle groundTile in _groundTiles)
            {
                if (boxBoundingBox.Intersects(groundTile))
                {
                    if (_isBoxBeingPushed)
                    {
                        _isBoxBeingPushed = false;
                    }
                    _boxPosition.Y = groundTile.Y - _boxTexture.Height;
                    hasCollided = true;
                    break;
                }
            }

            foreach (Rectangle platformTile in _platformTiles)
            {
                if (boxBoundingBox.Intersects(platformTile))
                {
                    if (_isBoxBeingPushed)
                    {
                        _isBoxBeingPushed = false;
                    }
                    _boxPosition.Y = platformTile.Y - _boxTexture.Height;
                    hasCollided = true;
                    break;
                }
            }

            if (boxBoundingBox.Intersects(_newPlatformRectangle2))
            {
                if (_isBoxBeingPushed)
                {
                    _isBoxBeingPushed = false;
                }
                _boxPosition.Y = _newPlatformRectangle2.Y - _boxTexture.Height;
                hasCollided = true;
            }

            if (hasCollided)
            {
                _boxVelocity = Vector2.Zero;
                _isBoxOnPlatform = true;
            }
            else
            {
                _isBoxOnPlatform = false;
            }
            if (_characterState == CharacterState.Running)
            {
               

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    _isFacingRight = false;
                    _characterState = CharacterState.Running;

                    if (boxBoundingBox.Right >= _characterPosition.X && _isBoxOnPlatform && _characterPosition.X > _boxPosition.X)
                    {
                        _boxPosition.X -= 5;
                    }
                    else
                    {
                        _characterPosition.X -= 5;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    _isFacingRight = true;
                    _characterState = CharacterState.Running;

                    if (boxBoundingBox.Left <= _characterPosition.X + _idleFrameWidth && _isBoxOnPlatform && _characterPosition.X < _boxPosition.X)
                    {
                        _boxPosition.X += 5;
                    }
                    else
                    {
                        _characterPosition.X += 5;
                    }
                }
                else if (_characterState != CharacterState.Jumping)
                {
                    _characterState = CharacterState.Idle;
                }
            }

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


                    Rectangle boxRectangle = new Rectangle((int)_boxPosition.X, (int)_boxPosition.Y, _boxTexture.Width, _boxTexture.Height);
                    if (keyboardState.IsKeyDown(Keys.A))
                    {
                        _isFacingRight = false;
                        _characterState = CharacterState.Running;
                        _characterPosition.X -= 0;
                        _isMovingLeft = true;
                        if (boxRectangle.Right >= _characterPosition.X && _isBoxOnPlatform)
                        {
                            _boxPosition.X -= 5;
                        }
                    }
                    else if (keyboardState.IsKeyDown(Keys.D))
                    {
                        
                        _isFacingRight = true;
                        _characterState = CharacterState.Running;
                        _characterPosition.X += 0;
                        _isMovingLeft = false;
                        if (boxRectangle.Left <= _characterPosition.X + _idleFrameWidth && _isBoxOnPlatform)
                        {
                            _boxPosition.X += 5;
                        }
                    }
                    else if (_characterState != CharacterState.Jumping)
                    {
                        _characterState = CharacterState.Idle;
                        _isMovingLeft = false;
                    }
                }
            }

            if (_characterState != CharacterState.Action)
            {
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    _characterPosition.X -= 5;
                    _isFacingRight = false;
                    if (_characterState != CharacterState.Jumping)
                    {
                        _characterState = CharacterState.Running;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    _characterPosition.X += 5;
                    _isFacingRight = true;
                    if (_characterState != CharacterState.Jumping)
                    {
                        _characterState = CharacterState.Running;
                    }
                }
                else if (_characterState != CharacterState.Jumping)
                {
                    _characterState = CharacterState.Idle;
                }
            }
            if (keyboardState.IsKeyDown(Keys.Space) && !keyboardState.IsKeyDown(Keys.W) && !_previousActionKeyPressed && _characterState != CharacterState.Jumping)
            {
                _characterState = CharacterState.Action;
            }

            if (_characterState == CharacterState.Action)
            {
                double frameTime = gameTime.ElapsedGameTime.TotalMilliseconds;

                _actionFrameTime += frameTime;

                if (_actionFrameTime >= _actionAnimationSpeed)
                {
                    _currentActionFrame = (_currentActionFrame + 1) % _actionFrameCount;
                    _actionFrameTime = 0;

                    if (_currentActionFrame == 0) 
                    {
                        _characterState = CharacterState.Idle;
                    }
                }
            }

            _previousActionKeyPressed = keyboardState.IsKeyDown(Keys.Space);

         
            _previousKeyboardState = keyboardState;
            _previousYPosition = _characterPosition.Y;

            if (!_isBoxOnPlatform)
            {
                _boxPosition.Y += _gravity + 5;
                
            }

            Rectangle boxRectangle2 = new Rectangle((int)_boxPosition.X, (int)_boxPosition.Y, _boxTexture.Width, _boxTexture.Height);
            foreach (Rectangle groundTile in _groundTiles)
            {
                if (boxRectangle2.Intersects(groundTile))
                {
                    _isBoxOnPlatform = false;
                    _boxPosition.Y = groundTile.Y - _boxTexture.Height;
                   
                    break;
                }
            }
            foreach (Rectangle platformTile in _platformTiles)
            {
                if (boxRectangle2.Intersects(platformTile))
                {
                    _isBoxOnPlatform = true;
                    _boxPosition.Y = platformTile.Y - _boxTexture.Height;
                   
                    break;
                }
            }
            if (boxRectangle2.Intersects(_newPlatformRectangle2))
            {
                _isBoxOnPlatform = true;
                _boxPosition.Y = _newPlatformRectangle2.Y - _boxTexture.Height;
                
            }
            else
            {
                _isBoxOnPlatform = false;
            }

            if (keyboardState.IsKeyDown(Keys.A) && _characterState != CharacterState.Action && _characterState != CharacterState.Jumping)
            {
                _characterPosition.X -= 5;
                _isFacingRight = false;
                _characterState = CharacterState.Running;
            }
            else if (keyboardState.IsKeyDown(Keys.D) && _characterState != CharacterState.Action && _characterState != CharacterState.Jumping)
            {
                _characterPosition.X += 5;
                _isFacingRight = true;
                _characterState = CharacterState.Running;
            }

            Rectangle boxRect = new Rectangle((int)_boxPosition.X, (int)_boxPosition.Y, _boxTexture.Width, _boxTexture.Height);
            Rectangle characterRect = new Rectangle((int)_characterPosition.X, (int)_characterPosition.Y, _characterWidth, _characterHeight);

            if (characterRect.Intersects(boxRect))
            {
            
                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D))
                {
                 
                    _isPushingBox = true;
                }
                else
                {
                   
                    _isPushingBox = false;
                }
            }

            if (_isPushingBox)
            {
            
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    _boxPosition.X -= 5;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    _boxPosition.X += 5;
                }

            
                foreach (Rectangle platformTile in _platformTiles)
                {
                    if (_boxPosition.Y + _boxTexture.Height == platformTile.Y && _boxPosition.X + _boxTexture.Width > platformTile.X && _boxPosition.X < platformTile.X + platformTile.Width)
                    {
                        
                        _isPushingBox = false;
                        _boxPosition.Y = platformTile.Y - _boxTexture.Height;
                        break;
                    }
                }

                if (_boxPosition.Y + _boxTexture.Height > 390)
                {
                   
                    _isPushingBox = false;
                    _boxPosition.Y = 390 - _boxTexture.Height;
                }
            }


            UpdatePlatformCharacterCollision(); 
            UpdatePlatform2CharacterCollision();

            if (!IsCharacterOnGroundOrPlatform() && _characterState != CharacterState.Jumping)
            {
                _characterState = CharacterState.Jumping;
                _characterVelocityY = 1f;
            }
            if (!IsCharacterOnGroundOrPlatform2() && _characterState != CharacterState.Jumping)
            {
                _characterState = CharacterState.Jumping;
                _characterVelocityY = 1f;
            }
            base.Update(gameTime);
       

        }

        private void UpdateCharacterMovementAndStates(KeyboardState keyboardState, GameTime gameTime, Rectangle playerBoundingBox, Rectangle boxBoundingBox)
        {
            if (_characterState != CharacterState.Jumping && _characterState != CharacterState.Action)
            {
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    _isFacingRight = false;
                    _characterState = CharacterState.Running;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    _isFacingRight = true;
                    _characterState = CharacterState.Running;
                }
                else
                {
                    _characterState = CharacterState.Idle;
                }
            }

            if (keyboardState.IsKeyDown(Keys.A) && playerBoundingBox.Right >= boxBoundingBox.Left && _isBoxOnPlatform && _characterPosition.X > _boxPosition.X)
            {
                _isFacingRight = false;
                _characterState = CharacterState.Running;
                _boxPosition.X -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.D) && playerBoundingBox.Left <= boxBoundingBox.Right && _isBoxOnPlatform && _characterPosition.X < _boxPosition.X)
            {
                _isFacingRight = true;
                _characterState = CharacterState.Running;
                _boxPosition.X += 5;
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