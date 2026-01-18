using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTs
{
    public class FreezeAnt : GameObject
    {
        public float SlowRate = 0.5f; // ลดความเร็วศัตรูลง 50%

        public FreezeAnt(Texture2D texture, Vector2 position) : base(texture)
        {
            Position = position;
        }

        public float FreezeDuration = 5f; // เวลาแช่แข็ง (วินาที)

        public override void Update(GameTime gameTime)
        {
            // FreezeAnt ใช้กระสุนแช่แข็ง ศัตรูจะถูกหยุดชั่วคราว
            base.Update(gameTime);
        }
    }
}
