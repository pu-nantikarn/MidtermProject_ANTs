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
        private float _speed = 5f;
        private Vector2 _velocity;
        public bool CanPierce = false;
        public int PierceCount = 0;
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
    }
}
