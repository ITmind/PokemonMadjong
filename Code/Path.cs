using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace PokemonMadjong
{
    public class Path
    {
        List<PatchItem> path = new List<PatchItem>();

        public int NumAngle
        {
            get
            {
                int num = 0;
 
                for (int i = 2; i < path.Count; i++)
                {
                    if (path[i-2].Col != path[i].Col && path[i-2].Row != path[i].Row)
                    {
                        num++;
                    }
                }

                return num;
            }
        }

        public PatchItem this[int key]
        {
            get
            {
                return path[key];
            }
        }

        public int Count
        {
            get
            {
                return path.Count;
            }
        }

        public Path()
        {
        }

        public void Add(int index)
        {
            path.Add(new PatchItem(index));
        }

        public void Add(int row,int col)
        {
            path.Add(new PatchItem(row,col));
        }

        public Path Copy()
        {
            Path temp = new Path();
            foreach (var item in path)
            {
                temp.Add(item.Index);
            }
            return temp;
        }

        public bool Contains(int index)
        {
            foreach (var item in path)
            {
                if (item.Index == index)
                {
                    return true;
                }
            }
            return false;
        }
    }

    enum Directional
    {
        Up,
        Left,
        Right,
        Down
    }
}
