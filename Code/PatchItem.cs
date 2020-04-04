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

namespace PokemonMadjong
{
    public class PatchItem
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Index
        {
            get
            {
                return Col + (Application.Current as PokemonMadjong.App).Settings.FieldWidth * Row;
            }
        }
        public PatchItem(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public PatchItem(int index)
        {
            Row = index / (Application.Current as PokemonMadjong.App).Settings.FieldWidth;
            Col = index - Row * (Application.Current as PokemonMadjong.App).Settings.FieldWidth;
        }
    }
}
