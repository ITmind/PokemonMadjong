using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;

namespace PokemonMadjong.Pages
{
    public partial class Game : PhoneApplicationStylePage
    {
        DispatcherTimer myDispatcherTimer = new DispatcherTimer();
        GameGrid gameGrid;
        ScaleTransform scale;
        double _initialScale = 0;
        TimeSpan currentTime = new TimeSpan(0, 0, 0);
        TimeSpan seconds = new TimeSpan(0, 0, 1);

        public Game()
        {
            DataContext = this;
            InitializeComponent();
            Settings.FieldHeight = 11;//9;
            Settings.FieldWidth = 18;//16;
            //Settings.FieldHeight = 5;//9;
            //Settings.FieldWidth = 5;//16;
            CreateGameGrid();
        }

        void gameGrid_NextLevel(object sender, EventArgs e)
        {
            myDispatcherTimer.Stop();
            Settings.Player1Score += (int)currentTime.TotalSeconds * 100; 
            Settings.CurGameN++;
            gameGrid = null;
            Settings.GameTime = new TimeSpan(0, Settings.CurGameN<8?10 - Settings.CurGameN:3, 0);
            CreateGameGrid();
            //NavigationService.Navigate(new Uri("/Pages/GameOver.xaml", UriKind.Relative));
        }

        void CreateGameGrid()
        {
            currentTime = new TimeSpan(0, 0, 0);
            //Settings.Grid.Clear();

            gameGrid = new GameGrid();
            gameGrid.NextLevel += new EventHandler(gameGrid_NextLevel);
            gameGrid.EndGame += new EventHandler(gameGrid_EndGame);
            gameGrid.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(gameGrid_ManipulationDelta);
            var gl = GestureService.GetGestureListener(gameGrid);
            //gl.PinchStarted += new EventHandler<PinchStartedGestureEventArgs>(gl_PinchStarted);
            //gl.PinchDelta += new EventHandler<PinchGestureEventArgs>(gl_PinchDelta);
            gl.DoubleTap += new EventHandler<Microsoft.Phone.Controls.GestureEventArgs>(gl_DoubleTap);
            scale = new ScaleTransform();
            scale.ScaleX = 1.0;
            scale.ScaleY = 1.0;
            gameGrid.RenderTransform = scale;

            ContentPanel.Children.Add(gameGrid);

            tbTime.Text = String.Format("{0:D2}:{1:D2}", Settings.GameTime.Minutes, Settings.GameTime.Seconds);
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            myDispatcherTimer.Tick += new EventHandler(Each_Tick);
            myDispatcherTimer.Start();
        }

        void gameGrid_EndGame(object sender, EventArgs e)
        {
            myDispatcherTimer.Stop();
            Settings.Player1Score += (int)currentTime.TotalSeconds * 100;
            NavigationService.Navigate(new Uri("/Pages/GameOver.xaml", UriKind.Relative));
        }

        void gl_DoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            if (gameGrid.Height == 480 * 2)
            {
                gameGrid.Height = double.NaN;
                gameGrid.Width = double.NaN;
            }
            else
            {
                gameGrid.Height = 480 * 2;
                gameGrid.Width = 800 * 2;
            }
            //scale.ScaleX = 2;
            //scale.ScaleY = 2;
        }

        void gl_PinchDelta(object sender, PinchGestureEventArgs e)
        {
            scale.ScaleX = _initialScale * e.DistanceRatio;
            scale.ScaleY = scale.ScaleX;
            //ContentPanel.Width = 800 * scale.ScaleX;
            //ContentPanel.Height = 480 * scale.ScaleX;
            //UpdateLayout();
        }

        void gl_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _initialScale = scale.ScaleX;

            // calculate the center for the zooming
            Point firstTouch = e.GetPosition(gameGrid, 0);
            Point secondTouch = e.GetPosition(gameGrid, 1);

            Point center = new Point(firstTouch.X + (secondTouch.X - firstTouch.X) / 2.0,
                                        firstTouch.Y + (secondTouch.Y - firstTouch.Y) / 2.0);

            scale.CenterX = center.X;
            scale.CenterY = center.Y;

            //UpdateLayout();
        }

        void gameGrid_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            
        }

        public void Each_Tick(object o, EventArgs sender)
        {
            currentTime = currentTime.Add(seconds);
            TimeSpan temp = Settings.GameTime.Subtract(currentTime);
            tbTime.Text = String.Format("{0:D2}:{1:D2}", temp.Minutes, temp.Seconds);
            if (currentTime >= Settings.GameTime)
            {
                myDispatcherTimer.Stop();
                NavigationService.Navigate(new Uri("/Pages/GameOver.xaml", UriKind.Relative));
            }
        }
    }
}