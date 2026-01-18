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
        public float ExplosionRadius = 40f;

        public BombAnt(Texture2D texture, Vector2 position) : base(texture)
        {
            Position = position;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Explode(List<Enemy> enemies, Vector2 center)
        {
            foreach (Enemy e in enemies)
            {
                if (!e.IsActive) continue;

                if (Vector2.Distance(e.Position, center) <= ExplosionRadius)
                {
                    e.IsActive = false;
                }
            }
        }
    }
}
