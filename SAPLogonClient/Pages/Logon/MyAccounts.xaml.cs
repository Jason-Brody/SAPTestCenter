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
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using FirstFloor.ModernUI.Windows.Controls;
using SAPLogonClient.AccountService;
using SAPLogonClient.ViewModel;

namespace SAPLogonClient.Pages.Logon
{
    /// <summary>
    /// Interaction logic for MyAccounts.xaml
    /// </summary>
    public partial class MyAccounts : UserControl
    {
        private App _app;

        private bool _isEdit = false;
        private string _endEdit = "F1 M 53.2929,21.2929L 54.7071,22.7071C 56.4645,24.4645 56.4645,27.3137 54.7071,29.0711L 52.2323,31.5459L 44.4541,23.7677L 46.9289,21.2929C 48.6863,19.5355 51.5355,19.5355 53.2929,21.2929 Z M 31.7262,52.052L 23.948,44.2738L 43.0399,25.182L 50.818,32.9601L 31.7262,52.052 Z M 23.2409,47.1023L 28.8977,52.7591L 21.0463,54.9537L 23.2409,47.1023 Z M 17,28L 17,23L 34,23L 34,28L 17,28 Z ";
        private string _edit = "F1 M 53.2929,21.2929L 54.7071,22.7071C 56.4645,24.4645 56.4645,27.3137 54.7071,29.0711L 52.2323,31.5459L 44.4541,23.7677L 46.9289,21.2929C 48.6863,19.5355 51.5355,19.5355 53.2929,21.2929 Z M 31.7262,52.052L 23.948,44.2738L 43.0399,25.182L 50.818,32.9601L 31.7262,52.052 Z M 23.2409,47.1023L 28.8977,52.7591L 21.0463,54.9537L 23.2409,47.1023 Z ";
        private ObservableCollection<AccountViewModel> _accountViewModels;
        private MyAccount _myAccount;
        public MyAccounts()
        {
            InitializeComponent();
            _app = App.Current as App;
            getAccounts();
        }

        private async void getAccounts()
        {
            setWorking(true);
            var acts = await _app.Client.MyAccountsAsync();
            
            var accounts = acts.OrderBy(o => o.BoxId).ToList();
            _accountViewModels = new ObservableCollection<AccountViewModel>();
            accounts.ForEach((a) => { AccountViewModel avm = new AccountViewModel(); MyAccount ma = new MyAccount(a); avm.Account = ma; _accountViewModels.Add(avm); });
            _accountViewModels.Add(new AccountViewModel() { Account = new MyAccount(), IsNew = true });
            lv_Accounts.DataContext = _accountViewModels;
            setWorking(false);
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            beginAnimation(_isEdit);
            setCommonStatus();
            tb_Box.IsEnabled = true;

            if(_isEdit)
            {
                _myAccount = new MyAccount();
                _myAccount.IsAvailable = true;
                _myAccount.AcctUsers = new ObservableCollection<AccountUser>();
                gd_Account.DataContext = _myAccount;
                dg_User.DataContext = _myAccount.AcctUsers;
            }
            

        }

        private async void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            beginAnimation(_isEdit);
            tb_Box.IsEnabled = false;
            setCommonStatus();

            gd_Account.DataContext = _myAccount;

            if(_isEdit)
            {
                setWorking(true);
                var aus = await _app.Client.GetAccountUsersAsync(_myAccount.Id);
                _myAccount.AcctUsers = new ObservableCollection<AccountUser>(aus.ToList());
                dg_User.DataContext = _myAccount.AcctUsers;
                setWorking(false);
            }
        }



        private void setWorking(bool isWorking)
        {
            pr.IsActive = isWorking;
            gd_Main.IsEnabled = !isWorking;
        }

        private void setCommonStatus()
        {
            _isEdit = !_isEdit;
            btn_Edit.IsEnabled = _isEdit;
            btn_Refresh.IsEnabled = !_isEdit;
            btn_Update.IsEnabled = _isEdit;
            lv_Accounts.SelectedIndex = -1;
            btn_Edit.IconData = StreamGeometry.Parse(_isEdit ? _endEdit : _edit);

        }

        private void beginAnimation(bool show)
        {
            DoubleAnimation movement = new DoubleAnimation();
            movement.Duration = TimeSpan.FromSeconds(0.5);
            if (show)
            {
                movement.To = 0;
            }
            else
            {
                movement.To = -gd_Info.ActualWidth;
            }
            gd_Content.BeginAnimation(Canvas.LeftProperty, movement);
        }

        private async void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                setWorking(true);
                validate();
                if(_myAccount.Id < 1)
                {
                    bool isBoxExisted = await _app.Client.IsBoxExistedAsync(_myAccount.BoxId);
                    
                    if (isBoxExisted)
                        throw new Exception(string.Format("The box named '{0}' is already existed,please choose another name",_myAccount.BoxId));
                }
                XmlSerializer xs = new XmlSerializer(typeof(MyAccount));
                StringBuilder sb = new StringBuilder();
                XmlWriter xw = XmlWriter.Create(sb);
                xs.Serialize(xw, _myAccount);
                xw.Close();
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(sb.ToString());
                if (xdoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
                    xdoc.RemoveChild(xdoc.FirstChild);
                }
                bool result = await _app.Client.UpdateAccountAsync(xdoc.InnerXml);
                
                if(!result)
                {
                    throw new Exception("Fail to Update");
                }
                setWorking(false);
                ModernDialog.ShowMessage("Update Successful", "Success", MessageBoxButton.OK);
            }
            catch(Exception ex)
            {
                setWorking(false);
                ModernDialog.ShowMessage(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void validate()
        {
            string msg = " can not be empth";
            if(_myAccount != null)
            {
                if(string.IsNullOrEmpty(_myAccount.BoxId))
                {
                    throw new Exception("Boxname"+msg);
                }
                if(string.IsNullOrEmpty(_myAccount.UserName))
                {
                    throw new Exception("Username"+msg);
                }
                if(string.IsNullOrEmpty(_myAccount.Password))
                {
                    throw new Exception("Password" + msg);
                }
                if(string.IsNullOrEmpty(_myAccount.Client))
                {
                    throw new Exception("Client" + msg);
                }
                if(string.IsNullOrEmpty(_myAccount.Server))
                {
                    throw new Exception("Server" + msg);
                }
            }
            else
            {
                throw new Exception("The Account is null");
            }
        }

        private void lv_Accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var avm = lv_Accounts.SelectedItem as AccountViewModel;
            if (avm != null)
            {
                _myAccount = avm.Account;
                btn_Edit.IsEnabled = true;
            }

        }

        private void btn_NewUser_Click(object sender, RoutedEventArgs e)
        {
            AccountUser au = new AccountUser();
            au.User = new User();
            NewUser nu = new NewUser(au);
            nu.ShowDialog();
            if (au.IsAvailable)
            {
                if (_myAccount.AcctUsers.Where(c => c.User.Email.ToLower() == au.User.Email.ToLower()).FirstOrDefault() == null)
                {
                    au.Uid = au.User.Id;
                    
                    _myAccount.AcctUsers.Add(au);
                }
            }
        }

        private void btn_RemoveUser_Click(object sender, RoutedEventArgs e)
        {
            var au = dg_User.SelectedItem as AccountUser;
            if (au != null)
            {
                _myAccount.AcctUsers.Remove(au);
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            getAccounts();
        }
    }
}
