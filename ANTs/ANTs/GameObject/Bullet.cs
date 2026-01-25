using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTs
{
    public class Bullet : GameObject
    {
        private float _speed = 5f; //ความเร็วกระสุน
        private Vector2 _velocity;
        public bool IsPoisonBullet = false;
        public int PierceCount = 10; //จำนวนการทะลุลง
        public bool IsFreezeBullet = false;
        public bool IsBombBullet = false;
         

        public Bullet(Texture2D texture, Vector2 startPos, float angle) : base(texture)
        {
            Position = startPos;
            Rotation = angle;
            // คำนวณทิศทางการวิ่งจากมุม
            _velocity = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * _speed;
        }

        public override void Update(GameTime gameTime)
        {
            Position += _velocity;

            // ลบกระสุนเมื่อออกนอกจอเกม
            if (Position.X < 0 || Position.X > Singleton.GAMEWIDTH ||
                Position.Y < Singleton.UI_TOP_HEIGHT || Position.Y > Singleton.UI_TOP_HEIGHT + Singleton.GAMEHEIGHT)
            {
                IsActive = false;
            }
        }

        public bool CheckBulletCollision(List<Enemy> enemies)
        {
            bool bulletHit = false;

            for (int j = enemies.Count - 1; j >= 0; j--)
            {
                if (this.Rectangle.Intersects(enemies[j].Rectangle))
                {
                    Enemy hitEnemy = enemies[j];

                    //กระสุน BombAnt ชนศัตรู
                    if (this.IsBombBullet)
                    {
                        BombAnt.Explode(enemies, hitEnemy.Position);

                        bulletHit = true;
                        break;
                    }

                    //กระสุน FreezeAnt ชนศัตรู
                    if (this.IsFreezeBullet)
                    {
                        int targetRow = enemies[j].RowId;
                        FreezeAnt.Freeze(enemies, targetRow);
                        bulletHit = true;
                        break;
                    }

                    //กระสุน PoisonAnt ชนศัตรู (ยิงทะลุแถว)
                    if (this.IsPoisonBullet)
                    {
                        PoisonAnt.PoisonHit(enemies, j);

                        this.PierceCount--; // ลดจำนวนการทะลุลง

                        if (this.PierceCount <= 0)
                        {
                            bulletHit = true;
                            break; // เมื่อกระสุนหมดแรงทะลุ ให้หยุดการตรวจเช็คศัตรูตัวถัดไปในเฟรมนี้
                        }
                        // ไม่ break เพื่อให้กระสุนทะลุไปตัวถัดไป
                        continue;
                    }

                    // กระสุนปกติ 
                    enemies.RemoveAt(j);
                    Singleton.Instance.Score += 10;
                    Singleton.Instance.BulletCount += 3;

                    bulletHit = true;
                    break;
                }
            }
            return bulletHit;
        }
    }
}
