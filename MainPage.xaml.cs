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

namespace PokemonMadjong
{
    public partial class MainPage : PhoneApplicationStylePage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnStart_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Settings.Clear();
            NavigationService.Navigate(new Uri("/Pages/Game.xaml", UriKind.Relative));
        }

        private void btnContinue_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Game.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (Settings.Grid.Count > 0)
            {
                btnContinue.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                btnContinue.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}