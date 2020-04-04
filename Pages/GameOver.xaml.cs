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
using Mogade;
using Mogade.WindowsPhone;
using System.Collections.ObjectModel;

namespace PokemonMadjong.Pages
{
    public partial class GameOver : PhoneApplicationStylePage
    {        
        private int _score;

        public GameOver()
        {
            DataContext = this;
            InitializeComponent();
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            UserName.Text = Settings.Player1Name;
            _score = Settings.Player1Score;
            textBlock1.Text = String.Format("You score: {0}", _score);
         
            base.OnNavigatedTo(e);
            Settings.Grid.Clear();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
          
            Settings.Player1Name = UserName.Text;
            
            if (string.IsNullOrEmpty(UserName.Text))
            {
                return;
            }
            Submit.IsEnabled = false;
            Submit.Content = "Sending...";

            //we can store a bit of arbitrary data in the Data field which we can then use in our leaderboard
            //you can always not set the Data if you have no extra information you want to store with the score
            var score = new Score { Points = _score, UserName = UserName.Text };
            Mogade.SaveScore(MogadeHelper.LeaderboardId(Leaderboards.Main), score, ScoreResponseHandler);
        }

        private void ScoreResponseHandler(Response<SavedScore> r)
        {
            if (!r.Success)
            {
                //todo handle error
                //we are in a separate thread, we can't just update controls from the main thread.
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error connect to Mogade");
                });

            }
            else
            {
            }

            //удалим game over
            //удалим game
            //удалим main menu
            Dispatcher.BeginInvoke(() =>
                {
                    //очистим таблицу
                    //Settings.Grid = new ObservableCollection<string>();

                    //и сделаем невозможность вернуться на игровое поле
                    for (int i = 0; i < 2; i++)
                    {
                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.RemoveBackEntry();
                        }
                    }

                    //NavigationService.GoBack();            
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
        }
    }
}