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
            Scale = new Vector2(1.2f, 1.2f); // ปรับขนาดให้ต่างจากมดปกติเล็กน้อย
        }

        public override void Update(GameTime gameTime)
        {
            // เพิ่ม Logic พิษที่นี่ เช่น เช็คศัตรูรอบข้าง
            base.Update(gameTime);
        }
    }
}
