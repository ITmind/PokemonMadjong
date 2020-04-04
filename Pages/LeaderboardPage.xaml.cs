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

namespace PokemonMadjong
{
    public partial class LeaderboardPage : PhoneApplicationStylePage
    {
        private static readonly LeaderboardScope[] _scopeOrder = new[] { LeaderboardScope.Overall, LeaderboardScope.Weekly, LeaderboardScope.Daily };
        private LeaderboardScope _scope;
        private int _page;

        public LeaderboardPage()
        {
            DataContext = this;
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _scope = _scopeOrder[0];
            _page = 1;
            LoadLeaderboard();
            base.OnNavigatedTo(e);
        }

        private void LoadLeaderboard()
        {
            _performanceProgressBar.IsIndeterminate = true;
            //ScopeTitle.Text = _scope.ToString();
            //we can avoid the cross-thread issue by dispatching the entire callback, but don't do too much!
            Mogade.GetLeaderboard(MogadeHelper.LeaderboardId(Leaderboards.Main), _scope, _page, 50, r => Dispatcher.BeginInvoke(() => LeaderboardReceived(r)));
            //could put a loading message here
        }

        private void LeaderboardReceived(Response<LeaderboardScores> response)
        {
            //would hide the loading message here
            if (!response.Success)
            {
                //todo handle the error, maybe display a message to the user
            }
            else
            {
                if (pivot.SelectedIndex == 0)
                {
                    ScoresOverall.Children.Clear();                    

                    for (var i = 0; i < response.Data.Scores.Count; ++i)
                    {
                        var row = new LeaderboardScoreRow(response.Data.Scores[i]);
                        ScoresOverall.Children.Add(row);
                    }
                }
                else if (pivot.SelectedIndex == 1)
                {
                    ScoresWeekly.Children.Clear();
                    
                    for (var i = 0; i < response.Data.Scores.Count; ++i)
                    {
                        var row = new LeaderboardScoreRow(response.Data.Scores[i]);
                        ScoresWeekly.Children.Add(row);
                    }
                }
                else
                {
                    ScoresDaily.Children.Clear();
                    
                    for (var i = 0; i < response.Data.Scores.Count; ++i)
                    {
                        var row = new LeaderboardScoreRow(response.Data.Scores[i]);
                        ScoresDaily.Children.Add(row);
                    }
                }       
            }
            _performanceProgressBar.IsIndeterminate = false;
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _scope = _scopeOrder[pivot.SelectedIndex];
            LoadLeaderboard();
        }
    }
}