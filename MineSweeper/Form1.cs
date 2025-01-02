using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            InitializeGame(30, 30, 100); // Oyun boyutu ve bomba sayısı
        }

        void InitializeGame(int width, int height, int bombCount)
        {
            Random random = new Random();
            List<Bomb> bombList = new List<Bomb>();

            pGame.Width = width * 20; // Panel genişliği
            pGame.Height = height * 20; // Panel yüksekliği

            // Bombaların rastgele yerleştirilmesi
            for (int i = 0; i < bombCount; i++)
            {
                Bomb bomb;
                do
                {
                    bomb = new Bomb(random.Next(0, width), random.Next(0, height));
                }
                while (bombList.Any(b => b.X == bomb.X && b.Y == bomb.Y));

                bombList.Add(bomb);
            }

            // Butonların oluşturulması
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MButton mb = new MButton
                    {
                        X = x,
                        Y = y,
                        Width = 20, // Buton genişliği
                        Height = 20, // Buton yüksekliği
                        Margin = new Padding(0),
                        Font = new Font("Arial", 8, FontStyle.Bold), // Daha küçük font
                        BackColor = Color.Gray,
                        Location = new Point(x * 20, y * 20),
                        FlatStyle = FlatStyle.Flat
                    };

                    if (bombList.Any(b => b.X == x && b.Y == y))
                    {
                        mb.IsBomb = true;
                    }

                    pGame.Controls.Add(mb);
                    mb.MouseDown += Mb_MouseDown;
                }
            }
        }

        private void Mb_MouseDown(object sender, MouseEventArgs e)
        {
            MButton mb = sender as MButton;

            if (mb != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (mb.IsClicked || mb.IsFlagged) return;

                    if (mb.IsBomb)
                    {
                        mb.BackColor = Color.Red;
                        MessageBox.Show("KAYBETTİNİZ!");
                        EndGame(false);
                    }
                    else
                    {
                        RevealButton(mb);
                        CheckWinCondition();
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (!mb.IsClicked)
                    {
                        mb.IsFlagged = !mb.IsFlagged;
                        mb.Text = mb.IsFlagged ? "🚩" : "";
                    }
                }
            }
        }

        private void RevealButton(MButton mb)
        {
            if (mb.IsClicked || mb.IsBomb) return;

            mb.IsClicked = true;
            mb.BackColor = Color.White;

            if (mb.NearbyBombCount == 0)
            {
                foreach (var neighbor in mb.GetNeighbors())
                {
                    if (!neighbor.IsClicked && !neighbor.IsFlagged)
                    {
                        RevealButton(neighbor);
                    }
                }
            }
            else
            {
                mb.ForeColor = mb.NearbyBombCount == 1 ? Color.Blue :
                               mb.NearbyBombCount == 2 ? Color.Green :
                               mb.NearbyBombCount == 3 ? Color.Red : Color.Purple;
                mb.Text = mb.NearbyBombCount.ToString();
            }
        }

        private void CheckWinCondition()
        {
            foreach (Control ctrl in pGame.Controls)
            {
                MButton mb = ctrl as MButton;
                if (mb != null && !mb.IsBomb && !mb.IsClicked)
                {
                    return;
                }
            }

            MessageBox.Show("TEBRİKLER! KAZANDINIZ!");
            EndGame(true);
        }

        private void EndGame(bool isWin)
        {
            foreach (Control ctrl in pGame.Controls)
            {
                MButton mb = ctrl as MButton;
                if (mb != null)
                {
                    mb.Enabled = false;

                    if (mb.IsBomb && !isWin)
                    {
                        mb.BackColor = Color.Red;
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnStart_Click_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public class MButton : Button
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsClicked { get; set; }
        public bool IsBomb { get; set; }
        public bool IsFlagged { get; set; }

        public int NearbyBombCount
        {
            get
            {
                return GetNeighbors().Count(n => n.IsBomb);
            }
        }

        public List<MButton> GetNeighbors()
        {
            List<MButton> neighbors = new List<MButton>();

            foreach (Control ctrl in Parent.Controls)
            {
                if (ctrl is MButton mb)
                {
                    if (Math.Abs(mb.X - X) <= 1 && Math.Abs(mb.Y - Y) <= 1 && mb != this)
                    {
                        neighbors.Add(mb);
                    }
                }
            }

            return neighbors;
        }
    }

    public class Bomb
    {
        public Bomb(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}