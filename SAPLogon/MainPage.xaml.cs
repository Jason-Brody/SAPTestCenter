using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.Automation;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SAPLogon
{
    [ScriptableType()]
    public partial class MainPage : UserControl
    {
        Application app = Application.Current;
        public MainPage()
        {
            InitializeComponent();
            HtmlPage.RegisterScriptableObject("Page", this);
            app.CheckAndDownloadUpdateAsync();
        }

        
        /// <summary>
        /// 0 -- Login Successful
        /// 1 -- UserName and Password is not corrent
        /// 2 -- Other Error
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [ScriptableMember()]
        public int Logon(string userName, string password, string client, string server, string language)
        {


            try
            {
                dynamic cmd = AutomationFactory.CreateObject("WScript.Shell");

                //cmd.Run(mailMsg, 1, false);
                cmd.Run("saplogon.exe", 1, false);

                dynamic sapAutomation = AutomationFactory.CreateObject("SapROTWr.SapROTWrapper");

                dynamic sapgui = null;

                while (sapgui == null)
                {
                    sapgui = sapAutomation.GetROTEntry("SAPGUI");
                    Thread.Sleep(1000);
                }

                dynamic application = sapgui.GetScriptingEngine;


                if (application != null)
                {

                    application.openConnectionByConnectionString(server);
                    int count = application.connections.count - 1;
                    dynamic sapconnection = application.Children(count);
                    dynamic session = sapconnection.Children(0);
                    Type t = session.GetType();
                    session.findById("wnd[0]/usr/txtRSYST-BNAME").text = userName;
                    session.findById("wnd[0]/usr/pwdRSYST-BCODE").text = password;
                    session.findById("wnd[0]/usr/txtRSYST-MANDT").text = client;
                    session.findById("wnd[0]/usr/txtRSYST-LANGU").text = language;
                    session.findById("wnd[0]").sendVKey(0);

                    try
                    {
                        session.findById("wnd[1]/usr/radMULTI_LOGON_OPT2").select();
                        session.findById("wnd[1]/tbar[0]/btn[0]").press();
                    }
                    catch
                    {

                    }

                    try
                    {
                        var status = session.findById("wnd[0]/sbar");
                        if (status.MessageType == "E")
                        {
                            sapconnection.CloseSession(session.Id);
                            return 1;
                        }
                            
                    }
                    catch
                    {
                        return 2;
                    }

                    //try
                    //{
                    //    var obj = session.findById("wnd[0]/usr/txtRSYST-BNAME");
                    //    if (obj != null)
                    //        return 1;
                    //}
                    //catch
                    //{
                    //    return 1;
                    //}



                }
            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 2;
            }
            return 0;
        }







    }
}
