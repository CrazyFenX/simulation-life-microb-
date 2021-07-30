using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simulation__life_
{
    public partial class Form1 : Form
    {
        private ushort[,] cells;
        private ushort[,] templateCells;
        Graphics graph;

        bool isitdown = false;
        bool isItEraser = false;
        Size cellSize = new Size(5, 5);
        Point location = new Point(0, 0);
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            cells = new ushort[100, 100];
            templateCells = new ushort[100, 100];
            cellSize = new Size(7, 7);
            location = new Point(0, 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            graph = e.Graphics;
            DrawIt();
        }

        protected void DrawIt()
        {
            for (int i = 0; i < cells.GetLength(0); i++)
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    var rect = new Rectangle(location.X + i * cellSize.Width, location.Y + j * cellSize.Height, cellSize.Width, cellSize.Height);
                    var brush = cells[i, j] == (byte)1 ? Brushes.Black : Brushes.White;
                    graph.FillRectangle(brush, rect);
                    //graph.DrawRectangle(Pens.Black, rect);
                }
        }

        protected void drawTheNet()
        {
            for (int i = 0; i < cells.GetLength(0); i++)
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    var rect = new Rectangle(location.X + i * cellSize.Width, location.Y + j * cellSize.Height, cellSize.Width, cellSize.Height);
                    //graph.FillRectangle(Brushes.White, rect);
                    graph.DrawRectangle(Pens.Black, rect);
                }
        }

        private void drawLocalChange(int i, int j)
        {
            Rectangle rect = new Rectangle(location.X + i * cellSize.Width, location.Y + j * cellSize.Height, cellSize.Width, cellSize.Height);
            //Brush brush = cells[i, j] == (byte)1 ? Brushes.Black : Brushes.White;
            Brush brush;
            switch (cells[i, j])
            {
                case 0:
                    brush = Brushes.White;
                    break;
                case 1:
                    brush = Brushes.Black;
                    break;
                case 2:
                    brush = Brushes.Red;
                    break;
                default:
                    brush = Brushes.White;
                    break;
            }
            graph.FillRectangle(brush, rect);
            //graph.DrawRectangle(Pens.Black, rect);
        }

        private void syncronizeMaps()
        {
            for (int i = 1; i < cells.GetLength(0) - 1; i++)
                for (int j = 1; j < cells.GetLength(1) - 1; j++)
                {
                    templateCells[i, j] = cells[i, j];
                }
        }

        private void bornPoints ()
        {
            ushort sum = 0;
            ushort[,] tmpCells = new ushort[cells.GetLength(0), cells.GetLength(1)];
            syncronizeMaps();

            for (int i = 1; i < cells.GetLength(0) - 1; i++)
                for (int j = 1; j < cells.GetLength(1) - 1; j++)
                {
                    sum = getSum(i, j);
                    tmpCells[i, j] = sum;
                }

            for (int i = 1; i < cells.GetLength(0) - 1; i++)
                for (int j = 1; j < cells.GetLength(1) - 1; j++)
                {
                    if (tmpCells[i, j] == 3)
                    {
                        cells[i, j] = 1;
                        if (rnd.Next() % 100 == 1 || rnd.Next() % 100 == 2)
                            cells[i, j] = 2;
                    }
                    //else if (tmpCells[i, j] != 2 && tmpCells[i, j] != 3 && tmpCells[i, j] != 4 && tmpCells[i, j] != 5 && tmpCells[i, j] != 6 && tmpCells[i, j] != 7 && tmpCells[i, j] != 8)
                    else if (tmpCells[i, j] != 2 && tmpCells[i, j] != 3)
                    {
                        cells[i, j] = 0;
                    }
                    //if (cells[i, j] == 2 && rnd.Next() % 10 >= 9)
                    if (cells[i, j] == 2 && tmpCells[i, j] > 1)
                    {
                        cells[i + 1, j] = 2;
                        cells[i, j + 1] = 2;
                        cells[i, j - 1] = 2;
                        cells[i - 1, j] = 2;
                        cells[i, j] = 0;
                    }
                }
            drawFromChecking();
        }

        private void drawFromChecking()
        {
            for (int i = 1; i < cells.GetLength(0) - 1; i++)
                for (int j = 1; j < cells.GetLength(1) - 1; j++)
                {
                    if (templateCells[i, j] != cells[i, j])
                    {
                        graph = this.CreateGraphics();
                        drawLocalChange(i, j);
                    }
                }
        }

        private ushort getSum(int i, int j)
        {
            ushort sum = 0;
            if (cells[i - 1, j - 1] == 1)
                sum++;
            if (cells[i - 1, j] == 1)
                sum++;
            if (cells[i - 1, j + 1] == 1)
                sum++;
            if (cells[i, j - 1] == 1)
                sum++;
            if (cells[i, j + 1] == 1)
                sum++;
            if (cells[i + 1, j - 1] == 1)
                sum++;
            if (cells[i + 1, j] == 1)
                sum++;
            if (cells[i + 1, j + 1] == 1)
                sum++;
            return sum;
        }

        private void randomPointGeneration()
        {
            Random rnd = new Random();
            for (int i = 1; i < cells.GetLength(0) - 1; i++)
                for (int j = 1; j < cells.GetLength(1) - 1; j++)
                {
                    if (rnd.Next() % 5 == 1)
                    {
                        cells[i, j] = 1;
                        //if (rnd.Next() % 100 == 1)
                        //    cells[i, j] = 2;
                    }
                }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int width = e.X / cellSize.Width;
            int height = e.Y / cellSize.Height;
            if (cells.GetLength(0) > width && cells.GetLength(1) > height)
                if ( e.Button == MouseButtons.Left)
                {
                    cells[width, height] = (byte)1;
                    graph = this.CreateGraphics();
                    drawLocalChange(width, height);
                    isitdown = true;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    cells[width, height] = (byte)0;
                    graph = this.CreateGraphics();
                    drawLocalChange(width, height);
                    isItEraser = true;
                }
        }

        private void timerOfMove_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToLongTimeString();
            bornPoints();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            timerOfMove.Start();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timerOfMove.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            randomPointGeneration();
            drawFromChecking();
        }

        private void timerDrawing_Tick(object sender, EventArgs e)
        {
            //graph = this.CreateGraphics();
            //drawTheNet();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            isitdown = false;
            isItEraser = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isitdown)
            {
                int width = e.X / cellSize.Width;
                int height = e.Y / cellSize.Height;
                if (cells.GetLength(0) > width && cells.GetLength(1) > height)
                {
                    cells[width, height] = (byte)1;
                    if (rnd.Next() % 100 == 1 || rnd.Next() % 100 == 2)
                        cells[width, height] = 2;
                    graph = this.CreateGraphics();
                    drawLocalChange(width, height);
                }
            }
            if (isItEraser)
            {
                int width = e.X / cellSize.Width;
                int height = e.Y / cellSize.Height;
                if (cells.GetLength(0) > width && cells.GetLength(1) > height)
                {
                    cells[width, height] = (byte)0;
                    graph = this.CreateGraphics();
                    drawLocalChange(width, height);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < cells.GetLength(0) - 1; i++)
                for (int j = 1; j < cells.GetLength(1) - 1; j++)
                {
                    cells[i, j] = 0;
                    graph = this.CreateGraphics();
                    drawLocalChange(i, j);
                }
        }
    }
}
