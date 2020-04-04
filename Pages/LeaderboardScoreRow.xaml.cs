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
using Mogade;
using System.ComponentModel;

namespace PokemonMadjong
{
    public partial class LeaderboardScoreRow : UserControl//, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        public string UserName { get; set; }
        public string Score { get; set; }

        public SettingsData Settings
        {
            get
            {
                return (Application.Current as PokemonMadjong.App).Settings;
            }
        }

        public LeaderboardScoreRow()
        {
            InitializeComponent();
        }

        public LeaderboardScoreRow(Score score)
        {
            UserName = score.UserName;
            Score = score.Points.ToString();
            DataContext = this;
            InitializeComponent();
        }

        public LeaderboardScoreRow(string columnName, string scoreName)
        {
            UserName = columnName;
            Score = scoreName;            
            DataContext = this;
            InitializeComponent();
            tbScore.FontWeight = FontWeights.Bold;
            tbUserName.FontWeight = FontWeights.Bold;
            tbScore.FontSize = 32;
            tbUserName.FontSize = 32;
        }
    }
}
