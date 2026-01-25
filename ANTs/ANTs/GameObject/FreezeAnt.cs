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
        public static float FreezeDuration = 5f; // เวลาแช่แข็ง (วินาที)

        public FreezeAnt(Texture2D texture, Vector2 position) : base(texture)
        {
            Position = position;
    
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        //ความสามารถของ FreezeAnt ใช้กระสุนแช่แข็ง ศัตรูจะถูกหยุดชั่วคราวทั้งแถว
        public static void Freeze(List<Enemy> Rowenemies, int targetRowId) {
            foreach (var enemy in Rowenemies)
            {
                if (enemy.RowId == targetRowId)
                {
                    enemy.EnemyFreeze(FreezeDuration);
                }
            }
        }
    }
}
