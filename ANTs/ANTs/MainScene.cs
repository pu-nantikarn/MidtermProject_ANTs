using ANTs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using static ANTs.Singleton;

public enum AntType
{
    Normal,
    Poison,
    Freeze,
    Bomb
}

namespace ANTs
{
    public class MainScene : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public SpriteFont _font;

        private Player _player;
        private List<Enemy> _enemies = new List<Enemy>();
        private List<Bullet> _bullets = new List<Bullet>();

        private KeyboardState _prevKeyboard;
        public float _keyCooldown = 0f; //ระยะห่างระหว่างการกด ป้องกันการกดค้าง
        public AntType _currentAnt = AntType.Normal;
        public int _antUseCount = 0;

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
            //  อ่าน Keyboard แค่ครั้งเดียวต่อเฟรม 
            var kb = Keyboard.GetState();
            _keyCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Check กดปุ่ม Esc เพื่อออกจากเกม 
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape))
                Exit();

            Singleton.Instance.currentMouse = Mouse.GetState(); 

            switch (Singleton.Instance.CurrentGameState)
            {
                case Singleton.GameState.Menu:
                    {
                        // กด SPACE ครั้งเดียวเพื่อเริ่มเกม
                        if (kb.IsKeyDown(Keys.Space) && _prevKeyboard.IsKeyUp(Keys.Space))
                        {
                            StartNewGame();
                            Singleton.Instance.CurrentGameState = GameState.Playing;
                        }
                        break;
                    }

                case GameState.Playing:
                    {
                        // A = buy Poison Ant (cost 5) ใช้ได้ 3 ครั้ง
                        if (_keyCooldown <= 0f && kb.IsKeyDown(Keys.A))
                        {
                            if (_currentAnt == AntType.Normal && Singleton.Instance.BulletCount >= 5)
                            {
                                Singleton.Instance.BulletCount -= 5;
                                _currentAnt = AntType.Poison;
                                _player.SetSpecialAnt(AntType.Poison, 3 ,Singleton.Instance.PoisonAnt);
                                
                            }
                            _keyCooldown = 0.2f;
                        }

                        // S = buy Freeze Ant (cost 10) ใช้ได้ 2 ครั้ง
                        if (_keyCooldown <= 0f && kb.IsKeyDown(Keys.S))
                        {
                            if (_currentAnt == AntType.Normal && Singleton.Instance.BulletCount >= 10)
                            {
                                Singleton.Instance.BulletCount -= 10;
                                _currentAnt = AntType.Freeze;
                                _player.SetSpecialAnt(AntType.Freeze, 2, Singleton.Instance.FreezeAnt);
                            }
                            _keyCooldown = 0.2f;
                        }

                        // D = buy Bomb Ant (cost 15) ใช้ได้ 1 ครั้ง
                        if (_keyCooldown <= 0f && kb.IsKeyDown(Keys.D))
                        {
                            if (_currentAnt == AntType.Normal && Singleton.Instance.BulletCount >= 15)
                            {
                                Singleton.Instance.BulletCount -= 15;
                                _currentAnt = AntType.Bomb;
                                _player.SetSpecialAnt(AntType.Bomb, 1, Singleton.Instance.BombAnt);
                            }
                            _keyCooldown = 0.2f;
                        }

                        //เรียก method ที่รวม update Playing อื่นๆ  
                        UpdatePlaying(gameTime); 

                        //พลังชีวืตหมด => Game Over
                        if (Singleton.Instance.Life <= 0 )
                            Singleton.Instance.CurrentGameState = GameState.GameOver;


                        break;
                    }

                case GameState.GameOver:
                    {
                        // R = Restart (เริ่มเล่นใหม่)
                        if (_keyCooldown <= 0f && kb.IsKeyDown(Keys.R))
                        {
                            StartNewGame();
                            Singleton.Instance.CurrentGameState = GameState.Playing;
                            _keyCooldown = 0.2f;
                        }


                        // B = Back to Menu (กลับเมนู)
                        if (_keyCooldown <= 0f && kb.IsKeyDown(Keys.B))
                        {
                            StartNewGame();

                            Singleton.Instance.CurrentGameState = GameState.Menu;
                            _keyCooldown = 0.2f; // กันกดค้างรัว
                        }

                        break;
                    }
            }

            Singleton.Instance.previousMouse = Singleton.Instance.currentMouse;

            //  อัปเดต prev keyboard 
            _prevKeyboard = kb;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(33, 33, 33));

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            switch (Singleton.Instance.CurrentGameState)
            {
                case GameState.Menu:
                    {
                        //Draw Screen Menu
                        _spriteBatch.Draw(
                            Singleton.Instance.RectTexture,
                            new Rectangle(0, 0, Singleton.SCREENWIDTH, Singleton.SCREENHEIGHT),
                            Color.Black
                        );

                        //Draw Title Logo
                        Texture2D title = Content.Load<Texture2D>("Ants_logo");
                        _spriteBatch.Draw(title, new Vector2(10,30), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                        Vector2 start = _font.MeasureString("Press [SPACE] to Start");
                        _spriteBatch.DrawString(_font, "Press [SPACE] to Start", new Vector2((Singleton.SCREENWIDTH - start.X ) / 2  , (Singleton.SCREENHEIGHT - start.Y) / 2 ) , Color.Yellow);

                        string[] lines =
                        {
                            "How to play",
                            "Use mouse to move, Click to shoot",
                            "Press [A] to buy Poison Ant",
                            "Press [S] to buy Freeze Ant",
                             "Press [D] to buy Bomb Ant"
                        };

                        float startY = 500f;
                        float gap = 28f;

                        //Loop Draw text 
                        for (int i = 0; i < lines.Length; i++)
                        {
                            Vector2 size = _font.MeasureString(lines[i]);

                            _spriteBatch.DrawString(
                                _font,
                                lines[i],
                                new Vector2((Singleton.SCREENWIDTH - size.X) / 2f, startY + i * gap),
                                Color.White
                            );
                        }
                        //Draw ant icon behind text
                        _spriteBatch.Draw(Singleton.Instance.PoisonAnt, new Vector2(430, 558), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                        _spriteBatch.Draw(Singleton.Instance.FreezeAnt, new Vector2(430, 586), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                        _spriteBatch.Draw(Singleton.Instance.BombAnt, new Vector2(430, 614), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                        break;
                    }

                case GameState.Playing:
                    //Game screen drawing
                    _spriteBatch.Draw(Singleton.Instance.RectTexture, new Vector2(0, Singleton.UI_TOP_HEIGHT), null, new Color(6, 146, 62), 0f, Vector2.Zero, new Vector2(Singleton.GAMEWIDTH, Singleton.GAMEHEIGHT), SpriteEffects.None, 0f);

                    // Cookie drawing
                    _spriteBatch.Draw(Singleton.Instance.Cookie, new Vector2(Singleton.GAMEWIDTH / 2f, Singleton.UI_TOP_HEIGHT + (Singleton.GAMEHEIGHT / 2f)), null, Color.White, 0f, new Vector2(Singleton.Instance.Cookie.Width / 2f, Singleton.Instance.Cookie.Height / 2f), 1.0f, SpriteEffects.None, 0f);

                    //Draw Player, Enemy, Bullet
                    _player.Draw(_spriteBatch);
                    foreach (var enemy in _enemies) enemy.Draw(_spriteBatch);
                    foreach (var bullet in _bullets) bullet.Draw(_spriteBatch);

                    // --- UI Top ---
                    //Draw Score UI
                    _spriteBatch.DrawString(_font, $"SCORE : {Singleton.Instance.Score}", new Vector2(10, 10), Color.White);

                    //Draw Ant Store UI (ขวาบน)
                    float startX = Singleton.SCREENWIDTH - 5; // เริ่มจากขอบขวาถอยเข้ามา
                    float uiY = 5; // ตำแหน่งแนวตั้งในแถบ UI บน
                    float itemSpacing = 130; // ระยะห่างระหว่างเซตไอเทมแต่ละอัน
                    DrawStoreAntUI(startX - (itemSpacing * 0), uiY, "15", "1",Singleton.Instance.BombAnt);
                    DrawStoreAntUI(startX - (itemSpacing * 1), uiY, "10", "2",Singleton.Instance.FreezeAnt);
                    DrawStoreAntUI(startX - (itemSpacing * 2), uiY, "5", "3",Singleton.Instance.PoisonAnt);

                    // --- UI Bottom ---
                    //Loop Draw LifeCookie
                    for (int i = 0; i < Singleton.Instance.Life; i++)
                    {
                        _spriteBatch.Draw(Singleton.Instance.LifeCookie, new Vector2(15 + (i * 35), 660), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                    }

                    //Draw จำนวน Bullet
                    _spriteBatch.Draw(Singleton.Instance.BulletAnt, new Vector2(500, 665), Color.White);
                    _spriteBatch.DrawString(_font, $"X {Singleton.Instance.BulletCount}", new Vector2(540, 665), Color.Yellow);

                    break;

                case GameState.GameOver:
                    {
                        //Draw Screen
                        _spriteBatch.Draw(
                            Singleton.Instance.RectTexture,
                            new Rectangle(0, 0, Singleton.SCREENWIDTH, Singleton.SCREENHEIGHT),
                            Color.Black
                        );

                        //Draw Text Game Over
                        string over = "GAME OVER";
                        Vector2 overSize = _font.MeasureString(over);
                        _spriteBatch.DrawString(
                            _font,
                            over,
                            new Vector2((Singleton.SCREENWIDTH - overSize.X) / 2f, 220f),
                            Color.Red
                        );

                        //Draw Total Score
                        string scoreText = $"Your Score: {Singleton.Instance.Score}";
                        Vector2 scoreSize = _font.MeasureString(scoreText);
                        _spriteBatch.DrawString(
                            _font,
                            scoreText,
                            new Vector2((Singleton.SCREENWIDTH - scoreSize.X) / 2f, 280f),
                            Color.White
                        );

                        //---- Draw Button Restart & Back to menu ----
                        string hint1 = "Press [R] to Restart";
                        string hint2 = "Press [B] to Back to Menu";

                        Vector2 h1 = _font.MeasureString(hint1);
                        Vector2 h2 = _font.MeasureString(hint2);

                        _spriteBatch.DrawString(
                            _font,
                            hint1,
                            new Vector2((Singleton.SCREENWIDTH - h1.X) / 2f, 350f),
                            Color.Yellow
                        );

                        _spriteBatch.DrawString(
                            _font,
                            hint2,
                            new Vector2((Singleton.SCREENWIDTH - h2.X) / 2f, 390f),
                            Color.White
                        );

                        break;
                    }
            }

            _spriteBatch.End();
            _graphics.BeginDraw();

            base.Draw(gameTime);
        }

        private void DrawStoreAntUI(float x, float y, string cost, string specialAntCount, Texture2D antTexture)
        {
            // จุดอ้างอิงหลัก
            Vector2 pos = new Vector2(x, y);

            // ระยะเยื้องแนวตั้ง (Vertical Alignment)
            float textOffsetY = 8;  // ขยับตัวอักษรลงมา
            float iconOffsetY = 20; // ขยับรูปภาพลงมา

            //Draw Cost
            _spriteBatch.DrawString(_font, cost, new Vector2(pos.X - 95, pos.Y + textOffsetY), Color.White);

            //Draw icon BulletAnt
            _spriteBatch.Draw(Singleton.Instance.BulletAnt,
                new Vector2(pos.X - 62, pos.Y + iconOffsetY),
                null, Color.White, 0f,
                new Vector2(Singleton.Instance.BulletAnt.Width / 2f, Singleton.Instance.BulletAnt.Height / 2f),
                0.7f, SpriteEffects.None, 0f);

            //Draw ":"
            _spriteBatch.DrawString(_font, ":", new Vector2(pos.X - 50, pos.Y + textOffsetY), Color.White);

            //Draw Ants per purchase
            _spriteBatch.DrawString(_font, specialAntCount, new Vector2(pos.X - 38, pos.Y + textOffsetY), Color.Yellow);

            //Draw Special Ant
            _spriteBatch.Draw(antTexture,
                new Vector2(pos.X - 10, pos.Y + iconOffsetY),
                null, Color.White, 0f,
                new Vector2(antTexture.Width / 2f, antTexture.Height / 2f),
                0.8f, SpriteEffects.None, 0f);
        }

        private void UpdatePlaying(GameTime gameTime)
        {
            // ปรับความยากตามคะแนน 
            Singleton.Instance.UpdateDifficulty();

            //อัปเดต Player 
            _player.Update(gameTime);

            //ระบบยิงกระสุน 
            Bullet newBullet = _player.Fire(ref _currentAnt);
            if (newBullet != null)
            {
                _bullets.Add(newBullet);
            }

            //ระบบเกิดของศัตรู (Spawn Timer)
            Enemy.SpawnTimer(gameTime, _enemies);  

            //ตรวจสอบศัตรูชนคุกกี้
            Enemy.CheckEnemyCollision(_enemies, gameTime);

            //ตรวจสอบกระสุนชนศัตรู
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime);

                bool hasHit = _bullets[i].CheckBulletCollision(_enemies);

                // ลบกระสุนออกถ้าชนศัตรู หรือออกนอกจอ
                if (hasHit || !_bullets[i].IsActive)
                {
                    _bullets.RemoveAt(i);
                }
            }
        }

        //reset สถานะเกมให้เริ่มใหม่
        private void StartNewGame()
        {
            Singleton.Instance.Reset();
            Enemy.ResetSpawner();
            _enemies.Clear();
            _bullets.Clear();

            _currentAnt = AntType.Normal;   
            if (_player != null)
            {
                _player.ChangeTexture(Singleton.Instance.PlayerAnt);

            }
        }

        
    }
}
