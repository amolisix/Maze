using System;

namespace Maze
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] array = {
               { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
               { 1, 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1},
               { 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1},
               { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1},
               { 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1},
               { 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1},
               { 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1},
               { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
               };
            Maze maze = new Maze(array);       //初始化地图数组
            Point start = new Point(1, 1);     //设置起点
            Point end = new Point(6, 10);      //设置终点
            var parent = maze.FindPath(start, end, false);      //寻找路径

            Console.WriteLine("Print path:");           //打印路径
            while (parent != null)                     //从终点依次寻找父节点，最终到达起点，并依次打印
            {
                array[parent.X, parent.Y] = 3;
                Console.WriteLine(parent.X + ", " + parent.Y);
                parent = parent.ParentPoint;
            }
            int i;
            for (i = 0; i <= 7; i++)
            {
                int j;
                for (j = 0; j <= 11; j++)
                {
                    Console.Write("{0,0}", array[i,j]);
                    
                }
                Console.WriteLine(' ');
            }
            string str;
            str = Console.ReadLine();
        }
    }
}