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
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;
//using SAPLogonClient.TestRemoteService;
using System.ServiceModel;

namespace SAPLogonClient.Pages.Testing
{
    /// <summary>
    /// Interaction logic for CodeWindow.xaml
    /// </summary>
    public partial class CodeWindow : ModernWindow
    {
        public CodeWindow()
        {
            InitializeComponent();
        }

        public CodeWindow(string code):this()
        {
            tb_Code.Text = code;
        }

        private void btn_Run_Click(object sender, RoutedEventArgs e)
        {
            string sourceFile = "Sample.cs";
            
            using (FileStream fs = new FileStream(sourceFile,FileMode.Create))
            {
                using(StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(tb_Code.Text);
                }
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();

            // Build the parameters for source compilation.
            CompilerParameters cp = new CompilerParameters();

            // Add an assembly reference.
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("SAPTestRunTime.dll");
            cp.ReferencedAssemblies.Add("Interop.SAPFEWSELib.dll");
            cp.ReferencedAssemblies.Add("SAPAutoLogon.dll");
            // Generate an executable instead of
            // a class library.
            cp.GenerateExecutable = true;

            // Set the assembly file name to generate.
            cp.OutputAssembly = "demo.dll";

            // Save the assembly as a physical file.
            cp.GenerateInMemory = false;

            // Invoke compilation.
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceFile);
            
            if (cr.Errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                
                // Display compilation errors.
               
                foreach (CompilerError ce in cr.Errors)
                {
                    sb.AppendLine(ce.ToString());
                    
                }
                MessageBox.Show(sb.ToString());
                return;
            }
            else
            {
                
                AppDomain runDomain = AppDomain.CreateDomain("runDomain");
                try
                {
                    runDomain.ExecuteAssembly("demo.dll");
                }
                 catch
                {

                }
                finally
                {
                    AppDomain.Unload(runDomain);
                }
               
            }

            
        }

        private void btn_RemoteRun_Click(object sender, RoutedEventArgs e)
        {
            //string sourceFile = "Sample.cs";

            //using (FileStream fs = new FileStream(sourceFile, FileMode.Create))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs))
            //    {
            //        sw.Write(tb_Code.Text);
            //    }
            //}

            //CSharpCodeProvider provider = new CSharpCodeProvider();

            //// Build the parameters for source compilation.
            //CompilerParameters cp = new CompilerParameters();

            //// Add an assembly reference.
            //cp.ReferencedAssemblies.Add("System.dll");
            //cp.ReferencedAssemblies.Add("SAPTestRunTime.dll");
            //cp.ReferencedAssemblies.Add("Interop.SAPFEWSELib.dll");
            //cp.ReferencedAssemblies.Add("SAPAutoLogon.dll");
            //// Generate an executable instead of
            //// a class library.
            //cp.GenerateExecutable = true;

            //// Set the assembly file name to generate.
            //cp.OutputAssembly = "demo.dll";

            //// Save the assembly as a physical file.
            //cp.GenerateInMemory = false;

            //// Invoke compilation.
            //CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceFile);

            //if (cr.Errors.Count > 0)
            //{
            //    StringBuilder sb = new StringBuilder();

            //    // Display compilation errors.

            //    foreach (CompilerError ce in cr.Errors)
            //    {
            //        sb.AppendLine(ce.ToString());

            //    }
            //    MessageBox.Show(sb.ToString());
            //    return;
            //}
            //else
            //{

            //    string endpointAddress = "http://" + tb_Remote.Text + ":9001/RemoteTestService";
            //    RemoteExecutionServiceClient client = new RemoteExecutionServiceClient(new BasicHttpBinding(), new EndpointAddress(endpointAddress));
            //    TestRemote.File f = new TestRemote.File() { Name = "demo.dll" };
            //    f.Content = File.ReadAllBytes("demo.dll");
            //    client.SendFile(f);
            //    client.Run(f);
                
            //}
           
            
        }
    }
}
