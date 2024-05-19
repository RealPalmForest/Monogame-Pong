using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Pongage
{
    public static class GameUI
    {
        public static bool isScoreOnScreen;

        public static int scoreOnScreenDuration = 100;
        private static int scoreOnScreenTimer = 0;

        private static readonly int scoreOnScreenAlphaDecreasePerTick = 3;
        private static int currentScoreOnScreenAlpha = 255;


        private static Vector2 GameCenter = new Vector2(Globals.GameBounds.Width / 2, Globals.HeaderBounds.Height + Globals.GameBounds.Height / 2);

        private static Vector2 RoundTimerPosition = new Vector2(Globals.HeaderBounds.Width / 2, Globals.HeaderBounds.Height / 3);
        private static String roundStopwatchText = Globals.RoundStopwatch.Elapsed.ToString("mm\\:ss\\.ff");
        private static Vector2 roundStopwatchOrigin = Globals.GameFont.MeasureString(roundStopwatchText) / 2;

        private static Color HeaderColor = new Color(Color.Black, 0.35f);

        private static Vector2 LeftScorePosition = new Vector2(Globals.HeaderBounds.Width / 4, Globals.HeaderBounds.Height / 2);
        private static Vector2 RightScorePosition = new Vector2((Globals.HeaderBounds.Width / 4) * 3, Globals.HeaderBounds.Height / 2);
        private static Vector2 ScoreNumberOrigin = Globals.GameFont.MeasureString("0") / 2;

        private static Vector2 TextOffset = new Vector2(0, 14);

        private static Texture2D WhiteTexture = Globals.Content.Load<Texture2D>("Textures/white");

        private static Texture2D LeftControlsTexture = Globals.Content.Load<Texture2D>("UI/LeftControls");
        private static Texture2D RightControlsTexture = Globals.Content.Load<Texture2D>("UI/RightControls");

        private static Vector2 LeftControlsPosition = new Vector2(Globals.GameBounds.Width / 5, Globals.GameBounds.Y + Globals.GameBounds.Height / 2);
        private static Vector2 RightControlsPosition = new Vector2(Globals.GameBounds.Width / 5 * 4, Globals.GameBounds.Y + Globals.GameBounds.Height / 2);

        private static Vector2 LeftControlsOrigin = new Vector2(LeftControlsTexture.Width, LeftControlsTexture.Height) / 2;
        private static Vector2 RightControlsOrigin = new Vector2(RightControlsTexture.Width, RightControlsTexture.Height) / 2;

        private static Vector2 StartTextPosition = new Vector2(Globals.GameBounds.Width / 2, Globals.GameBounds.Y + Globals.GameBounds.Height * 0.85f);
        private static Vector2 StartTextOrigin = Globals.GameFont.MeasureString("Press Space to Start") / 2;


        public static void DisplayScore()
        {
            isScoreOnScreen = true;
        }


        public static void Update()
        {
            if (isScoreOnScreen)
            {
                scoreOnScreenTimer++;
                currentScoreOnScreenAlpha -= scoreOnScreenAlphaDecreasePerTick;

                if (scoreOnScreenTimer >= scoreOnScreenDuration)
                {
                    scoreOnScreenTimer = 0;
                    currentScoreOnScreenAlpha = 255;
                    isScoreOnScreen = false;
                }
            }
        }


        public static void Draw()
        {
            if (Globals.IsPaused)
            {
                DrawStart();
            }

            DrawHeader();

            if (isScoreOnScreen)
            {
                DrawScore(currentScoreOnScreenAlpha);
            }
        }


        private static void DrawScore(int alpha = 255)
        {
            string scoreText = Globals.LeftPaddle.Score + " - " + Globals.RightPaddle.Score;
            Vector2 Origin = Globals.GameFont.MeasureString(scoreText) / 2;

            Globals.SpriteBatch.DrawString(Globals.GameFont, scoreText, GameCenter + TextOffset, new Color(Color.White, alpha), 0f, Origin, 1f, SpriteEffects.None, 0f);
        }

        private static void DrawHeader()
        {
            // Background
            Globals.SpriteBatch.Draw(WhiteTexture, Globals.HeaderBounds, HeaderColor);

            // Round Stopwatch
            roundStopwatchText = Globals.RoundStopwatch.Elapsed.ToString("mm\\:ss\\.ff");
            Globals.SpriteBatch.DrawString(Globals.GameFont, roundStopwatchText, RoundTimerPosition + TextOffset, Color.White, 0f, roundStopwatchOrigin, 0.3f, SpriteEffects.None, 0f);

            // Score
            Globals.SpriteBatch.DrawString(Globals.GameFont, Globals.LeftPaddle.Score.ToString(), LeftScorePosition + TextOffset, Color.White, 0f, ScoreNumberOrigin, 0.7f, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(Globals.GameFont, Globals.RightPaddle.Score.ToString(), RightScorePosition + TextOffset, Color.White, 0f, ScoreNumberOrigin, 0.7f, SpriteEffects.None, 0f);
        }

        private static void DrawStart()
        {
            Globals.SpriteBatch.Draw(LeftControlsTexture, LeftControlsPosition, new Rectangle(0, 0, LeftControlsTexture.Width, LeftControlsTexture.Height), Color.White, 0f, LeftControlsOrigin, 0.17f, SpriteEffects.None, 0f);
            Globals.SpriteBatch.Draw(RightControlsTexture, RightControlsPosition, new Rectangle(0, 0, RightControlsTexture.Width, RightControlsTexture.Height), Color.White, 0f, RightControlsOrigin, 0.17f, SpriteEffects.None, 0f);

            Globals.SpriteBatch.DrawString(Globals.GameFont, "press SPACE to start", StartTextPosition + TextOffset, Color.White, 0f, StartTextOrigin, 0.4f, SpriteEffects.None, 0f);
        }
    }
}
