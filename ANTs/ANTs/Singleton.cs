using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ANTs
{
    class Singleton
    {
        private static Singleton instance;

        public Texture2D RectTexture, Cookie, PlayerAnt, EnemyAnt, Bullet, BulletAnt, LifeCookie, BombAnt, FreezeAnt, PoisonAnt;
        

        public const int UI_TOP_HEIGHT = 50;
        public const int UI_BOTTOM_HEIGHT = 50;

        public const int GAMEWIDTH = 600;
        public const int GAMEHEIGHT = 600;

        public const int SCREENWIDTH = GAMEWIDTH ;
        public const int SCREENHEIGHT = GAMEHEIGHT + UI_TOP_HEIGHT + UI_BOTTOM_HEIGHT;

        public int Life = 5;
        public int BulletCount = 10;
        //public bool IsGameOver = false;

        public MouseState currentMouse, previousMouse;

        public enum GameState
        {
            Menu, Playing, GameOver
        }
        public GameState CurrentGameState = GameState.Menu;

        public Random Random = new Random();

        public int Score;
        public int Wave;
        public float EnemySpeed = 1.0f; // ความเร็วศัตรู (เป็นค่าเริ่มต้น)

        private Singleton()
        {

        }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content, GraphicsDevice graphicsDevice)
        {
            Cookie = Content.Load<Texture2D>("Cookie1");
            PlayerAnt = Content.Load<Texture2D>("playerAnt");
            EnemyAnt = Content.Load<Texture2D>("enemyAnt");
            Bullet = Content.Load<Texture2D>("bullet");
            LifeCookie = Content.Load<Texture2D>("lifeCookie");
            BulletAnt = Content.Load<Texture2D>("bulletAnt");
            BombAnt = Content.Load<Texture2D>("bombAnt");
            FreezeAnt = Content.Load<Texture2D>("freezeAnt");
            PoisonAnt = Content.Load<Texture2D>("poisonAnt");

            RectTexture = new Texture2D(graphicsDevice, 1, 1);
            RectTexture.SetData(new[] { Color.White });

            
        }

        public void Reset()
        {
            Life = 5;
            Score = 0;
            BulletCount = 10;
            EnemySpeed = 1.0f; 
            //IsGameOver = false;
            CurrentGameState = GameState.Playing;
        }
    }
}