//using AutoUpgrade;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using System.Xml.Serialization;

namespace SAPLogonClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            
            this.Closed += MainWindow_Closed;
            //Upgrade.Current.StartUpgrade(false);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            ThemeData data = new ThemeData();
            try
            {
                data.A = AppearanceManager.Current.AccentColor.A;
                data.R = AppearanceManager.Current.AccentColor.R;
                data.G = AppearanceManager.Current.AccentColor.G;
                data.B = AppearanceManager.Current.AccentColor.B;
                data.ThemeURI = AppearanceManager.Current.ThemeSource.OriginalString;

                XmlSerializer xs = new XmlSerializer(typeof(ThemeData));
                xs.Serialize(new System.IO.FileStream((App.Current as App).ThemeConfig, System.IO.FileMode.Create),data);
                
            }
            catch
            {

            }
            
        }


    }
}
