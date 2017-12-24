using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze
{
    class Maze
    {
        public const int OBLIQUE = 14; //相邻点与当前点成斜线，G值取为根号2，为方便计算乘以十并取整
        public const int STEP = 10;    //相邻点与当前点成直线，G值取为10
        public int[,] MazeArray { get; private set; }     //定义地图数组
        List<Point> CloseList;          //已检查节点列表
        List<Point> OpenList;           //待检查节点列表
         
        public Maze(int[,] maze)         //构造函数
        {
            this.MazeArray = maze;         //初始化地图数组
            OpenList = new List<Point>(MazeArray.Length);       //初始化待检查节点列表
            CloseList = new List<Point>(MazeArray.Length);      //初始化已检查节点列表
        }

        public Point FindPath(Point start, Point end, bool IsIgnoreCorner)     //寻路函数
        {
            OpenList.Add(start);                 //将节点添加到待检查列表中
            while (OpenList.Count != 0)          //待检查节点列表已被耗尽，所有点已检测完毕，返回
            {
                //找出F值最小的点
                var tempStart = OpenList.MinPoint();      
                //第一个检查的点是待检查列表中F值最小的点，将其设为临时起点tempStart
                OpenList.RemoveAt(0);       //将该点从待检查节点列表中移除
                CloseList.Add(tempStart);    //将该点添加到已检查列表中
                //已该点为临时起点，找出它周围的点，对于2D为四周八个方向的点，SurrroundPoints中自动检查是否有障碍物
                var surroundPoints = SurrroundPoints(tempStart, IsIgnoreCorner);
                foreach (Point point in surroundPoints)       //对于周围的每个点
                {
                    if (OpenList.Exists(point))             //判断该点已在待寻找列表中存在，说明该点之前已可直接抵达
                        //检查如果从当前点tempStart到该点是否更近
                        //计算G值, 如果比原来的大, 就什么都不做, 否则设置它的父节点为当前点,并更新G和F
                        FoundPoint(tempStart, point);       //在FoundPoint函数中进行判断和更新G、F值操作
                    else
                        //如果它们不在开始列表里, 就加入, 在NotFoundPoint函数中设置父节点,并计算GHF
                        NotFoundPoint(tempStart, end, point);
                }
                if (OpenList.Get(end) != null) //在openlist中查询终点位置是否已被探测到，是则返回
                    return OpenList.Get(end);
            }
            return OpenList.Get(end); //openlist已被耗尽，所有点已检测完毕，返回
        }

        private void FoundPoint(Point tempStart, Point point)        
        {
            var G = CalcG(tempStart, point);     //计算从当前点tempStart到该点的G值
            if (G < point.G)                      //如果经过当前点tempStart到该点的G值比从父节点到该点的G值更小
            {
                point.ParentPoint = tempStart;     //将该点的父节点置为当前点tempStart
                point.G = G;                        //更新G值
                point.CalcF();                         //更新F值
            }
        }

        private void NotFoundPoint(Point tempStart, Point end, Point point)      //计算四周点的G值及F值
        {
            point.ParentPoint = tempStart;         //将该四周点的父节点直接置为当前点
            point.G = CalcG(tempStart, point);     //更新G值
            point.H = CalcH(end, point);        //更新H值
            point.CalcF();                       //更新F值
            OpenList.Add(point);                  //将该点添加到待搜索列表中
        }

        private int CalcG(Point start, Point point)    //计算两点间G值
        {
            //由于计算两点间G值的情况只出现在相邻点之间，所以G值增量只有两种可能，斜线或直线，分别表示为10和14
            //这里计算的G值是从起点到当前的最小距离，利用当前点父节点的G值，存疑，parentG似乎应该为
            //int parentG = start.ParentPoint != null ? start.ParentPoint.G : 0;
            int G = (Math.Abs(point.X - start.X) + Math.Abs(point.Y - start.Y)) == 1 ? STEP : OBLIQUE;      
            int parentG = start.ParentPoint != null ? start.ParentPoint.G : 0;      //计算父节点G值
            return G + parentG;               //G值为从start到point的G值加上父节点的G值
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="end"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private int CalcH(Point end, Point point)      //计算H值，此处直接计算曼哈顿距离即可
        {
               int step = Math.Abs(point.X - end.X) + Math.Abs(point.Y - end.Y);
            //对角线估价法
            
       /*
                int cntX = Math.Abs(point.X - end.X);
                int cntY = Math.Abs(point.Y - end.Y);
                // 判断到底是那个轴相差的距离更远 ， 实际上，为了简化计算，我们将代价*10变成了整数。
                if (cntX > cntY)
                {
                    return 14 * cntY + 10 * (cntX - cntY);
                }
                else
                {
                    return 14 * cntX + 10 * (cntY - cntX);
                }
      
            */
            return step * STEP;

        }

        //获取某个点周围可以到达的点
        public List<Point> SurrroundPoints(Point point, bool IsIgnoreCorner)
        {
            var surroundPoints = new List<Point>(9);                   //周围可到达的点，其实只有八个

            for(int x = point.X -1; x <= point.X+1;x++)                //在X、Y坐标正负1的范围内
                for (int y = point.Y - 1; y <= point.Y + 1; y++)
                {
                    if (CanReach(point,x, y,IsIgnoreCorner))          //判断能否抵达，即判断是否有障碍物
                        surroundPoints.Add(x, y);                      //将该点添加到周围点中
                }
            return surroundPoints;
        }

        //在二维数组对应的位置不为障碍物
        private bool CanReach(int x, int y)
        {
            return MazeArray[x, y] == 0;            //在数组中的值为0表示可以抵达
        }

        public bool CanReach(Point start, int x, int y, bool IsIgnoreCorner)        //函数重载，
        {
            if (!CanReach(x, y) || CloseList.Exists(x, y))
                return false;
            else
            {
                if (Math.Abs(x - start.X) + Math.Abs(y - start.Y) == 1)
                    return true;
                //如果是斜方向移动, 判断是否 "拌脚"
                else
                {
                    //if (CanReach(Math.Abs(x - 1), y) && CanReach(x, Math.Abs(y - 1)))
                    if (CanReach(Math.Abs(x - 1), y) && CanReach(x, Math.Abs(y - 1)))
                        //if (CanReach(start.x, y) && CanReach(x, start.y))
                        return true;
                    else
                        return IsIgnoreCorner;
                }
            }
        }
    }

    //Point 类型
    public class Point
    {
        public Point ParentPoint { get; set; }      //链表，路径结构，指向父节点
        public int F { get; set; }  //F=G+H
        public int G { get; set; }
        public int H { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public void CalcF()
        {
            this.F = this.G + this.H;
        }
    }

    //对 List<Point> 的一些扩展方法
    public static class ListHelper
    {
        public static bool Exists(this List<Point> points, Point point)
        {
            foreach (Point p in points)
                if ((p.X == point.X) && (p.Y == point.Y))
                    return true;
            return false;
        }

        public static bool Exists(this List<Point> points, int x, int y)
        {
            foreach (Point p in points)
                if ((p.X == x) && (p.Y == y))
                    return true;
            return false;
        }

        //给出最小值
        public static Point MinPoint(this List<Point> points)
        {
            points = points.OrderBy(p => p.F).ToList();
            return points[0];
        }
        public static void Add(this List<Point> points, int x, int y)
        {
            Point point = new Point(x, y);
            points.Add(point);
        }

        public static Point Get(this List<Point> points, Point point)
        {
            foreach (Point p in points)
                if ((p.X == point.X) && (p.Y == point.Y))
                    return p;
            return null;
        }

        public static void Remove(this List<Point> points, int x, int y)
        {
            foreach (Point point in points)
            {
                if (point.X == x && point.Y == y)
                    points.Remove(point);
            }
        }
    }
}
