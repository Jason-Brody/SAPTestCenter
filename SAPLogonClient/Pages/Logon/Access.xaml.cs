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
    /// Interaction logic for Access.xaml
    /// </summary>
    public partial class Access : UserControl
    {
        private App _app;
        private List<SAPBox> _boxes;
        public Access()
        {
            InitializeComponent();
            _app = App.Current as App;
            
                getBoxes();
            
            
        }

        private async void getBoxes()
        {
            
            setWorking(true);
            try
            {
                var boxes = await _app.Client.BoxInfoAsync(true);
                _boxes = boxes.GroupBy(x => { return new { x.BoxName, x.Id }; }).Select(g => new SAPBox
                {
                    BoxName = g.Key.BoxName,
                    Id = g.Key.Id,
                    Email = string.Join(";", g.Select(x => x.Email))
                }).OrderBy(o => o.BoxName).ToList();
                lv_test.DataContext = _boxes;
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(new Action(() => {
                    MessageBox.Show(ex.Message);
                }));
            }
           
            setWorking(false);
        }

        private void tb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = tb_Search.Text;
            if (string.IsNullOrEmpty(search))
            {
                lv_test.DataContext = _boxes;
            }
            else
            {
                lv_test.DataContext = _boxes.Where(c => c.BoxName.ToLower().Contains(search.ToLower())).ToList();
            }
        }



        private void setWorking(bool isWorking)
        {
            pb.Visibility = isWorking ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            gd_Main.IsEnabled = !isWorking;
            pb.IsActive = isWorking;
        }



        private void btn_mail_Click(object sender, RoutedEventArgs e)
        {
            var sapbox = lv_test.SelectedItem as SAPBox;
            if (sapbox != null)
            {
                _app.SendMail(sapbox.Email, "SAP Logon", "SAPBox:" + sapbox.BoxName);
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            getBoxes();
        }

        private async void btn_Access_Click(object sender, RoutedEventArgs e)
        {
            var sapbox = lv_test.SelectedItem as SAPBox;
            if (sapbox != null)
            {
                try
                {
                    setWorking(true);
                    bool result = await _app.Client.GetAccessAsync(sapbox.Id);
                    if (result == true)
                    {
                        ModernDialog.ShowMessage("An email has been sent to owner,please wait for owners response,or you can send mail to owner directly", "Apply Access for Box:" + sapbox.BoxName, MessageBoxButton.OK);
                    }
                    else
                    {
                        throw new Exception("You have already applied the access");
                    }
                    setWorking(false);
                }
                catch (Exception ex)
                {
                    setWorking(false);
                    ModernDialog.ShowMessage(ex.Message, "Error", MessageBoxButton.OK);
                }
            }
            else
            {
                ModernDialog.ShowMessage("Please select a box", "Warning", MessageBoxButton.OK);
            }
        }
    }
}
