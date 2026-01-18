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
        public float ExplosionRadius = 150f;

        public BombAnt(Texture2D texture, Vector2 position) : base(texture)
        {
            Position = position;
        }

        public override void Update(GameTime gameTime)
        {
            // เพิ่ม Logic การระเบิด
            base.Update(gameTime);
        }

        public void Explode()
        {
            // สั่งทำลายศัตรูในรัศมี
        }
    }
}
