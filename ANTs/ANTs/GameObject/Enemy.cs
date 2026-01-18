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
        private float _speed = 0.3f; // ปรับความเร็วตามต้องการ
        private Vector2 _velocity;
        public int RowId; // ใช้ระบุกลุ่มของแถว

        // รับ directionVector และ rowId เพิ่มเข้ามา
        public Enemy(Texture2D texture, Vector2 startPosition, Vector2 directionVector, int rowId) : base(texture)
        {
            Position = startPosition;
            RowId = rowId;
            _velocity = directionVector * _speed;

            // หมุนหน้าไปตามทิศทางที่วิ่ง
            Rotation = (float)Math.Atan2(_velocity.Y, _velocity.X);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            // เคลื่อนที่ไปข้างหน้าตามทิศทางของแถว (รักษารูปขบวน)
            Position += _velocity;

            // เช็คการชนคุกกี้ (ถ้าตัวนี้ชน จะส่งสัญญาณให้ MainScene ลบทั้ง RowId นี้)
            Vector2 center = new Vector2(Singleton.GAMEWIDTH / 2f, 50 + (Singleton.GAMEHEIGHT / 2f));
            if (Vector2.Distance(Position, center) < 60f)
            {
                // ตัวนี้ชนแล้ว! เราจะจัดการลบทั้งแถวใน MainScene.Update
                IsActive = false;
                // หมายเหตุ: การลด Life จะไปทำใน MainScene เพื่อไม่ให้ลดซ้ำซ้อน
            }
        }

        public static List<Enemy> SpawnEnemy(int rowCounter, Texture2D texture)
        {
            List<Enemy> _enemySpawn = new List<Enemy>();
            int direction = Singleton.Instance.Random.Next(8);
            Vector2 basePos = Vector2.Zero;
            Vector2 offset = Vector2.Zero;
            Vector2 moveDirection = Vector2.Zero;

            // จุดกึ่งกลางหน้าจอเกม (เป้าหมาย)
            float centerX = Singleton.GAMEWIDTH / 2f;
            float centerY = Singleton.UI_TOP_HEIGHT + (Singleton.GAMEHEIGHT / 2f);
            

            switch (direction)
            {
                case 0: // บน (มาเป็นแถวนอน)
                    basePos = new Vector2(centerX, Singleton.UI_TOP_HEIGHT);
                    offset = new Vector2(30, 0);
                    moveDirection = new Vector2(0, 1);
                    break;
                case 1: // ล่าง (มาเป็นแถวนอน)
                    basePos = new Vector2(centerX, Singleton.SCREENHEIGHT - 50);
                    offset = new Vector2(30, 0);
                    moveDirection = new Vector2(0, -1);
                    break;
                case 2: // ซ้าย (มาเป็นแถวตั้ง)
                    basePos = new Vector2(0, centerY);
                    offset = new Vector2(0, 30);
                    moveDirection = new Vector2(1, 0);
                    break;
                case 3: // ขวา (มาเป็นแถวตั้ง)
                    basePos = new Vector2(Singleton.SCREENWIDTH - 50, centerY);
                    offset = new Vector2(0, 30);
                    moveDirection = new Vector2(-1, 0);
                    break;
                default: // ทิศเฉียง (4-7)
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
    }
}
