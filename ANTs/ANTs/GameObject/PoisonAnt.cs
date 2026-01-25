using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTs
{
    public class PoisonAnt : GameObject
    {
        public int DamagePerSecond = 2;

        public PoisonAnt(Texture2D texture, Vector2 position) : base(texture)
        {
            Position = position;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        //ความสามารถของ PoisonAnt ยิงกระสุนทะลุศัตรู
        public static void PoisonHit(List<Enemy> enemies, int index)
        {
            enemies.RemoveAt(index);// กำจัดศัตรูตัวที่ระบุ
            Singleton.Instance.Score += 10; //เพิ่มคะแนน
            Singleton.Instance.BulletCount += 3; //เพิ่มกระสุน
        }
    }
}
