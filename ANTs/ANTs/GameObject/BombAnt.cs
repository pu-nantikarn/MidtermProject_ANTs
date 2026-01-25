using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTs
{
    public class BombAnt : GameObject
    {
        public static float ExplosionRadius = 100f; // ระยะระเบิด

        public BombAnt(Texture2D texture, Vector2 position) : base(texture)
        {
            Position = position;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        //ความสามารถของ BombAnt (ระเบิดศัตรู)
        public static void Explode(List<Enemy> enemies, Vector2 explosionCenter)
        {
            //วนลูปตรวจสอบศัตรูทั้งหมดในฉาก
            for (int k = enemies.Count - 1; k >= 0; k--)
            {
                if (Vector2.Distance(enemies[k].Position, explosionCenter) <= ExplosionRadius)
                {
                    enemies.RemoveAt(k); //กำจัดศัตรู
                    Singleton.Instance.Score += 10; // เพิ่มคะแนน
                    Singleton.Instance.BulletCount += 3; //เพิ่มกระสุน
                }
            }
        }
    }
}
