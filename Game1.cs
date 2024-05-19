using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pongage.Managers;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Pongage
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Paddle Player1;
        private Paddle Player2;

        private Ball Ball;

        private Point PaddleSize;

        private Color PaddleColor;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Globals.KeyboardState = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Globals.GameStopwatch.Start();
            Globals.RoundStopwatch.Start();

            Globals.SpriteBatch = _spriteBatch;
            Globals.Content = Content;
            Globals.WindowBounds = GraphicsDevice.PresentationParameters.Bounds;
            Globals.HeaderBounds = new Rectangle(0, 0, Globals.WindowBounds.Width, 80);
            Globals.GameBounds = new Rectangle(0, Globals.HeaderBounds.Height, Globals.WindowBounds.Width, Globals.WindowBounds.Height - 80);
            Globals.GameFont = Content.Load<SpriteFont>("UI/Font");

            Globals.Hit = Content.Load<SoundEffect>("Audio/Hit");
            Globals.RoundEnd = Content.Load<SoundEffect>("Audio/RoundEnd");


            PaddleSize = new Point(17, 130);
            PaddleColor = Color.White;

            Globals.RandomiseBackgroundColor();


            Ball = new Ball(
                new Vector2(Globals.GameBounds.Width / 2, Globals.GameBounds.Height / 2),
                Content.Load<Texture2D>("Textures/Ball"),
                new Point(17, 17));


            Player1 = new Paddle(Vector2.Zero, PaddleColor, PaddleSize);
            Player1.SetDistanceFromEdge(20);
            Player1.ResetPosition();
            
            Player2 = new Paddle(Vector2.Zero, PaddleColor, PaddleSize);
            Player2.SetDistanceFromEdge(Globals.GameBounds.Width - 20);
            Player2.ResetPosition();

            Globals.LeftPaddle = Player1;
            Globals.RightPaddle = Player2;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.OldKeyboardState = Globals.KeyboardState;
            Globals.KeyboardState = Keyboard.GetState();

            base.Update(gameTime);

            if(Globals.KeyboardState.IsKeyDown(Keys.Space) && Globals.OldKeyboardState.IsKeyUp(Keys.Space))
            {
                Globals.IsPaused = !Globals.IsPaused;
            }

            if(Globals.IsPaused)
            {
                if(Globals.GameStopwatch.IsRunning)
                    TogglePauseGame();

                return;
            }
            else
            {
                if (!Globals.GameStopwatch.IsRunning)
                    TogglePauseGame();
            }


            if (!IsActive)
            {
                if (Globals.GameStopwatch.IsRunning)
                    TogglePauseGame();

                Globals.IsPaused = true;

                return;
            }
            else
            {
                if (!Globals.GameStopwatch.IsRunning)
                    TogglePauseGame();
            }


            // Get Player input
            InputManager.Update();

            GameUI.Update();

            // Move Players
            if (!Ball.waitingForNextRound)
            {
                Player1.MoveVertically(InputManager.PlayerOneVerticalVelocity);
                Player2.MoveVertically(InputManager.PlayerTwoVerticalVelocity);
            }

            Ball.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Globals.BackgroundColor);

            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);

            //_spriteBatch.Draw(Content.Load<Texture2D>("Textures/debugRed"), Globals.GameBounds, Color.White);

            Player1.Draw();
            Player2.Draw();

            Ball.Draw();

            GameUI.Draw();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void TogglePauseGame()
        {
            if (Globals.GameStopwatch.IsRunning)
                Globals.GameStopwatch.Stop();
            else
                Globals.GameStopwatch.Start();


            if (Globals.RoundStopwatch.IsRunning)
                Globals.RoundStopwatch.Stop();
            else
                Globals.RoundStopwatch.Start();
        }
    }
}
