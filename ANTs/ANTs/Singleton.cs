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
        
        public const int UI_TOP_HEIGHT = 50; //ขนาด UI ส่วนบน
        public const int UI_BOTTOM_HEIGHT = 50; //ขนาด UI ส่วนล่าง

        public const int GAMEWIDTH = 600; //ความกว้างจอเกม
        public const int GAMEHEIGHT = 600; //ความสูงจอเกม

        public const int SCREENWIDTH = GAMEWIDTH ; //ความกว้างจอรวม
        public const int SCREENHEIGHT = GAMEHEIGHT + UI_TOP_HEIGHT + UI_BOTTOM_HEIGHT; //ความสูงจอรวม

        public int Life = 5; //พลังชีวิต (คุกกี้)
        public int BulletCount = 10; //กระสุนเริ่มต้น
        public float SpawnInterval = 5.0f; //อัตราการเกิดของศัตรูเริ่มต้น

        public MouseState currentMouse, previousMouse;

        public enum GameState
        {
            Menu, Playing, GameOver
        }
        public GameState CurrentGameState = GameState.Menu;

        public Random Random = new Random();

        public int Score;
        public float EnemySpeed = 0.5f; // ความเร็วศัตรูเริ่มต้น

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
            //Build รูปภาพเข้าตัวแปร
            Cookie = Content.Load<Texture2D>("Cookie1"); //คุกกี้ตรงกลาง
            PlayerAnt = Content.Load<Texture2D>("playerAnt"); //มดผู้เล่น (มดปกติ)
            EnemyAnt = Content.Load<Texture2D>("enemyAnt"); //มดศัตรู
            Bullet = Content.Load<Texture2D>("bullet"); //กระสุน
            LifeCookie = Content.Load<Texture2D>("lifeCookie"); //พลังชีวิต
            BulletAnt = Content.Load<Texture2D>("bulletAnt"); //ไอคอนแสดงจำนวนกระสุนที่มี
            BombAnt = Content.Load<Texture2D>("bombAnt"); //มดระเบิด (มดพิเศษ)
            FreezeAnt = Content.Load<Texture2D>("freezeAnt"); //มดแช่แข็ง (มดพิเศษ)
            PoisonAnt = Content.Load<Texture2D>("poisonAnt"); //มดพิษ (มดพิเศษ)

            RectTexture = new Texture2D(graphicsDevice, 1, 1);
            RectTexture.SetData(new[] { Color.White });

        }

        public void Reset()
        {
            Life = 5;
            Score = 0;
            BulletCount = 10;
            EnemySpeed = 0.5f;
            SpawnInterval = 5.0f;
            CurrentGameState = GameState.Playing;

        }

        public void UpdateDifficulty()
        {
            //ทุกๆ 300 คะแนน จะเพิ่มความยาก
            int tier = Score / 300;

            //ทุกๆ 300 คะแนน จะเพิ่มความเร็วศัตรู 0.1
            float speed = 0.5f + (0.1f * tier);
            if (speed > 1.5f) speed = 1.5f;
            EnemySpeed = speed;

            //ทุกๆ 300 คะแนน จะเพิ่มอัตราการเกิดศัตรู 0.5 วิ
            float spawn = 5.0f - (tier * 0.5f);
            if (spawn < 3f) spawn = 3f;
            SpawnInterval = spawn;
        }
    }
}