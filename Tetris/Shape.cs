using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Shape
    {
        public int x;
        public int y;
        public int[,] matrix;
        public int[,] nextMatrix;
        public int mSize;
        public int nextmSize;
        /*Инициализация всех возможных форм*/
        public int[,] iShape = new int[4, 4]
        {
            {0,0,1,0},
            {0,0,1,0},
            {0,0,1,0},
            {0,0,1,0}
        };

        public int[,] lShape = new int[3, 3]
        {
            {2,0,0},
            {2,0,0},
            {2,2,0}
        };

        public int[,] tShape = new int[3, 3]
        {
                {0,0,0},
                {3,3,3},
                {0,3,0}
        };

        public int[,] zShape = new int[3, 3]
        {
                {0,4,0},
                {0,4,4},
                {0,0,4}
        };

        public int[,] sShape = new int[2, 2]
        {
                {5,5},
                {5,5},
        };
        /*Инициализация всех возможных форм*/

        //Консутруктор новой фигуры
        public Shape(int _x, int _y)
        {
            x = _x;
            y = _y;
            matrix = generateShape();
            mSize = (int)Math.Sqrt(matrix.Length);
            nextMatrix = generateShape();
            nextmSize = (int)Math.Sqrt(nextMatrix.Length);
        }

        //Генерация следующей по порядку фигуры, которая отображается в правом верхнем углу
        public void genetateNextShape(int _x, int _y)
        {
            x = _x;
            y = _y;
            matrix = nextMatrix;
            mSize = (int)Math.Sqrt(matrix.Length);
            nextMatrix = generateShape();
            nextmSize = (int)Math.Sqrt(nextMatrix.Length);
        }
        //Генерация случайной фигуры
        public int[,] generateShape()
        {
            int[,] _matrix = sShape;
            Random s = new Random();
            switch (s.Next(1,6))
            {
                case 1:
                    _matrix = iShape;                  
                    break;
                case 2:
                    _matrix = lShape;
                    break;
                case 3:
                    _matrix = tShape;
                    break;
                case 4:
                    _matrix = zShape;
                    break;
                case 5:
                    _matrix = sShape;
                    break;
            }
            return _matrix;
        }
        //Поворот
        public void rotate() 
        {
            int[,] rotateMatrix = new int[mSize, mSize];

            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < mSize; j++)
                {
                    rotateMatrix[i, j] = matrix[j, (mSize - 1) - i];
                } 
            }

            //Если фигура выходит за карту справа при повороте - сместить её влево на то количество клеток, на которое она выходит
            matrix = rotateMatrix;
            int offset = (10 - (x + mSize));
            if (offset < 0)
            {
                for (int i = 0; i < Math.Abs(offset); i++)
                {
                    moveLeft();
                }

            }
            //Если фигура выходит за карту слева при повороте - сместить её вправо на то количество клеток, на которое она выходит
            if (x < 0)
            {
                for (int i = 0; i < Math.Abs(x) + 1; i++)
                {
                    moveRight();
                }
            }
        }
        public void moveDown()
        {
            y++;
        }
        public void moveRight()
        {

            x++;
        }
        public void moveLeft()
        {
            x--;
        }
    }
}
