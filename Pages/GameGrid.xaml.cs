using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Windows.Threading;

namespace PokemonMadjong.Pages
{
    public partial class GameGrid : UserControl//, INotifyPropertyChanged
    {

        public event EventHandler EndGame;
        public event EventHandler NextLevel;
        bool gameEnd = false;
        

        private List<Border> selectedBorderList = new List<Border>(10);
        List<BitmapImage> sprites = new List<BitmapImage>(13);
        List<Path> pathList = new List<Path>();
        Path curPath = null;
        int debugLevel = 0;

        DispatcherTimer myDispatcherTimer = new DispatcherTimer();

        public int FirstSelect { get; set; }
        public int SecondSelect { get; set; }
        Color backgroundColor = Colors.White;//new HexColor("#FFD0691E");
        Random random;

        public SettingsData Settings
        {
            get
            {
                //return testGrid;
                return (Application.Current as PokemonMadjong.App).Settings;
            }
        }

        protected void OnEndGame(EventArgs e)
        {
            EventHandler handler = EndGame;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnNextLevel(EventArgs e)
        {
            EventHandler handler = NextLevel;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public GameGrid()
        {
            // Required to initialize variables
            random = new Random((int)DateTime.UtcNow.Ticks);
            DataContext = this;
            InitializeComponent();
            Init();
            LoadBitmap();

            GenerateField();

            myDispatcherTimer.Interval = new TimeSpan(0,0,0,0,200);
            myDispatcherTimer.Tick += new EventHandler(myDispatcherTimer_Tick);
        }

        void myDispatcherTimer_Tick(object sender, EventArgs e)
        {
            myDispatcherTimer.Stop();
            Settings.Grid[FirstSelect] = -1;
            Settings.Grid[SecondSelect] = -1;
            var FirstBorder = GetBorder(FirstSelect);
            (FirstBorder.Child as Grid).Children.RemoveAt(0);
            FirstBorder.Background = new SolidColorBrush(Colors.Transparent);


            var SecondBorder = GetBorder(SecondSelect);
            //SecondBorder.Child = null;
            (SecondBorder.Child as Grid).Children.RemoveAt(0);
            SecondBorder.Background = new SolidColorBrush(Colors.Transparent);
            selectedBorderList.Clear();

            switch (Settings.CurGameN)
            {
                case 0:
                    ClearPath();
                    break;
                case 1:
                    UpColumn();
                    break;
                case 2:
                    DownColumn();
                    break;
                case 3:
                    LeftRow();
                    break;
                case 4:
                    RightRow();
                    break;
                default:
                    if (Settings.CurGameN % 2 > 0)
                    {
                        UpColumn();
                    }
                    else
                    {
                        RightRow();
                    }
                    break;
            }
            //UpColumn();
            //DownColumn();
            //LeftRow();
            //RightRow();
            SetSelectedBordersColor(backgroundColor);
            selectedBorderList.Clear();
            FirstSelect = -1;
            SecondSelect = -1;

            bool temp = ChekGrid();
            //SetBorderColor(FirstSelect, Colors.Green);
            //SetBorderColor(SecondSelect, Colors.Green);
            //Debug.WriteLine("First: {0}  Second:{1} ",FirstSelect,SecondSelect);
            if (IsGridEmpty())
            {
                //end
                //(Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/GameOver.xaml", UriKind.Relative));
                OnNextLevel(new EventArgs());
                return;
            }

            if (!temp)
            {
                SortGrid();
                GenerateField();
            }
            FirstSelect = -1;
            SecondSelect = -1;

        }

        void GenerateField()
        {
            LayoutRoot.Children.Clear();
            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Clear();

            //первая пустая маленькая
            
            for (int row = 0; row < Settings.FieldHeight; row++)
            {
                var rd = new RowDefinition() { Height = new GridLength(1, (row == 0 || row == Settings.FieldHeight - 1) ? GridUnitType.Auto : GridUnitType.Star) };
                LayoutRoot.RowDefinitions.Add(rd);
            }
            //последняя пустая маленькая
            //rd = new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) };
            //LayoutRoot.RowDefinitions.Add(rd);

            //первая пустая маленькая
            
            for (int row = 0; row < Settings.FieldWidth; row++)
            {
                var cd = new ColumnDefinition() { Width = new GridLength(1, (row == 0 || row == Settings.FieldWidth-1)?GridUnitType.Auto:GridUnitType.Star) };
                LayoutRoot.ColumnDefinitions.Add(cd);
            }

            for (int row = 0; row < Settings.FieldHeight; row++)
            {
                for (int column = 0; column < Settings.FieldWidth; column++)
                {
                    int n = column + Settings.FieldWidth * row;
                    Border border = new Border();
                    border.Tag = n;
                    border.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Border_Tap);
                    //border.BorderBrush = new SolidColorBrush(Colors.Black);
                    Grid childGrid = new Grid();
                    border.Child = childGrid;
                    if (Settings.Grid[n] != -1)
                    {
                        Image img = new Image();
                        img.Source = sprites[Settings.Grid[n]];
                        
                        childGrid.Children.Add(img);
                    }
                    else
                    {
                        border.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    border.SetValue(System.Windows.Controls.Grid.ColumnProperty, column);
                    border.SetValue(System.Windows.Controls.Grid.RowProperty, row);
                    LayoutRoot.Children.Add(border);

                }
            }
        }

        void LoadBitmap()
        {
            //37 - horiozontal
            //38 - vertical
            //39 - half_down
            //40 - half_left
            //41 - half_right
            //42 - half_up
            //43 - bottomleft
            //44 - bottomright
            //45 - topleft
            //46 - topright

            for (int i = 0; i < 48; i++)
            {
                sprites.Add(new BitmapImage(new Uri("/PokemonMadjong;component/Content/"+i.ToString()+".png", UriKind.Relative)));
            }
        }

        public void Init()
        {
            if (Settings.Grid.Count == 0)
            {
                int n = 0;
                for (int i = 0; i < Settings.FieldHeight; i++)
                {
                    if (i == 0 || i == Settings.FieldHeight-1)
                    {
                        for (int j = 0; j < Settings.FieldWidth; j++)
                        {
                            Settings.Grid.Add(-1);
                        }
                    }
                    else
                    {
                        Settings.Grid.Add(-1);
                        for (int j = 0; j < Settings.FieldWidth-2; j++)
                        {
                            Settings.Grid.Add(n);
                            n++;
                            if (n > 35) n = 0;
                        }
                        Settings.Grid.Add(-1);
                    }
                }
                SortGrid();
            }
           
            FirstSelect = -1;
            SecondSelect = -1;

        }

        void SortGrid()
        {
            pathList.Clear();
            curPath = null;
            int numIterations = 5;

            do
            {
                Debug.WriteLine("Sort");
                int n,n2;
                for (int i = 0; i < Settings.Grid.Count * 1; i++)
                {
                    do
                    {
                        n = random.Next(Settings.Grid.Count);
                    } while (Settings.Grid[n] == -1);
                    
                    do{
                    n2 = random.Next(Settings.Grid.Count);
                    } while (Settings.Grid[n2] == -1);

                    int temp = Settings.Grid[n];
                    Settings.Grid[n] = Settings.Grid[n2];
                    Settings.Grid[n2] = temp;
                }

                numIterations--;
                if (numIterations < 0)
                {
                    gameEnd = true;
                    break;
                }
            } while (!ChekGrid());

            if (gameEnd)
            {
                OnEndGame(new EventArgs());
            }
        }

        bool ChekGrid()
        {
            for (int i = 0; i < Settings.Grid.Count; i++)
            {
                if (Settings.Grid[i] == -1) continue;
                FirstSelect = i;
                if (Calculate())
                {
                    //FirstSelect = -1;
                    return true;
                }
            }

            //FirstSelect = -1;
            return false;
        }

        bool IsGridEmpty()
        {
            for (int i = 0; i < Settings.Grid.Count; i++)
            {
                if (Settings.Grid[i] != -1)
                {
                    return false;
                }
            }

            return true;
        }

        void DrawPatch()
        {
            int row1 = FirstSelect / Settings.FieldWidth;
            int col1 = FirstSelect - row1 * Settings.FieldWidth;

            int row2 = SecondSelect / Settings.FieldWidth;
            int col2 = SecondSelect - row2 * Settings.FieldWidth;

            //выберем самый коротки
            var orderPathList = from item in pathList
                                orderby item.Count
                                select item;

            curPath = orderPathList.First();
            //curPath.Add(SecondSelect);
            for (int i = 0; i < curPath.Count; i++)
            {
                Image img = new Image();
                img.Source = sprites[47];
                img.Stretch = Stretch.Fill;
                (GetBorder(curPath[i].Index).Child as Grid).Children.Add(img);
            }
            //MessageBox.Show("OK");
        }

        void ClearPath()
        {
            //foreach (var item in path)
            //{
            //    (GetBorder(item.Index).Child as Grid).Children.RemoveAt(0);
            //}
            //path.Clear();
            if (curPath == null) return;

            for (int i = 0; i < curPath.Count; i++)
            {
                var children = (GetBorder(curPath[i].Index).Child as Grid).Children;
                if (children.Count == 1 && Settings.Grid[curPath[i].Index]==-1)
                {
                    children.RemoveAt(0);
                }
            }
            pathList.Clear();
            curPath = null;
        }

        public void Grid_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var border = sender as Border;
            int index = (int)border.Tag;
            if (Settings.Grid[index] == -1) return;

            //int row = index / Settings.FieldWidth;
            //int col = index - row * Settings.FieldWidth;
            if (backgroundColor == Colors.White)
            {
                backgroundColor = ((SolidColorBrush) border.Background).Color;
            }
            border.Background = new SolidColorBrush(Colors.Red);
            selectedBorderList.Add(sender as Border);

            if (FirstSelect == -1)
            {
                FirstSelect = (int)border.Tag;
                
            }
            else if(SecondSelect == -1)
            {
                bool timer = false;
                SecondSelect = (int)border.Tag;

                if (Settings.Grid[FirstSelect] != Settings.Grid[SecondSelect] || FirstSelect == SecondSelect)
                {

                }
                else if (!Calculate())
                {

                }
                else
                {
                    DrawPatch();
                    Settings.Player1Score += 20;
                    myDispatcherTimer.Start();
                    timer = true;
                }

                if (!timer)
                {
                    SetSelectedBordersColor(backgroundColor);
                    selectedBorderList.Clear();
                    FirstSelect = -1;
                    SecondSelect = -1;

                    bool temp = ChekGrid();
                   
                    if (IsGridEmpty())
                    {
                      
                        OnNextLevel(new EventArgs());
                        return;
                    }

                    if (!temp)
                    {
                        SortGrid();
                        GenerateField();
                    }
                    FirstSelect = -1;
                    SecondSelect = -1;
                } 
            }

            
            
        }

        bool Calculate()
        {
            pathList.Clear();
            debugLevel = 0;

            if (SecondSelect == FirstSelect) return false;

            Path newPath = new Path();
            newPath.Add(FirstSelect);

            List<Path> tempList = Walk(FirstSelect, Directional.Down, newPath.Copy());
            if (tempList != null)
            {
                foreach (var item in tempList)
                {
                    pathList.Add(item);
                }
                if (pathList.Count > 0) return true;
            }

            tempList = Walk(FirstSelect, Directional.Left, newPath.Copy());
            if (tempList != null)
            {
                foreach (var item in tempList)
                {
                    pathList.Add(item);
                }
                if (pathList.Count > 0) return true;
            }

            tempList = Walk(FirstSelect, Directional.Right, newPath.Copy());
            if (tempList != null)
            {
                foreach (var item in tempList)
                {
                    pathList.Add(item);
                }
                if (pathList.Count > 0) return true;
            }

            tempList = Walk(FirstSelect, Directional.Up, newPath.Copy());
            if (tempList != null)
            {
                foreach (var item in tempList)
                {
                    pathList.Add(item);
                }
                if (pathList.Count > 0) return true;
            }

            if (pathList.Count > 0) return true;

            return false;
        }


        List<Path> Walk(int fromIndex, Directional directional, Path path)
        {
            debugLevel++;
            int row = fromIndex / Settings.FieldWidth;
            int col = fromIndex - row * Settings.FieldWidth;
            switch (directional)
            {
                case Directional.Down:
                    DebugPath("Down", debugLevel);
                    row++;
                    if (row == Settings.FieldHeight)
                    {
                        debugLevel--;
                        return null;
                    }
                    break;
                case Directional.Left:
                    DebugPath("Left", debugLevel);
                    col--;
                    if (col == -1)
                    {
                        debugLevel--;
                        return null;
                    }
                    break;
                case Directional.Right:
                    DebugPath("Right", debugLevel);
                    col++;
                    if (col == Settings.FieldWidth)
                    {
                        debugLevel--;
                        return null;
                    }
                    break;
                case Directional.Up:
                    DebugPath("Up", debugLevel);
                    row--;
                    if (row == -1)
                    {
                        debugLevel--;
                        return null;
                    }
                    break;
            }
        
            int index = col + Settings.FieldWidth * row;
            if (path.Contains(index))
            {
                DebugPath("-Double", debugLevel);
                debugLevel--;
                return null;
            }
            List<Path> temp = new List<Path>();

            if (SecondSelect != -1 && SecondSelect == index)
            {
                DebugPath("-Find OK", debugLevel);
                path.Add(index);
                if (path.NumAngle <= 2)
                {
                    temp.Add(path);
                    debugLevel--;
                    return temp;
                }
                
            }
            else if (SecondSelect == -1 && Settings.Grid[FirstSelect] == Settings.Grid[index])
            {
                path.Add(index);
                if (path.NumAngle <= 2)
                {
                    SecondSelect = index;
                    temp.Add(path);
                    debugLevel--;
                    return temp;
                }
            }
 
            if (Settings.Grid[index] == -1)
            {
                
                path.Add(index);
                if (path.NumAngle > 2)
                {
                    //Debug.WriteLine("--- Angle > 2 break");
                    DebugPath("- Angle > 2", debugLevel);
                    debugLevel--;
                    return null;
                }

                List<Path> tempList = Walk(index, Directional.Down, path.Copy());
                if (tempList != null)
                {
                    foreach (var item in tempList)
                    {
                        temp.Add(item);
                    }
                    //для увеличения скорости, найдем первый путь и все
                    if (temp.Count > 0) return temp;
                    //debugLevel--;
                    //return temp;
                }
                tempList = Walk(index, Directional.Left, path.Copy());
                if (tempList != null)
                {
                    foreach (var item in tempList)
                    {
                        temp.Add(item);
                    }
                    if (temp.Count > 0) return temp;
                    //debugLevel--;
                    //return temp;
                }
                tempList = Walk(index, Directional.Right, path.Copy());
                if (tempList != null)
                {
                    foreach (var item in tempList)
                    {
                        temp.Add(item);
                    }
                    if (temp.Count > 0) return temp;
                    //debugLevel--;
                    //return temp;
                }
                tempList = Walk(index, Directional.Up, path.Copy());
                if (tempList != null)
                {
                    foreach (var item in tempList)
                    {
                        temp.Add(item);
                    }
                    if (temp.Count > 0) return temp; 
                    //debugLevel--;
                    //return temp;
                }
                debugLevel--;
                return temp;
            }
            //Debug.WriteLine("NO PATH");
            debugLevel--;
            return null;
        }

        void DebugPath(string text,int level)
        {
            return;
            //string temp = "";
            //for (int i = 0; i < level; i++)
            //{
            //    temp += "-";
            //}
            //Debug.WriteLine(temp+text);
        }
        private void SetBorderColor(int index, Color color)
        {
            foreach (var control in LayoutRoot.Children)
            {
                if (control is Border)
                {
                    //отметим голубым букву и розовым свободные поля вокруг
                    if ((control as Border).Tag.ToString() == index.ToString())
                    {
                        (control as Border).Background = new SolidColorBrush(color);
                        break;
                    }
                }
            }
        }

        Border GetBorder(int index)
        {
            foreach (var control in LayoutRoot.Children)
            {
                if (control is Border)
                {
                    //отметим голубым букву и розовым свободные поля вокруг
                    if ((control as Border).Tag.ToString() == index.ToString())
                    {
                        return (control as Border);
                    }
                }
            }

            return null;
        }

        public void SetSelectedBordersColor(Color color)
        {
            foreach (Border border in selectedBorderList)
            {
                border.Background = new SolidColorBrush(color);
            }
        }

        void UpColumn()
        {
            int row1 = FirstSelect / Settings.FieldWidth;
            int col1 = FirstSelect - row1 * Settings.FieldWidth;
            int row2 = SecondSelect / Settings.FieldWidth;
            int col2 = SecondSelect - row2 * Settings.FieldWidth;

            //up col1
            for (int i = 1; i < Settings.FieldHeight-2; i++)
            {
                int index = col1 + Settings.FieldWidth * i;
                if (Settings.Grid[index] != -1) continue;

                for (int j = i; j < Settings.FieldHeight-1; j++)
                {
                    int index2 = col1 + Settings.FieldWidth * j;
                    int index3 = col1 + Settings.FieldWidth * (j+1);
                    Settings.Grid[index2] = Settings.Grid[index3];

                }
            }

            //up col2
            for (int i = 1; i < Settings.FieldHeight - 2; i++)
            {
                int index = col2 + Settings.FieldWidth * i;
                if (Settings.Grid[index] != -1) continue;

                for (int j = i; j < Settings.FieldHeight - 1; j++)
                {
                    int index2 = col2 + Settings.FieldWidth * j;
                    int index3 = col2 + Settings.FieldWidth * (j + 1);

                    Settings.Grid[index2] = Settings.Grid[index3];

                }
            }

            GenerateField();

        }

        void DownColumn()
        {
            int row1 = FirstSelect / Settings.FieldWidth;
            int col1 = FirstSelect - row1 * Settings.FieldWidth;
            int row2 = SecondSelect / Settings.FieldWidth;
            int col2 = SecondSelect - row2 * Settings.FieldWidth;

            //up col1
            for (int i = Settings.FieldHeight - 2; i > 1; i--)
            {
                int index = col1 + Settings.FieldWidth * i;
                if (Settings.Grid[index] != -1) continue;

                for (int j = i; j >0 ; j--)
                {
                    int index2 = col1 + Settings.FieldWidth * j;
                    int index3 = col1 + Settings.FieldWidth * (j - 1);
                    Settings.Grid[index2] = Settings.Grid[index3];

                }
            }

            //up col2
            for (int i = Settings.FieldHeight - 2; i > 1; i--)
            {
                int index = col2 + Settings.FieldWidth * i;
                if (Settings.Grid[index] != -1) continue;

                for (int j = i; j > 0; j--)
                {
                    int index2 = col2 + Settings.FieldWidth * j;
                    int index3 = col2 + Settings.FieldWidth * (j - 1);
                    Settings.Grid[index2] = Settings.Grid[index3];

                }
            }

            GenerateField();

        }

        void LeftRow()
        {
            int row1 = FirstSelect / Settings.FieldWidth;
            int col1 = FirstSelect - row1 * Settings.FieldWidth;
            int row2 = SecondSelect / Settings.FieldWidth;
            int col2 = SecondSelect - row2 * Settings.FieldWidth;

            //up row1
            for (int i = 1; i < Settings.FieldWidth - 2; i++)
            {
                int index = i + Settings.FieldWidth * row1;
                if (Settings.Grid[index] != -1) continue;

                for (int j = i; j < Settings.FieldWidth - 1; j++)
                {
                    int index2 = j + Settings.FieldWidth * row1;
                    int index3 = (j + 1) + Settings.FieldWidth * row1;

                    Settings.Grid[index2] = Settings.Grid[index3];

                }
            }

            //up row1
            for (int i = 1; i < Settings.FieldWidth - 2; i++)
            {
                int index = i + Settings.FieldWidth * row2;
                if (Settings.Grid[index] != -1) continue;

                for (int j = i; j < Settings.FieldWidth - 1; j++)
                {
                    int index2 = j + Settings.FieldWidth * row2;
                    int index3 = (j + 1) + Settings.FieldWidth * row2;

                    Settings.Grid[index2] = Settings.Grid[index3];

                }
            }
            GenerateField();
        }

        void RightRow()
        {
            int row1 = FirstSelect / Settings.FieldWidth;
            int col1 = FirstSelect - row1 * Settings.FieldWidth;
            int row2 = SecondSelect / Settings.FieldWidth;
            int col2 = SecondSelect - row2 * Settings.FieldWidth;

            for (int i = Settings.FieldWidth - 2; i > 1; i--)
            {
                int index = i + Settings.FieldWidth * row1;
                if (Settings.Grid[index] != -1) continue;

                for (int j = i; j > 0; j--)
                {
                    int index2 = j + Settings.FieldWidth * row1;
                    int index3 = (j - 1) + Settings.FieldWidth * row1;
                    Settings.Grid[index2] = Settings.Grid[index3];

                }
            }

            for (int i = Settings.FieldWidth - 2; i > 1; i--)
            {
                int index = i + Settings.FieldWidth * row2;
                if (Settings.Grid[index] != -1) continue;

                for (int j = i; j > 0; j--)
                {
                    int index2 = j + Settings.FieldWidth * row2;
                    int index3 = (j - 1) + Settings.FieldWidth * row2;
                    Settings.Grid[index2] = Settings.Grid[index3];

                }
            }
            GenerateField();
        }

    }
}