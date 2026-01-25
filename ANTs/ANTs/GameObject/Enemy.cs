using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTs
{
    public class Enemy : GameObject
    {
        private Vector2 _direction;
        public int RowId; // ใช้ระบุกลุ่มของแถว
        private float freezeTimer = 0f;
        public bool IsFrozen = false;
        private static float _spawnTimer = 0f;
        private static int _rowCounter = 0;

        private static readonly Vector2 _cookieCenter = new Vector2(Singleton.GAMEWIDTH / 2f, 50 + (Singleton.GAMEHEIGHT / 2f));
        private const float COOKIE_RADIUS = 60f;

        public Enemy(Texture2D texture, Vector2 startPosition, Vector2 directionVector, int rowId) : base(texture)
        {
            Position = startPosition;
            RowId = rowId;
            _direction = directionVector;
            if (_direction != Vector2.Zero)
                _direction.Normalize(); //ปรับทิศทางการเคลื่อนที่ให้คงที่

            // หมุนหน้าไปตามทิศทางที่วิ่ง
            Rotation = (float)Math.Atan2(_direction.Y, _direction.X);
        }

        public override void Update(GameTime gameTime)
        {
            //เช็คสถานะแช่แข็ง
            if (IsFrozen)
            {
                freezeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (freezeTimer <= 0)
                    IsFrozen = false;

                return;
            }

            if (!IsActive) return;

            Position += _direction * Singleton.Instance.EnemySpeed;

            // เช็คการชนคุกกี้ 
            if (Vector2.Distance(Position, _cookieCenter) < COOKIE_RADIUS)
            {
                IsActive = false;
            }
        }

        //อัตราการเกิดศัตรู
        public static void SpawnTimer(GameTime gameTime, List<Enemy> enemies)
        {
            _spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_spawnTimer >= Singleton.Instance.SpawnInterval)
            {
                _rowCounter++;
                List<Enemy> spawnAntEnemy = Enemy.SpawnEnemy(_rowCounter, Singleton.Instance.EnemyAnt);
                enemies.AddRange(spawnAntEnemy);
                _spawnTimer = 0f;

            }
        }
        public static void ResetSpawner()
        {
            _spawnTimer = 0f;
            _rowCounter = 0;
        }

        public static List<Enemy> SpawnEnemy(int rowCounter, Texture2D texture)
        {
            List<Enemy> _enemySpawn = new List<Enemy>();
            int direction = Singleton.Instance.Random.Next(8); //สุ่มเกิด
            Vector2 basePos = Vector2.Zero;
            Vector2 offset = Vector2.Zero;
            Vector2 moveDirection = Vector2.Zero;

            // จุดกึ่งกลางหน้าจอเกม (เป้าหมาย)
            float centerX = Singleton.GAMEWIDTH / 2f;
            float centerY = Singleton.UI_TOP_HEIGHT + (Singleton.GAMEHEIGHT / 2f);


            switch (direction)
            {
                case 0: //ศัตรูเกิดด้านบน (มาเป็นแถวนอน)
                    basePos = new Vector2(centerX, Singleton.UI_TOP_HEIGHT);
                    offset = new Vector2(30, 0);
                    moveDirection = new Vector2(0, 1);
                    break;
                case 1: //ศัตรูเกิดด้านล่าง (มาเป็นแถวนอน)
                    basePos = new Vector2(centerX, Singleton.SCREENHEIGHT - 50);
                    offset = new Vector2(30, 0);
                    moveDirection = new Vector2(0, -1);
                    break;
                case 2: //ศัตรูเกิดด้านซ้าย (มาเป็นแถวตั้ง)
                    basePos = new Vector2(0, centerY);
                    offset = new Vector2(0, 30);
                    moveDirection = new Vector2(1, 0);
                    break;
                case 3: //ศัตรูเกิดด้านขวา (มาเป็นแถวตั้ง)
                    basePos = new Vector2(Singleton.SCREENWIDTH - 50, centerY);
                    offset = new Vector2(0, 30);
                    moveDirection = new Vector2(-1, 0);
                    break;
                default: //ศัตรูเกิดทิศเฉียง (ซ้ายบน, ซ้ายล่าง, ขวาบน, ขวาล่าง)
                    basePos = new Vector2(direction == 4 || direction == 6 ? 0 : 600,
                                         direction == 4 || direction == 5 ? 50 : 650);
                    moveDirection = new Vector2(centerX, centerY) - basePos;
                    moveDirection.Normalize();
                    offset = new Vector2(-moveDirection.Y, moveDirection.X) * 30;
                    break;
            }

            // สร้างศัตรู 3 ตัวเรียงตามแนว offset
            for (int i = -1; i <= 1; i++)
            {
                _enemySpawn.Add(new Enemy(Singleton.Instance.EnemyAnt, basePos + (offset * i), moveDirection, rowCounter));
            }
            return _enemySpawn;
        }

        //Check สถานะศัตรูว่าถูกแช่แข็งหรือไม่
        public void EnemyFreeze(float duration)
        {
            IsFrozen = true;
            freezeTimer = duration;
        }

        public static void CheckEnemyCollision(List<Enemy> enemies, GameTime gameTime)
        {
            // ใช้ for loop ถอยหลังตามเดิมเพื่อความปลอดภัย
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                // ตรวจสอบว่ามดตัวนี้ชนคุกกี้หรือไม่ 
                if (!enemies[i].IsActive)
                {
                    int currentRowId = enemies[i].RowId;

                    // ลบมดทุกตัวที่มี RowId เดียวกัน (หายไปทั้งแถว) เมื่อชนคุกกี้
                    enemies.RemoveAll(e => e.RowId == currentRowId);

                    // ลดชีวิต 1 แต้มต่อ 1 แถว
                    Singleton.Instance.Life--;
                    Singleton.Instance.BulletCount += 3; //เพิ่มกรุะสุน

                    break;
                }
            }
        }
    }
}
