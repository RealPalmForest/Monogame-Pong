using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pongage
{
    internal class Ball
    {
        public int DelayBeforeNextRound = 100;
        private int TimerForNextRound = 0;
        public bool waitingForNextRound = false;

        public Vector2 StartPosition;

        public float DefaultSpeed = 3.2f;
        public float Speed = 3.2f;
        public float SpeedIncreaseEachBounce = 0.07f;

        public Vector2 Position;
        private Vector2 Velocity;
        private Vector2 Origin;
        private Rectangle Bounds;

        private Point Size;

        private Rectangle TextureSourceRectangle;
        private Texture2D Texture;
        private Color Color;

        public Ball(Vector2 position, Texture2D texture, Point size)
        {
            Speed = DefaultSpeed;

            StartPosition = position;
            SetTexture(texture);
            SetSize(size);

            ResetPosition();
        }

        public void Update()
        {
            if(waitingForNextRound)
            {
                TimerForNextRound++;

                if(TimerForNextRound >= DelayBeforeNextRound)
                {
                    waitingForNextRound = false;
                    ResetPosition();
                    TimerForNextRound = 0;

                    Speed = DefaultSpeed;

                    Globals.RightPaddle.ResetPosition();
                    Globals.LeftPaddle.ResetPosition();

                    Globals.RandomiseBackgroundColor();

                    Globals.RoundStopwatch.Restart();
                }
                else
                {
                    return;
                }
            }


            if (Position.X - Origin.X < Globals.GameBounds.X || Position.X + Origin.X > Globals.GameBounds.Width)
                //Globals.LeftPaddle.GetBounds().Contains(new Vector2(Position.X, Position.Y + Origin.Y)) ||
                //Globals.LeftPaddle.GetBounds().Contains(new Vector2(Position.X, Position.Y - Origin.Y)) ||
                //Globals.RightPaddle.GetBounds().Contains(new Vector2(Position.X, Position.Y + Origin.Y)) ||
                //Globals.RightPaddle.GetBounds().Contains(new Vector2(Position.X, Position.Y - Origin.Y)))
            {
                EndRound();
                Globals.PlaySound(Sound.RoundEnd);
            }


            Vector2 newVelocity = Vector2.One;
            if (Globals.LeftPaddle.GetBounds().Contains(new Vector2(Position.X - Origin.X, Position.Y)) &&
                Position.Y < Globals.LeftPaddle.GetBounds().Y + Globals.LeftPaddle.GetBounds().Height &&
                Position.Y > Globals.LeftPaddle.GetBounds().Y)
            {
                // Collided on the left paddle
                newVelocity = new Vector2(-Velocity.X, Velocity.Y);
            }
            else if (Globals.RightPaddle.GetBounds().Contains(new Vector2(Position.X + Origin.X, Position.Y)) &&
                Position.Y < Globals.RightPaddle.GetBounds().Y + Globals.RightPaddle.GetBounds().Height &&
                Position.Y > Globals.RightPaddle.GetBounds().Y)
            {
                // Collided on the right paddle
                newVelocity = new Vector2(-Velocity.X, Velocity.Y);
            }
            else if(Position.Y - Origin.Y < Globals.GameBounds.Y ||
                Position.Y + Origin.Y > Globals.GameBounds.Y + Globals.GameBounds.Height)
            {
                // Collided with the top or bottom wall
                newVelocity = new Vector2(Velocity.X, -Velocity.Y);
            }
            else
            {
                MoveWithVelocity(Vector2.Normalize(Velocity));
                return;
            }

            Speed += SpeedIncreaseEachBounce;
            MoveWithVelocity(Vector2.Normalize(newVelocity));


            if (CollidingWithPaddles())
            {
                EndRound();
                Globals.PlaySound(Sound.RoundEnd);
            }
            else
            {
                Globals.PlaySound(Sound.Hit);
            }

            Debug.WriteLine($"Old Velocity: {Velocity}, New Velocity: {newVelocity}");
            Velocity = newVelocity;
        }

        public void MoveWithVelocity(Vector2 velocity)
        {
            Position += velocity * Speed;

            UpdateBounds();
        }

        public bool CollidingWithPaddles()
        {
            if (Globals.LeftPaddle.GetBounds().Contains(new Vector2(Position.X - Origin.X, Position.Y)))
            {
                // Collided on the left paddle
                return true;
            }
            else if (Globals.RightPaddle.GetBounds().Contains(new Vector2(Position.X + Origin.X, Position.Y)))
            {
                // Collided on the right paddle
                return true;
            }

            return false;
        }

        public bool CollidingWithAnyting()
        {
            if (Globals.LeftPaddle.GetBounds().Contains(new Vector2(Position.X - Origin.X, Position.Y)))
            {
                // Collided on the left paddle
            }
            else if (Globals.RightPaddle.GetBounds().Contains(new Vector2(Position.X + Origin.X, Position.Y)))
            {
                // Collided on the right paddle
            }
            else if (Position.Y - Origin.Y < Globals.GameBounds.Y ||
                Position.Y + Origin.Y > Globals.GameBounds.Y + Globals.GameBounds.Height ||
                Globals.LeftPaddle.GetBounds().Contains(new Vector2(Position.X, Position.Y + Origin.Y)) ||
                Globals.LeftPaddle.GetBounds().Contains(new Vector2(Position.X, Position.Y - Origin.Y)) ||
                Globals.RightPaddle.GetBounds().Contains(new Vector2(Position.X, Position.Y + Origin.Y)) ||
                Globals.RightPaddle.GetBounds().Contains(new Vector2(Position.X, Position.Y - Origin.Y)))
            {
                // Collided with the top or bottom wall
            }
            else
            {
                return false;
            }

            return true;
        }


        public void EndRound()
        {
            Globals.RoundStopwatch.Stop();

            if (Position.X < Globals.GameBounds.Width / 2)
            {
                // Ball is on the left side
                Globals.RightPaddle.Score++;
            }
            else
            {
                // Ball is on the right side
                Globals.LeftPaddle.Score++;
            }


            Debug.WriteLine($"Score is now: {Globals.LeftPaddle.Score} - {Globals.RightPaddle.Score}");

            waitingForNextRound = true;
            GameUI.DisplayScore();
        }


        public void SetTexture(Texture2D texture)
        {
            Texture = texture;
            TextureSourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void SetSize(Point size)
        {
            Size = size;
            Origin = size.ToVector2() / 2;
            UpdateBounds();
        }

        public void UpdateBounds()
        {
            Bounds = new Rectangle((Position - Origin).ToPoint(), Size);
        }


        public void ResetPosition()
        {
            Position = StartPosition;
            Position.Y = Globals.Random.Next(Globals.HeaderBounds.Height + Globals.GameBounds.Y + Bounds.Height, Globals.GameBounds.Height - Bounds.Height);

            int rand = Globals.Random.Next(4);
            if (rand == 0)
                Velocity = new Vector2(1, 1);
            else if (rand == 1)
                Velocity = new Vector2(1, -1);
            else if (rand == 2)
                Velocity = new Vector2(-1, 1);
            else if (rand == 3)
                Velocity = new Vector2(-1, -1);
        }


        public bool CheckIfOffscreen()
        {
            if (Position.Y - Origin.Y < Globals.GameBounds.Y || Position.Y + Origin.Y > Globals.GameBounds.Height)
                return true;

            return false;
        }


        public void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Bounds, TextureSourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
