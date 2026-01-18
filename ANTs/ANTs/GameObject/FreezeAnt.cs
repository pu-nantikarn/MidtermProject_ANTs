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

        public override void Update(GameTime gameTime)
        {
            // เพิ่ม Logic แช่แข็งที่นี่
            base.Update(gameTime);
        }
    }
}
