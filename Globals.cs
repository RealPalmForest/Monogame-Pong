using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pongage
{
    public enum Sound
    {
        Hit,
        RoundEnd
    }

    public static class Globals
    {
        public static SoundEffect Hit;
        public static SoundEffect RoundEnd;


        public static Random Random = new Random();

        public static Stopwatch GameStopwatch = new Stopwatch();
        public static Stopwatch RoundStopwatch = new Stopwatch();

        public static Paddle LeftPaddle {  get; set; }
        public static Paddle RightPaddle { get; set; }

        public static Rectangle WindowBounds {  get; set; }
        public static Rectangle GameBounds { get; set; }
        public static Rectangle HeaderBounds { get; set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static SpriteFont GameFont { get; set; }


        public static Color BackgroundColor { get; set; }

        public static bool DebugMode { get; set; }


        public static void RandomiseBackgroundColor()
        {
            BackgroundColor = new Color(Globals.Random.Next(150), Globals.Random.Next(150), Globals.Random.Next(150));
        }

        public static void PlaySound(Sound sound)
        {
            switch (sound)
            {
                case Sound.Hit:
                    Hit.Play(0.1f, default, default);
                    break;
                case Sound.RoundEnd:
                    RoundEnd.Play(0.1f, default, default);
                    break;
            }
        }
    }
}
