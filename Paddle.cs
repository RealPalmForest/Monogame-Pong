using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pongage
{
    public class Paddle
    {
        public int Score = 0;

        public float moveSpeed = 2.3f;

        public Color Color;
        private Point Size;
        private Texture2D Texture;
        private Rectangle TextureSourceRectangle;

        public Vector2 Position;
        private Vector2 Origin;
        private Rectangle Bounds;
        private float distanceFromEdge;

        public Rectangle GetSourceRectangle() { return TextureSourceRectangle; }
        public Texture2D GetTexture() { return Texture; }
        public Vector2 GetOrigin() { return Origin; }
        public Rectangle GetBounds() { return Bounds; }
        public Point GetSize() { return Size; }
        public float GetDistanceFromEdge() { return distanceFromEdge; }
        

        public Paddle(Vector2 position, Color color, Point size)
        {
            Position = position;
            Color = color;

            SetSize(size);
            SetTexture(Globals.Content.Load<Texture2D>("Textures/Paddle"));
            SetDistanceFromEdge(Globals.Random.Next(50, Globals.GameBounds.Width - 50));
        }


        //
        // Set Functions
        //
        public void SetTexture(Texture2D texture)
        {
            Texture = texture;
            TextureSourceRectangle = new Rectangle(
                new Point(0, 0),
                new Point(texture.Width, texture.Height));
        }
        public void SetDistanceFromEdge(float distanceFromEdge)
        {
            this.distanceFromEdge = distanceFromEdge;
            Position.X = distanceFromEdge;
        }
        public void SetSize(Point size)
        {
            Size = size;
            Origin = size.ToVector2() / 2;
            UpdateBounds();
        }




        public void MoveVertically(float verticalVelocity)
        {
            Position += (new Vector2(0f, verticalVelocity) * moveSpeed);

            if(CheckIfOffscreen())
            {
                Position -= (new Vector2(0f, verticalVelocity) * moveSpeed);
            }

            UpdateBounds();
        }

        public void ResetPosition()
        {
            Position.Y = Globals.HeaderBounds.Height + Globals.GameBounds.Height / 2;
        }

        public void UpdateBounds()
        {
            Bounds = new Rectangle((Position - Origin).ToPoint(), Size);
        }

        public void Draw()
        {
            //spriteBatch.Draw(Globals.Content.Load<Texture2D>("Textures/debugRed"), Bounds, Color.White);

            Globals.SpriteBatch.Draw(Texture, Bounds, TextureSourceRectangle, Color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Legacy System dependent on texture instead of size
            //spriteBatch.Draw(Texture, Position, TextureSourceRectangle, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);
        }

        public bool CheckIfOffscreen()
        {
            if (Position.Y - Origin.Y < Globals.GameBounds.Y || Position.Y + Origin.Y > Globals.GameBounds.Y + Globals.GameBounds.Height)
                return true;

            return false;
        }
    }
}
