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
        private float _radius = 60f; //ระยะห่างจากจุดศูนย์กลาง
        private Vector2 _center = new Vector2(Singleton.GAMEWIDTH / 2f, Singleton.UI_TOP_HEIGHT + (Singleton.GAMEHEIGHT / 2f));
        private int _antUseCount = 0; // จำนวนมดพิเศษที่เหลืออยู่


        public Player(Texture2D texture) : base(texture)
        {
        }

        public override void Update(GameTime gameTime)
        {
            //---คำนวณขนาดตัวละคร----
            int bulletCount = Singleton.Instance.BulletCount;
            float newScale = 1.0f;

            if (bulletCount >= 25) //กระสุนเยอะ ขนาดตัวจะขยายใหญ่ขึ้น
            {
                newScale = 1.0f + (bulletCount - 25) * 0.02f;
                if (newScale > 2f) newScale = 2f;
                Scale = new Vector2(newScale, newScale);
            }
            else
            {
                Scale = Vector2.One;
            }

            //---ควบคุมการเคลื่อนที่รอบจุดศูนย์กลาง---
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

            //คำนวนณความเร็วการหมุน
            float minSpeed = 0.03f;
            float maxSpeed = 0.15f;
            //คำนวณมุมระหว่างเมาส์กับจุดศูนย์กลางคุกกี้
            float playerRotation = (float)Math.Atan2(mousePos.Y - _center.Y, mousePos.X - _center.X);
            float rotationSpeed = maxSpeed / (float)Math.Pow(Scale.X, 3);

            if (rotationSpeed < minSpeed) rotationSpeed = minSpeed;
            Rotation = CustomLerpAngle(Rotation, playerRotation, rotationSpeed);
            //ตำแหน่งของมดจะอยู่บนเส้นรอบวงของวงกลมรอบคุกกี้
            Position.X = _center.X + (float)Math.Cos(Rotation) * _radius;
            Position.Y = _center.Y + (float)Math.Sin(Rotation) * _radius;
        }

        public Bullet Fire(ref AntType currentType)
        {
            //ตรวจสอบการคลิกเมาส์ซ้าย
            if (Singleton.Instance.currentMouse.LeftButton == ButtonState.Pressed &&
                Singleton.Instance.previousMouse.LeftButton == ButtonState.Released)
            {
                //ตรวจสอบเงื่อนไขกระสุนปกติ (ถ้าเป็นมดปกติและกระสุนหมด จะยิงไม่ได้)
                if (currentType == AntType.Normal && Singleton.Instance.BulletCount <= 0)
                {
                    return null;
                }
                Bullet newBullet = new Bullet(Singleton.Instance.Bullet, this.Position, this.Rotation);

                // คุณสมบัติกระสุนตามชนิดมดที่ใช้งานอยู่
                switch (currentType)
                {
                    case AntType.Poison:
                        newBullet.IsPoisonBullet = true;
                        newBullet.PierceCount = 10;
                        break;

                    case AntType.Freeze:
                        newBullet.IsFreezeBullet = true;
                        break;

                    case AntType.Bomb:
                        newBullet.IsBombBullet = true;
                        break;
                    case AntType.Normal:
                        Singleton.Instance.BulletCount--;
                        break;
                }

                //จัดการจำนวนการใช้มดพิเศษ
                if (currentType != AntType.Normal)
                {
                    _antUseCount--;

                    // หากจำนวนการใช้งานมดพิเศษหมด ให้กลับเป็นมดปกติ
                    if (_antUseCount <= 0)
                    {
                        currentType = AntType.Normal;
                        ChangeTexture(Singleton.Instance.PlayerAnt);
                    }
                }
                return newBullet;
            }
            return null;
        }

        public void SetSpecialAnt(AntType type, int count, Texture2D texture)
        {
            this._antUseCount = count; //จำนวนครั้งที่สามารถใช้งานได้ ต่อการซื้อ 1 ครั้ง
            this.ChangeTexture(texture); //เปลี่ยนตัวละครตามตัวที่ซื้อ
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

        public void ChangeTexture(Texture2D newTexture)
        {
            _texture = newTexture;
            // อัปเดต Origin ใหม่
            Origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
        }

    }
}
