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
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using System.Threading;
using System.ComponentModel;

namespace PokemonMadjong
{
    public class SettingsData: INotifyPropertyChanged, INotifyPropertyChanging
    {

        int player1Score = 0;
        ObservableCollection<int> grid;

        public ObservableCollection<int> Grid
        {
            get
            {
                return grid;
            }
            set
            {
                NotifyPropertyChanging("Grid");
                grid = value;
                NotifyPropertyChanged("Grid");
            }
        }

        public int Player1Score
        {
            get
            {
                return player1Score;
            }
            set
            {
                NotifyPropertyChanging("Player1Score");
                player1Score = value;
                NotifyPropertyChanged("Player1Score");
            }
        }
        public string Player1Name { get; set; }
        public int FieldWidth { get; set; }
        public int FieldHeight { get; set; }
        public int CurGameN { get; set; }
        public TimeSpan GameTime { get; set; }

        public SettingsData()
        {
            Grid = new ObservableCollection<int>();
            Player1Score = 0;
            Player1Name = "Player1";
            FieldWidth = 10;
            FieldHeight = 5;
            CurGameN = 0;
            GameTime = new TimeSpan(0, 10, 0);
            grid.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(grid_CollectionChanged);
        }

        void grid_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Grid");
        }

        public void Clear()
        {
            Grid = new ObservableCollection<int>();
            Player1Score = 0;
            Player1Name = "Player1";
            FieldWidth = 10;
            FieldHeight = 5;
            CurGameN = 0;
            GameTime = new TimeSpan(0, 10, 0);
            grid.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(grid_CollectionChanged);
        }

        public void Load()
        {
        }

        public void Save()
        {

        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
