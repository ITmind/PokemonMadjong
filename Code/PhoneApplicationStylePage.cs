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
using Microsoft.Phone.Controls;
using System.ComponentModel;
using Mogade.WindowsPhone;
using Microsoft.Phone.Marketplace;

namespace PokemonMadjong
{
    public class PhoneApplicationStylePage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public IMogadeClient Mogade 
        { 
            get
            {
                return (Application.Current as PokemonMadjong.App).Mogade;
            }
        }

        public SettingsData Settings
        {
            get
            {
                return (Application.Current as PokemonMadjong.App).Settings;
            }
        }

        public bool IsInTrialMode
        {
            get
            {
#if TRIAL
                return true;
                
#else
                LicenseInformation license = new LicenseInformation();
                return license.IsTrial();
#endif
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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
