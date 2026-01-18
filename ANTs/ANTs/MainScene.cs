using ANTs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using static ANTs.Singleton;

namespace ANTs
{
    public class MainScene : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public SpriteFont _font;

        private float _spawnTimer = 0f;
        private float _spawnInterval = 3f;

        private Player _player;
        private List<Enemy> _enemies = new List<Enemy>();
        private List<Bullet> _bullets = new List<Bullet>();
        
        private int _rowCounter = 0;

        public MainScene()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = Singleton.SCREENWIDTH;
            _graphics.PreferredBackBufferHeight = Singleton.SCREENHEIGHT;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Singleton.Instance.LoadContent(this.Content, GraphicsDevice);

            _player = new Player(Singleton.Instance.PlayerAnt);

            _font = Content.Load<SpriteFont>("GameFont");

        }

        protected override void Update(GameTime gameTime)
        {
            // 1. ตรวจสอบปุ่ม Exit และสถานะ Game Over
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Singleton.Instance.currentMouse = Mouse.GetState();

            switch (Singleton.Instance.CurrentGameState)
            {
                case Singleton.GameState.Menu:
                    // ถ้ากด Space Bar ให้เริ่มเกม
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        Singleton.Instance.CurrentGameState = GameState.Playing;
                    break;

                case GameState.Playing:
                    UpdatePlaying(gameTime); // แยก Logic การเล่นไปไว้อีก Method เพื่อไม่ให้ Update ยาวเกินไป
                    if (Singleton.Instance.Life <= 0 || Singleton.Instance.BulletCount == 0)
                        Singleton.Instance.CurrentGameState = GameState.GameOver;

                    break;

                case GameState.GameOver:
                    if (Keyboard.GetState().IsKeyDown(Keys.R))
                    {
                        Singleton.Instance.Reset();
                        _enemies.Clear();
                        _bullets.Clear();
                        _spawnTimer = 0;
                    }
                    break;
            }

  
            Singleton.Instance.previousMouse = Singleton.Instance.currentMouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(33,33,33));

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            switch (Singleton.Instance.CurrentGameState)
            {
                case GameState.Menu:
                    _spriteBatch.Draw(Singleton.Instance.RectTexture, new Rectangle(0, 0, Singleton.SCREENWIDTH, Singleton.SCREENHEIGHT), Color.Black);
                    Vector2 fontStart = _font.MeasureString("Press [SPACE] to Start");
                    _spriteBatch.DrawString(_font, "Press [SPACE] to Start" , new Vector2((Singleton.SCREENWIDTH - fontStart.X) / 2, (Singleton.SCREENHEIGHT - fontStart.Y) / 2), Color.White);
                    break;

                case GameState.Playing:
                    //Game screen drawing
                    _spriteBatch.Draw(Singleton.Instance.RectTexture, new Vector2(0, Singleton.UI_TOP_HEIGHT), null, new Color(6, 146, 62), 0f, Vector2.Zero, new Vector2(Singleton.GAMEWIDTH, Singleton.GAMEHEIGHT), SpriteEffects.None, 0f);

                    //Cookie drawing
                    _spriteBatch.Draw(Singleton.Instance.Cookie, new Vector2(Singleton.GAMEWIDTH / 2f, Singleton.UI_TOP_HEIGHT + (Singleton.GAMEHEIGHT / 2f)), null, Color.White, 0f, new Vector2(Singleton.Instance.Cookie.Width / 2f, Singleton.Instance.Cookie.Height / 2f), 1.0f, SpriteEffects.None, 0f);

                    // วาด Objects
                    _player.Draw(_spriteBatch);
                    foreach (var enemy in _enemies) enemy.Draw(_spriteBatch);
                    foreach (var bullet in _bullets) bullet.Draw(_spriteBatch);

                    // --- UI ด้านบน (Score/Wave) ---
                    _spriteBatch.DrawString(_font, $"SCORE : {Singleton.Instance.Score}", new Vector2(10, 10), Color.White);

                    // --- 3. [ส่วนที่เพิ่ม] วาด UI มดพลังวิเศษที่มุมขวาบน ---
                    float startX = Singleton.SCREENWIDTH - 5; // เริ่มจากขอบขวาถอยเข้ามา
                    float uiY = 5; // ตำแหน่งแนวตั้งในแถบ UI บน
                    float itemSpacing = 100; // ระยะห่างระหว่างเซตไอเทมแต่ละอัน

                    // รายการมดพลังวิเศษ (วาดจากขวาไปซ้าย: Bomb -> Freeze -> Poison)
                    DrawStoreAntUI(startX - (itemSpacing * 0), uiY, "15", Singleton.Instance.BombAnt);
                    DrawStoreAntUI(startX - (itemSpacing * 1), uiY, "10", Singleton.Instance.FreezeAnt);
                    DrawStoreAntUI(startX - (itemSpacing * 2), uiY, "5", Singleton.Instance.PoisonAnt);

                    // --- UI ด้านล่าง (Life/Ammo) ---
                    // วาดเลือด 5 ดวง (LifeCookie)
                    for (int i = 0; i < Singleton.Instance.Life; i++)
                    {
                        _spriteBatch.Draw(Singleton.Instance.LifeCookie, new Vector2(15 + (i * 35), 660), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                    }

                    // วาดจำนวนกระสุน (BulletAnt)
                    _spriteBatch.Draw(Singleton.Instance.BulletAnt, new Vector2(500, 665), Color.White);
                    _spriteBatch.DrawString(_font, $"X {Singleton.Instance.BulletCount}", new Vector2(540, 665), Color.Yellow);

                    break;

                case GameState.GameOver:
                    _spriteBatch.Draw(Singleton.Instance.RectTexture, new Rectangle(0, 0, Singleton.SCREENWIDTH, Singleton.SCREENHEIGHT), Color.Black);
                    Vector2 fontSize1 = _font.MeasureString("GAME OVER! Press R to Restart"); 
                    _spriteBatch.DrawString(_font, "GAME OVER! Press R to Restart", new Vector2((Singleton.SCREENWIDTH - fontSize1.X) / 2, (Singleton.SCREENHEIGHT - fontSize1.Y) / 2), Color.Red);
                    break;

            }

            _spriteBatch.End();
            _graphics.BeginDraw();

            base.Draw(gameTime);
        }

        
        private void DrawStoreAntUI(float x, float y, string cost, Texture2D antTexture)
        {
            // จุดอ้างอิงหลัก
            Vector2 pos = new Vector2(x, y);

            // ระยะเยื้องแนวตั้ง (Vertical Alignment) 
            // ปรับค่าเหล่านี้เพื่อให้ตัวอักษรและรูปภาพดู "กลาง" เท่ากัน
            float textOffsetY = 8;  // ขยับตัวอักษรลงมาหน่อย
            float iconOffsetY = 20; // ขยับรูปภาพ (อิงจากกึ่งกลางรูป)

            // 1. วาดราคา (Cost)
            _spriteBatch.DrawString(_font, cost, new Vector2(pos.X - 95, pos.Y + textOffsetY), Color.Yellow);

            // 2. วาดรูป BulletAnt (มดกระสุน)
            _spriteBatch.Draw(Singleton.Instance.BulletAnt,
                new Vector2(pos.X - 62, pos.Y + iconOffsetY),
                null, Color.White, 0f,
                new Vector2(Singleton.Instance.BulletAnt.Width / 2f, Singleton.Instance.BulletAnt.Height / 2f),
                0.7f, SpriteEffects.None, 0f);

            // 3. วาดเครื่องหมาย :
            _spriteBatch.DrawString(_font, ":", new Vector2(pos.X - 50, pos.Y + textOffsetY), Color.White);

            // 4. วาดรูปมดพลังวิเศษ (Special Ant)
            _spriteBatch.Draw(antTexture,
                new Vector2(pos.X - 25, pos.Y + iconOffsetY),
                null, Color.White, 0f,
                new Vector2(antTexture.Width / 2f, antTexture.Height / 2f),
                0.7f, SpriteEffects.None, 0f);
        }

        private void UpdatePlaying(GameTime gameTime)
        {
            //อัปเดต Player (มดหมุนรอบคุกกี้ตามเมาส์)
            _player.Update(gameTime);

            //ระบบยิงกระสุน (คลิกเมาส์ซ้าย)
            Bullet newBullet = _player.Fire();
            if (newBullet != null) _bullets.Add(newBullet);

            //ระบบเกิดของศัตรู (Spawn Timer)
            _spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_spawnTimer >= _spawnInterval)
            {

                _rowCounter++;
                List<Enemy> _spawnAntEnemy = Enemy.SpawnEnemy(_rowCounter, Singleton.Instance.EnemyAnt);
                _enemies.AddRange(_spawnAntEnemy);
                _spawnTimer = 0f;
                if (_spawnInterval > 0.8f) _spawnInterval -= 0.05f;
            }

            // [ส่วนที่แก้ไข] ตรวจสอบศัตรูและการชนคุกกี้
            // ใช้ for loop ถอยหลังเพื่อป้องกัน Error เวลาลบข้อมูลใน List
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                _enemies[i].Update(gameTime);

                // ตรวจสอบว่ามดตัวนี้ "ชนคุกกี้" หรือไม่ (IsActive จะเป็น false จากใน Enemy.cs)
                if (!_enemies[i].IsActive)
                {
                    int currentRowId = _enemies[i].RowId;

                    // ลบมดทุกตัวที่มี RowId เดียวกัน (หายไปทั้งแถว)
                    _enemies.RemoveAll(e => e.RowId == currentRowId);

                    // ลดชีวิต 1 แต้มต่อ 1 แถว
                    Singleton.Instance.Life--;
                    Singleton.Instance.BulletCount += 3;

                    break;
                }
            }

            //[ส่วนที่แก้ไข] ตรวจสอบกระสุนชนศัตรู
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime);

                // เช็คการชนกับศัตรูทีละตัว
                bool bulletHit = false;
                for (int j = _enemies.Count - 1; j >= 0; j--)
                {
                    if (_bullets[i].Rectangle.Intersects(_enemies[j].Rectangle))
                    {
                        // ถ้าโดนยิง ตายแค่ตัวเดียว (ไม่ต้องลบทั้งแถว)
                        _enemies.RemoveAt(j);
                        bulletHit = true;

                        // เพิ่มคะแนนและคืนกระสุน
                        Singleton.Instance.Score += 10;
                        Singleton.Instance.BulletCount += 3;
                        break;
                    }
                }

                // ลบกระสุนออกถ้าชนศัตรู หรือ IsActive เป็น false (ออกนอกจอ)
                if (bulletHit || !_bullets[i].IsActive)
                {
                    _bullets.RemoveAt(i);
                }
            }
        }
    }
}
