using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form1 : Form
    {
        int[,] map;
        int timer;
        int w, h, squareSize;
        Shape shape;
        
        public int score;
        public Form1()
        {
            InitializeComponent();
            Init();
        }
        //Инициализация размеров поля, новой фигуры, таймера, нажатия клавиши
        public void Init()
        {
            w = 16;
            h = 8;
            timer = 500;
            map = new int[w, h];
            squareSize = 25;
            score = 0;
            shape = new Shape(3, 0);
           
            // map = new int[w, h];

            this.KeyUp += new KeyEventHandler(KeyFunc);

            timer1.Interval = timer;
            timer1.Tick += new EventHandler(update);
            timer1.Start();
            //Делает недействительной конкретную область элемента управления 
            //и вызывает отправку сообщения рисования элементу управления.
            Invalidate();
        }

        //Обработка клавиш
        private void KeyFuncGameOver(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    //Презапуск игры
                    for (int i = 0; i < w; i++)
                    {
                        for (int j = 0; j < h; j++)
                        {
                            map[i, j] = 0;
                        }
                    }
                    timer1.Tick -= new EventHandler(update);
                    Init();
                    break;
                default: break;
            }
        }
        private void KeyFunc(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //Поворот
                case Keys.Q:
                    //Если вокруг нет края карты/другой фигуры
                    if (RotateAbility())
                    { 
                        ResetArea();  
                        shape.rotate();                 
                        Megre();
                        Invalidate();
                    }
                    break;
                    //Резко опустить фигуру вниз
                case Keys.Down:
                    timer1.Interval = 10;
                    break;
                case Keys.Right:
                    //Если справа нет других объектов
                    if (!CollideHor(1))
                    {
                        ResetArea();
                        shape.moveRight();
                        Megre();
                        Invalidate();
                    }                          
                break;
                case Keys.Left:
                    //Если слева нет других объектов
                    if (!CollideHor(-1))
                    {
                        ResetArea();
                        shape.moveLeft();
                        Megre();
                        Invalidate();     
                    }
                    break;  
            }
        }

        
        private void update(object sender, EventArgs e)
        {
            ResetArea();
            //При отсутствии столкновения двигаться вниз 
            if (!Collide())
            {
                shape.moveDown();
            }
            else 
            {
                //Синхронизировать карту
                Megre();
                sliceMap(); 
                //На случай, если была использована функция сбрасывания фигуры вниз
                timer1.Interval = timer;
                //Создать новую фигуру
                shape.genetateNextShape(3, 0);
                //Если сразу после появления фигуры она во что-то уперлась - игра окончена
                if (Collide())
                {
                    timer1.Stop();
                    label1.Text = "Game over! Your score is " + score + "Press space to start new game";
                    this.KeyUp -= KeyFunc;
                    this.KeyUp += new KeyEventHandler(KeyFuncGameOver);
                   


                }
            }
            Megre();
            Invalidate();
        }

        //Проверка, есть ли у фигуры место для поворота
        public bool RotateAbility()
        {
            for (int i = shape.y; i <shape.y +shape.mSize; i++)
            {
                for (int j = shape.x; j < shape.x +shape.mSize; j++)
                {
                   
                    if (j>0 && j < h)
                    {
                        if (map[i, j] != 0 && shape.matrix[i - shape.y, j - shape.x] == 0)
                        {
                            return false;
                        }
                    }
                    
                }
            }
            return true;
        }
       
        public void Megre()
        {
            for (int i = shape.y; i < shape.y + shape.mSize; i++)
            {
                for (int j = shape.x; j < shape.x + shape.mSize; j++)
                {
                    if (shape.matrix[i - shape.y, j - shape.x] != 0)
                    {
                        map[i, j] = shape.matrix[i - shape.y, j - shape.x];
                    }

                }
            }
        }

        //Столкновение фигуры по оси Y
        public bool Collide()
        {
            for (int i = shape.y + shape.mSize - 1; i >= shape.y; i--)
            {
                for (int j = shape.x; j < shape.x + shape.mSize; j++)
                {
                    if (shape.matrix[i - shape.y, j - shape.x] != 0)
                    {                      
                        if (i + 1 == w)
                        {
                            return true;
                        }

                        if (map[i+1, j] != 0)
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        //Столкновение фигуры по оси X
        public bool CollideHor(int dir)
        {
            for (int i = shape.y; i < shape.y + shape.mSize; i++)
            {
                for (int j = shape.x; j < shape.x + shape.mSize; j++)
                {
                    if (shape.matrix[i - shape.y, j - shape.x] != 0)
                    {
                        if (j + 1 * dir > 7 || j + 1 * dir <  0)
                        {
                            return true;
                        }

                        if (map[i, j + 1 * dir] != 0 )
                        {
                            if (j -shape.x + 1 * dir >= shape.mSize || j - shape.x + 1 * dir < 0)
                            {
                                return true;
                            }
                            if (shape.matrix[i - shape.y, j-shape.x + 1 * dir] == 0)
                            {
                                return true;
                            }
                        }                 
                    }

                }
            }
            return false;
        }

        //Перерисовка карты в случае стирания одной из полос
        public void sliceMap() 
        {
            int count = 0;
            int removedLindes = 0;
            for (int i = 0; i < w; i++)
            {
                count = 0;
                for (int j = 0; j < h; j++)
                {
                    if (map[i, j] != 0)
                    {
                        count++;
                    }
                }

                if (count == h)
                {
                    removedLindes++;    
                    for (int l = i; l >= 1; l--)
                    {
                        for (int m = 0; m < h; m++)
                        {
                            map[l, m] = map[l-1, m];
                        }
                    }
                    
                }
            }
            for (int i = 0; i< removedLindes; i++)
                score += (10 * removedLindes);
            label1.Text = "Your score: "  + score; 
        }

        //Метод, очищающий карту после каждого движения объекта
        public void ResetArea()
        {
            for (int i = shape.y; i < shape.y + shape.mSize; i++)
            {
                for (int j = shape.x; j < shape.x + shape.mSize; j++)
                {
                    if (i >= 0 && j >= 0 && i < w && j < h)
                    {
                        if (shape.matrix[i - shape.y, j - shape.x] != 0)
                        {
                            map[i, j] = 0;
                        }
                    }               
                }
            }
        }
        //Отрисовка фигуры на карте
        public void DrawMap(Graphics e)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    switch (map[i, j])
                    {
                        case 1:
                            e.FillRectangle(Brushes.Red, new Rectangle(50 + j * (squareSize) + 1, 50 + i * (squareSize) + 1, squareSize - 1, squareSize - 1));
                            break;
                        case 2:
                            e.FillRectangle(Brushes.Yellow, new Rectangle(50 + j * (squareSize) + 1, 50 + i * (squareSize) + 1, squareSize - 1, squareSize - 1));
                            break;
                        case 3:
                            e.FillRectangle(Brushes.Green, new Rectangle(50 + j * (squareSize) + 1, 50 + i * (squareSize) + 1, squareSize - 1, squareSize - 1));
                            break;
                        case 4:
                            e.FillRectangle(Brushes.Blue, new Rectangle(50 + j * (squareSize) + 1, 50 + i * (squareSize) + 1, squareSize - 1, squareSize - 1));
                            break;
                        case 5:
                            e.FillRectangle(Brushes.Purple, new Rectangle(50 + j * (squareSize) + 1, 50 + i * (squareSize) + 1, squareSize - 1, squareSize - 1));
                            break;
                        case 0:
                            break;
                    }                 
                }
            }
        }
        //Отрисовка следующей фигуры
        public void DrawNextShape(Graphics e)
        {
            for (int i = 0; i < shape.nextmSize; i++)
            {
                for (int j = 0; j < shape.nextmSize; j++)
                {
                    if(shape.nextMatrix[i, j] != 0)
                        e.FillRectangle(Brushes.Gray, new Rectangle(300 + j * (squareSize) + 1, 50 + i * (squareSize) + 1, squareSize - 1, squareSize - 1));
                }
            }
        }
        //Отрисовка сетки
        public void DrawGrid(Graphics g)
        {
            for (int i = 0; i <= w; i++)
            {
                g.DrawLine(Pens.Black, new Point(50, 50+i* squareSize), new Point(50 + 8* squareSize, 50+i* squareSize));
            }
            for (int j = 0; j <= 8; j++)
            {
                g.DrawLine(Pens.Black, new Point(50 + j * squareSize, 50), new Point(50 + j * squareSize, 50 + w* squareSize));
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawNextShape(e.Graphics);
            DrawGrid(e.Graphics);
            
            DrawMap(e.Graphics);
           
        }

    }
}
