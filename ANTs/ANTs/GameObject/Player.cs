using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ANTs
{
    public class Player : GameObject
    {
        private float _radius = 60f;
        private Vector2 _center = new Vector2(Singleton.GAMEWIDTH / 2f, Singleton.UI_TOP_HEIGHT + (Singleton.GAMEHEIGHT / 2f));

        public Player(Texture2D texture) : base(texture)
        {
        }

        public override void Update(GameTime gameTime)
        {
            int bulletCount = Singleton.Instance.BulletCount;
            float newScale = 1.0f;

            if (bulletCount >= 25)
            {
                newScale = 1.0f + (bulletCount - 25) * 0.02f;
                if (newScale > 2f) newScale = 2f;
                Scale = new Vector2(newScale, newScale);
            }
            else
            {
                Scale = Vector2.One;
            }

            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

            float minSpeed = 0.03f;
            float maxSpeed = 0.15f;
            // คำนวณมุมระหว่างเมาส์กับจุดศูนย์กลางคุกกี้
            float playerRotation = (float)Math.Atan2(mousePos.Y - _center.Y, mousePos.X - _center.X);
            float rotationSpeed = maxSpeed / (float)Math.Pow(Scale.X, 3);

            if (rotationSpeed < minSpeed) rotationSpeed = minSpeed;
            Rotation = CustomLerpAngle(Rotation, playerRotation, rotationSpeed);
            // ตำแหน่งของมดจะอยู่บนเส้นรอบวงของวงกลมรอบคุกกี้
            Position.X = _center.X + (float)Math.Cos(Rotation) * _radius;
            Position.Y = _center.Y + (float)Math.Sin(Rotation) * _radius;
        }

        public Bullet Fire()
        {
            if (Singleton.Instance.currentMouse.LeftButton == ButtonState.Pressed &&
                Singleton.Instance.previousMouse.LeftButton == ButtonState.Released)
            {
                if (Singleton.Instance.BulletCount > 0)
                {
                    Singleton.Instance.BulletCount--;
                    return new Bullet(Singleton.Instance.Bullet, this.Position, this.Rotation);
                }
            }
            return null;
        }

        // ฟังก์ชันช่วยจัดการมุมให้อยู่ในระยะ -PI ถึง PI
        private float WrapAngle(float angle)
        {
            while (angle > MathHelper.Pi) angle -= MathHelper.TwoPi;
            while (angle < -MathHelper.Pi) angle += MathHelper.TwoPi;
            return angle;
        }

        // ฟังก์ชันสำหรับ Lerp มุมโดยเฉพาะ
        private float CustomLerpAngle(float current, float target, float speed)
        {
            float diff = WrapAngle(target - current);
            return current + (diff * speed);
        }
        public void SetTexture(Texture2D newTexture)
        {
            _texture = newTexture;
        }

    }
}
