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
using SAPGuiAutomationLib;
using FirstFloor.ModernUI.Windows.Controls;
using SAPFEWSELib;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
//using Young.Automation.Selenium.TestAssistant;
using SAPLogonClient.ViewModel;
//using SAPTestRunTime;

namespace SAPLogonClient.Pages.Logon
{
    /// <summary>
    /// Interaction logic for Logon.xaml
    /// </summary>
    public partial class Logon : UserControl
    {
        private App _app;
        private List<SAPBox> _boxes;
        public Logon()
        {
            InitializeComponent();
            _app = App.Current as App;
            
            getBoxes();
        }

        private async void getBoxes()
        {
            try
            {
                setWorking(true);
                var boxes = await _app.Client.BoxInfoAsync(false);
                
                _boxes = boxes.GroupBy(x => { return new { x.BoxName, x.Id }; }).Select(g => new SAPBox
                {
                    BoxName = g.Key.BoxName,
                    Id = g.Key.Id,
                    Email = string.Join(";", g.Select(x => x.Email))
                }).OrderBy(o => o.BoxName).ToList();

                lv_test.DataContext = _boxes;
                setWorking(false);
            }
            catch(Exception ex)
            {
                setWorking(false);
                ModernDialog.ShowMessage(ex.Message,"Error",MessageBoxButton.OK);
            }
        }

        private void tb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = tb_Search.Text;
            if(string.IsNullOrEmpty(search))
            {
                lv_test.DataContext = _boxes;
            }
            else
            {
                lv_test.DataContext = _boxes.Where(c => c.BoxName.ToLower().Contains(search.ToLower())).ToList();
            }
        }

        private async void btn_Logon_Click(object sender, RoutedEventArgs e)
        {
            var sapBox = lv_test.SelectedItem as SAPBox;
            if(sapBox!=null)
            {
                setWorking(true);
                var account = await _app.Client.AccountInfoAsync(sapBox.Id, Environment.MachineName);
                try
                {
                    account.Id = sapBox.Id;
                    if(account.IsWebLogin)
                    {
                        BrowserWindow b = new BrowserWindow();
                        b.ShowDialog();
                        await webLogonTask(account, BrowserWindow.Browser);
                    }
                    else
                    {
                        await logonTask(account);
                    }
                    
                    setWorking(false);
                }
                catch(Exception ex)
                {
                    setWorking(false);
                    ModernDialog.ShowMessage(ex.Message, "Error", MessageBoxButton.OK);
                }
                
            }
            
        }

        private void setWorking(bool isWorking)
        {
            pb.Visibility = isWorking ? System.Windows.Visibility.Visible:System.Windows.Visibility.Collapsed;
            gd_Main.IsEnabled = !isWorking;
            pb.IsActive = isWorking;
        }

        private Task logonTask(Account acct)
        {
            return Task.Run(() => {
                
                //SAPLogon logon = new SAPLogon();
                //logon.AfterLogin += (s, e) => {
                        
                //        GuiPasswordField gpf = SAPTestHelper.Current.TryGetElementById<GuiPasswordField>("wnd[1]/usr/pwdRSYST-NCODE");
                //        GuiTextField txtF = SAPTestHelper.Current.TryGetElementById<GuiTextField>("wnd[0]/usr/pwdRSYST-BCODE");
                //        if (gpf != null || txtF !=null)
                //        {
                //            GuiConnection gc = s.Parent as GuiConnection;
                //            gc.CloseSession(s.Id);
                //            _app.Client.FailLoginAsync(acct);
                //            throw new Exception("Fail to Login, an email has will send to owner shortly");
                //        }
                //        else
                //        {
                //            _app.Client.AddLog(acct.Id, Environment.MachineName, true);
                //        }
                    
                    
                //};
                //logon.StartProcess();
                //logon.OpenConnection(acct.Server);
                //SAPTestHelper.Current.SetSession(logon);
                //logon.Login(acct.UserName, acct.Password, acct.Client,"EN");
            });
            
        }

        private Task webLogonTask(Account acct,Browser b)
        {
            return Task.Run(() => {
                //IWebDriver driver = null;
                //if(b == Browser.Chrome)
                //{
                //    driver = new ChromeDriver();
                //}
                //else
                //{
                //    driver = new InternetExplorerDriver();
                //}

                
                //IOptions io = driver.Manage();
                //io.Window.Maximize();
                //string url = acct.Server;
                //driver.Navigate().GoToUrl(url);
                //IWebElement element = null;

               

                //element = driver.GetElementLazy(By.Id("sap-user"), 20);
                //element.SendNewKeys(acct.UserName);
                //element = driver.GetElementLazy(By.Id("sap-client"), 20);
                //element.SendNewKeys(acct.Client);
                
                //element = driver.GetElementLazy(By.Id("sap-password"), 20);
                //element.SendNewKeys(acct.Password);
                //element = driver.GetElementLazy(By.Id("LOGON_BUTTON"), 20);
                //element.Submit();

                //try
                //{
                //    element = driver.GetElementLazy(By.Id("sap-user"), 2);
                //}
                //catch
                //{
                //    element = null;
                //}
                
                //if(element!=null)
                //{
                //    _app.Client.FailLoginAsync(acct);

                //    //System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName("chromedriver");
                //    //if(ps!=null)
                //    //{
                //    //    foreach(var p in ps)
                //    //    {
                //    //        p.Kill();
                //    //    }
                //    //}

                //    throw new Exception("Fail to Login, an email has will send to owner shortly");
                //}
                
            });
        }

        private void btn_mail_Click(object sender, RoutedEventArgs e)
        {
            var sapbox = lv_test.SelectedItem as SAPBox;
            if(sapbox != null)
            {
                _app.SendMail(sapbox.Email, "SAP Logon", "SAPBox:"+sapbox.BoxName);
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            getBoxes();
        }

        
    }
}