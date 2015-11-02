using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SAPLogonClient.AccountService;

namespace SAPLogonClient.Pages.Logon
{
    /// <summary>
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class NewUser : ModernWindow
    {
        private AccountUser _user;
        private App _app;
        private string _icon_Correct = "F1 M 23.7501,33.25L 34.8334,44.3333L 52.2499,22.1668L 56.9999,26.9168L 34.8334,53.8333L 19.0001,38L 23.7501,33.25 Z";
        private string _icon_Wrong = "F1 M 56.0143,57L 45.683,57L 39.0246,44.6245C 38.7758,44.1665 38.5156,43.3183 38.2442,42.0799L 38.1339,42.0799C 38.0095,42.6623 37.7127,43.5473 37.2433,44.7348L 30.5594,57L 20.1857,57L 32.5018,38L 21.2714,19L 31.8487,19L 37.3621,30.3915C 37.7918,31.2963 38.1763,32.365 38.5156,33.5977L 38.6259,33.5977C 38.8408,32.857 39.2394,31.7543 39.8219,30.2897L 45.8951,19L 55.4714,19L 44.0969,37.8388L 56.0143,57 Z";
        public NewUser()
        {
            InitializeComponent();
            _app = App.Current as App;
        }

        public NewUser(AccountUser AccountUser):this()
        {
            _user = AccountUser;
            _user.IsAvailable = false;
        }

        private async void btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            
            setWorking(true);
            _user.User = await _app.Client.UserInfoAsync(tb_Email.Text);
            


            if (_user.User.Id < 1)
            {
                _user.User.Email = tb_Email.Text;
                await _app.GetUserDetail(_user.User);
                if(_user.User.EmployeeId > 0)
                {
                    _user.User.UserName = "";
                    await _app.Client.RegisterUserAsync(_user.User);
                    _user.User = await _app.Client.UserInfoAsync(_user.User.Email);
                    
                }
                    
            }
            if (_user.User.EmployeeId > 0)
            {
                btn_Icon.IconData = StreamGeometry.Parse(_icon_Correct);
                btn_Icon.Foreground = new SolidColorBrush(Colors.Green);
                btn_OK.IsEnabled = true;
            }
            else
            {
                btn_Icon.Foreground = new SolidColorBrush(Colors.Red);
                btn_Icon.IconData = StreamGeometry.Parse(_icon_Wrong);
            }
            btn_Icon.Visibility = System.Windows.Visibility.Visible;
            setWorking(false);
        }

        private void setWorking(bool isWorking)
        {
            pr.IsActive = isWorking;
            gd_Main.IsEnabled = !isWorking;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            _user.IsAvailable = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void tb_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(btn_Icon.Visibility == System.Windows.Visibility.Visible)
            {
                btn_Icon.Visibility = System.Windows.Visibility.Collapsed;
            }
            if(btn_OK.IsEnabled)
            {
                btn_OK.IsEnabled = false;
            }
        }
    }
}
