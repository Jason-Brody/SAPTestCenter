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
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Net;
using System.IO;
using System.Windows.Media.Animation;
using SAPLogonClient.Pages.Settings;
using FirstFloor.ModernUI.Presentation;
using SAPLogonClient.AccountService;
using System.Xml;
using System.Xml.Serialization;

namespace SAPLogonClient
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : ModernWindow
    {
        private App _app;

        private List<string> _Log = new List<string>();
        private string _picPath = Environment.CurrentDirectory + @"\Resources\self.png";
        public Start()
        {
            _app = App.Current as App;
            InitializeComponent();

            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ThemeData));
                ThemeData data =  xs.Deserialize(new FileStream((App.Current as App).ThemeConfig, System.IO.FileMode.OpenOrCreate)) as ThemeData;
                AppearanceManager.Current.ThemeSource = new Uri(data.ThemeURI, UriKind.Relative);
                AppearanceManager.Current.AccentColor = Color.FromArgb(data.A, data.R,data.G,data.B);
                
            }
            catch
            {
                AppearanceManager.Current.ThemeSource = new Uri("/SAPLogonClient;component/Theme/bing.xaml", UriKind.Relative);
                AppearanceManager.Current.AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);
            }


            
            getUser();
            GetBingImage();
        }

        public void GetBingImage()
        {
            
            try
            {
                WebClient web = new WebClient();
                web.Encoding = System.Text.Encoding.UTF8;
                web.Proxy = new WebProxy("web-proxy.sgp.hp.com", 8080);
                var data = web.DownloadData("http://www.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=1");
                XmlDocument xDoc = new XmlDocument();
                var dataString = System.Text.Encoding.Default.GetString(data);
                xDoc.LoadXml(dataString);
                XmlNode node = xDoc.SelectSingleNode("images/image/url");

                var uri = "http://www.bing.com/" + node.InnerText;

                App.BingImageUrl = new BitmapImage(new Uri(uri));
            }
            catch
            {

            }

        }


        private async void getUser()
        {
            try
            {

                _Log.Add("Getting User From Server");
                
                var user = await _app.Client.UserInfoAsync("");
                

                _Log.Add("Get Response from server");
                if (user == null || user.EmployeeId < 1 || string.IsNullOrEmpty(user.UserName))
                {

                    _app.SAPUser = new User();
                    _app.SAPUser.NTAccount = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    _Log.Add("NTAccount:" + _app.SAPUser.NTAccount);
                    _Log.Add("Getting User from HP Server");
                    await _app.GetUserDetail(_app.SAPUser, false);
                    _Log.Add("User Mail:" + _app.SAPUser.Email);
                    if (string.IsNullOrEmpty(_app.SAPUser.PhotoUrl) == false)
                    {
                        await SaveImageFromUrl(_app.SAPUser.PhotoUrl);
                        _Log.Add("Save Image");
                    }

                    tbl_Process.Text = "Registing User Data";
                    _Log.Add("Register User Info");
                    await _app.Client.RegisterUserAsync(_app.SAPUser);
                    //bool result = await _app.Client.RegisterUserAsync(_app.SAPUser);
                    //if(!result)
                    //{
                    //    throw new Exception("Fail when registering the user data");
                    //}
                }
                else
                {
                    _Log.Add("Success to get user from server");
                    _app.SAPUser = user;
                }
                img_Self.Source = new BitmapImage(new Uri("Resources/self.png", UriKind.Relative));
                string file = System.IO.Path.Combine(Environment.CurrentDirectory, "Log.xml");
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    xs.Serialize(fs, _Log);
                }
                beginAnimation();

            }
            catch (System.ServiceModel.Security.SecurityNegotiationException ex)
            {
                if (ModernDialog.ShowMessage(ex.Message + "\rPlease contact developer Zhou Yang for help.\rCantact now?", "Error", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    _app.SendMail("yang.zhou4@hp.com", "[SAP Logon tool]#Error", ex.Message);
                }
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                string file = System.IO.Path.Combine(Environment.CurrentDirectory, "Log.xml");
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    xs.Serialize(fs, _Log);
                }
                if (ModernDialog.ShowMessage(ex.Message + "\rPlease contact developer Zhou Yang for help.\rCantact now?", "Error", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    _app.SendMail("yang.zhou4@hp.com", "[SAP Logon tool]#Error", ex.Message);
                }
                Application.Current.Shutdown();
            }
            
        }

        private Task GetUserInfo(User user)
        {
            return Task.Run(() =>
            {
                var u = UserPrincipal.Current;
                user.Email = u.EmailAddress;
                user.EmployeeId = int.Parse(u.EmployeeId);
                user.NTAccount = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                user.UserName = u.DisplayName;
            });
        }



        private Task SaveImageFromUrl(string url)
        {
            return Task.Run(() =>
            {
                using (WebClient Client = new WebClient())
                {
                    Client.DownloadFile(url, _picPath);
                }
            });
        }

        private void beginAnimation()
        {
            DoubleAnimation hide_Process = new DoubleAnimation();
            hide_Process.To = cv_Main.ActualWidth;
            hide_Process.Duration = TimeSpan.FromSeconds(1);
            hide_Process.Completed += (s, e) =>
            {
                sp_Process.Visibility = System.Windows.Visibility.Collapsed;
                gd_Info.DataContext = _app.SAPUser;
                gd_Pic.Visibility = System.Windows.Visibility.Visible;
                DoubleAnimation height_Pic = new DoubleAnimation();
                height_Pic.From = gd_Pic.ActualHeight + 200;
                height_Pic.Duration = TimeSpan.FromSeconds(1);
                height_Pic.Completed += (s1, e1) =>
                {
                    gd_Info.Visibility = System.Windows.Visibility.Visible;
                    DoubleAnimation left_Info = new DoubleAnimation();
                    left_Info.From = 0;
                    left_Info.Duration = TimeSpan.FromSeconds(1);
                    sp_Info.BeginAnimation(Canvas.RightProperty, left_Info);
                };
                img_Self.BeginAnimation(Canvas.BottomProperty, height_Pic);
            };
            sp_Process.BeginAnimation(Canvas.RightProperty, hide_Process);







        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void btn_Web_Click(object sender, RoutedEventArgs e)
        {

            ModernWindow mw = new ModernWindow();

            WebBrowser wb = new WebBrowser();
            mw.Content = wb;

            wb.Navigate("http://mp.weixin.qq.com/s?__biz=MzAxMzI4MTUxMw==&mid=204472346&idx=1&sn=97445b23d97c280fa920134d2348a298&scene=2#rd");
            mw.Show();
        }




    }
}
