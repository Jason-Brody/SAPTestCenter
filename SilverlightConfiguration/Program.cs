using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SilverlightConfiguration
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Import Certificate");
                X509Certificate2 cert = null;
                string file = AppDomain.CurrentDomain.BaseDirectory + "SAPLogon_TemporaryKey.pfx";
                cert = new X509Certificate2(file, "zhouyang78607");
                X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);

                store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);

               
            }
            
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            

            try
            {
                Console.WriteLine("Register Key");
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Silverlight",true);
                key.SetValue("AllowElevatedTrustAppsInBrowser", 1, RegistryValueKind.DWord);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Please try as System Admin");
                Console.ReadLine();
            }
        }

        static bool RegisterUser()
        {
            var user = System.DirectoryServices.AccountManagement.UserPrincipal.Current;
            User u = new User();
            u.Email = user.EmailAddress;
            u.NTAccount = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            u.UserName = user.DisplayName;
            u.EmployeeId = int.Parse(user.EmployeeId);
            ActServiceClient client = new ActServiceClient("http://localhost:17491/");

            var getTask = client.GetItemAsync<string>("api/Account/Test");
            getTask.Wait();
            string res = getTask.Result;


            Task<bool> postTask = client.PostJsonItemAsync<User>("api/Account/RegisterUser", u);
            postTask.Wait();
            bool result = postTask.Result;
            return result;
        }
    }
}
