using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ANTs
{
    public class GameObject
    {
        protected Texture2D _texture;
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale = Vector2.One;
        public Vector2 Origin;
        public bool IsActive = true;

        public GameObject(Texture2D texture)
        {
            _texture = texture;
            Origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            spriteBatch.Draw(_texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }

        public Rectangle Rectangle => new Rectangle((int)(Position.X - Origin.X * Scale.X), (int)(Position.Y - Origin.Y * Scale.Y), (int)(_texture.Width * Scale.X),(int)( _texture.Height * Scale.Y));
    }

}
