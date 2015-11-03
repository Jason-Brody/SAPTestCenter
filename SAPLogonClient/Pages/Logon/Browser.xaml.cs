using FirstFloor.ModernUI.Windows.Controls;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SAPLogonClient.AccountService;
using SAPLogonClient.ViewModel;
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
//using Young.Automation.Selenium.TestAssistant;

namespace SAPLogonClient.Pages.Logon
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class BrowserWindow : ModernWindow
    {
        public static Browser Browser { get; set; }
        public BrowserWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Browser = (Browser)cb_Browser.SelectedValue;
            this.Close();
        }

        

        
    }
}
