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

        public Vector2 PositionOld { get; private set; } = Vector2.Zero;
        public Vector2 Position = Vector2.Zero;
        private Vector2 Velocity = Vector2.Zero;
        private Vector2 Origin;
        private Rectangle Bounds;

        private Point Size;

        private Rectangle TextureSourceRectangle;
        private Texture2D Texture;
        private Color Color;

        private int PaddleBounceCooldown = 10;
        private int PaddleBounceCounter = 10;

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
                    // New round
                    PaddleBounceCounter = 0;

                    waitingForNextRound = false;
                    TimerForNextRound = 0;

                    Speed = DefaultSpeed;

                    ResetPosition();
                    Globals.RightPaddle.ResetPosition();
                    Globals.LeftPaddle.ResetPosition();

                    Globals.RandomiseBackgroundColor();

                    Globals.RoundStopwatch.Restart();
                    Globals.GameStopwatch.Start();
                }
                else
                {
                    return;
                }
            }

            // End round if the ball has left the game field
            if (!Globals.GameBounds.Contains(Position))
            {
                EndRound();
            }

            if (PaddleBounceCounter > 0)
                PaddleBounceCounter--;

            //
            // Collision
            //
            Vector2 newVelocity = Vector2.Zero;
            bool collisionWithPaddle = false;
            if (Globals.LeftPaddle.GetBounds().Intersects(Bounds))
            {
                // Collided on the left paddle
                newVelocity = new Vector2(-Velocity.X, Velocity.Y);
                collisionWithPaddle = true;
            }
            else if (Globals.RightPaddle.GetBounds().Intersects(Bounds))
            {
                // Collided on the right paddle
                newVelocity = new Vector2(-Velocity.X, Velocity.Y);
                collisionWithPaddle = true;
            }
            else if(Position.Y - Origin.Y < Globals.GameBounds.Y ||
                Position.Y + Origin.Y > Globals.GameBounds.Y + Globals.GameBounds.Height)
            {
                // Collided with the top or bottom wall
                newVelocity = new Vector2(Velocity.X, -Velocity.Y);
            }


            PositionOld = Position;

            // No collision
            if (newVelocity == Vector2.Zero)
            { 
                MoveWithVelocity(Vector2.Normalize(Velocity));
            }

            // If a collision happened and velocity needs to be changed
            else
            {
                // Move with the new velocity
                MoveWithVelocity(Vector2.Normalize(newVelocity));

                // Increase ball speed with every bounce
                Speed += SpeedIncreaseEachBounce;

                //Debug.WriteLine($"Old Velocity: {Velocity}, New Velocity: {newVelocity}");
                Velocity = newVelocity;


                if (PaddleBounceCounter > 0)
                {
                    if(collisionWithPaddle)
                    {
                        EndRound();
                        return;
                    }  
                }
                else
                {
                    if(collisionWithPaddle)
                        PaddleBounceCounter = PaddleBounceCooldown;
                }

                Globals.PlaySound(Sound.Hit);
            }
        }

        public void MoveWithVelocity(Vector2 velocity)
        {
            Position += velocity * Speed;

            UpdateBounds();
        }

        
        public bool IsBehindPaddles(Vector2 position)
        {
            if (position.X - Origin.X < Globals.LeftPaddle.Position.X - Globals.LeftPaddle.GetOrigin().X)
            {
                // Behind the left paddle
                return true;
            }
            else if (position.X + Origin.X > Globals.RightPaddle.Position.X + Globals.RightPaddle.GetOrigin().X)
            {
                // Behind the right paddle
                return true;
            }

            return false;
        }


        public void EndRound()
        {
            Globals.RoundStopwatch.Stop();
            Globals.GameStopwatch.Stop();

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

            //Debug.WriteLine($"Score is now: {Globals.LeftPaddle.Score} - {Globals.RightPaddle.Score}");

            waitingForNextRound = true;
            GameUI.DisplayScore();

            Globals.PlaySound(Sound.RoundEnd);
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
            PositionOld = Position;
            Position = StartPosition;
            Position.Y = Globals.Random.Next(Globals.HeaderBounds.Height + Globals.GameBounds.Y + Bounds.Height, Globals.GameBounds.Height - Bounds.Height);

            UpdateBounds();

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


        public void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, new Rectangle((PositionOld - Origin * 0.8f).ToPoint(), (Size.ToVector2() * 0.8f).ToPoint()), TextureSourceRectangle, new Color(Color.White, 0.4f), 0f, Vector2.Zero, SpriteEffects.None, 0f);
            Globals.SpriteBatch.Draw(Texture, Bounds, TextureSourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
