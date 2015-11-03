//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using SAPGuiAutomationLib;
//using System.Collections.ObjectModel;
//using SAPTestRunTime;
//using System.CodeDom;
//using System.Reflection;
//using System.CodeDom.Compiler;
//using System.IO;
//using SAPFEWSELib;

//namespace SAPLogonClient.Pages.Testing
//{
//    /// <summary>
//    /// Interaction logic for TestScript.xaml
//    /// </summary>
//    public partial class TestScript : UserControl
//    {
//        ObservableCollection<RecordStep> steps = new ObservableCollection<RecordStep>();
        
//        public TestScript()
//        {
//            InitializeComponent();
//            dg_Step.DataContext = steps;
//        }

//        private void btn_Record_Click(object sender, RoutedEventArgs e)
//        {
//            if (SAPTestHelper.Current.SAPGuiSession == null)
//                SAPTestHelper.Current.SetSession();

//            SAPAutomationHelper.Current.SetSession(SAPTestHelper.Current.SAPGuiSession);
//            SAPAutomationHelper.Current.StartRecording((r) =>
//            {
//                dg_Step.Dispatcher.BeginInvoke(new Action(() => {
//                    r.ActionName = upperFirstChar(r.ActionName);
//                    r.CompInfo.Id = r.CompInfo.Id.Substring(19);
//                steps.Add(r);
//            })); });

            
//        }

//        private string upperFirstChar(string s)
//        {
//            if (string.IsNullOrEmpty(s))
//            {
//                return string.Empty;
//            }
//            // Return char and concat substring.
//            return char.ToUpper(s[0]) + s.Substring(1);
//        }

//        private void mi_Run_Click(object sender, RoutedEventArgs e)
//        {
//            if (SAPAutomationHelper.Current.SAPGuiApiAssembly == null)
//                SAPAutomationHelper.Current.SetSAPApiAssembly();
//            if(dg_Step.SelectedItems != null)
//            {
//                foreach(RecordStep step in dg_Step.SelectedItems)
//                {
//                    SAPAutomationHelper.Current.RunAction(step);
//                }
//            }
//        }

//        private void btn_Stop_Click(object sender, RoutedEventArgs e)
//        {
//            SAPAutomationHelper.Current.StopRecording();
//        }

//        private void generateCode(string codeType)
//        {
//            CodeCompileUnit targetUnit = new CodeCompileUnit();
//            CodeTypeDeclaration targetClass = new CodeTypeDeclaration("Demo");
//            CodeNamespace ns = new CodeNamespace("SAPTestDemo");
//            ns.Imports.Add(new CodeNamespaceImport("System"));
//            ns.Imports.Add(new CodeNamespaceImport("SAPTestRunTime"));
//            ns.Imports.Add(new CodeNamespaceImport("SAPFEWSELib"));
//            targetClass.IsClass = true;
//            targetClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
//            ns.Types.Add(targetClass);
//            targetUnit.Namespaces.Add(ns);
//            CodeMemberMethod runMethod = new CodeMemberMethod();
//            runMethod.Attributes = MemberAttributes.Static | MemberAttributes.Public;
//            runMethod.Name = "Main";

//            runMethod.Statements.Add(new CodeExpressionStatement(
//                new CodeMethodInvokeExpression(
//                    new CodeVariableReferenceExpression("SAPTestHelper.Current"),
//                    "SetSession", new CodeExpression[0])));
                    

//            foreach (RecordStep step in dg_Step.SelectedItems)
//            {
//                runMethod.Statements.Add(step.GetCodeStatement());
//            }

//            targetClass.Members.Add(runMethod);

//            CodeDomProvider provider = CodeDomProvider.CreateProvider(codeType);
//            CodeGeneratorOptions options = new CodeGeneratorOptions();
//            options.BracingStyle = "C";
//            FileStream fs = new FileStream("Sample.cs", FileMode.Create);
//            using (StreamWriter sourceWriter = new StreamWriter(fs))
//            {
//                provider.GenerateCodeFromCompileUnit(
//                    targetUnit, sourceWriter, options);
//            }
//            fs.Close();
//            fs.Dispose();
//        }

//        private void mi_CSharp_Click(object sender, RoutedEventArgs e)
//        {
//            if(dg_Step.SelectedItems == null)
//            {
//                return;
//            }
//            generateCode("C#");

//            StreamReader sr = new StreamReader("Sample.cs");
//            string code = sr.ReadToEnd();
//            sr.Close();
//            CodeWindow win = new CodeWindow(code);
//            win.ShowDialog();
//        }

//        private void mi_VB_Click(object sender, RoutedEventArgs e)
//        {
//            if (dg_Step.SelectedItems == null)
//            {
//                return;
//            }
//            generateCode("VB");

//            StreamReader sr = new StreamReader("Sample.cs");
//            string code = sr.ReadToEnd();
//            sr.Close();
//            CodeWindow win = new CodeWindow(code);
//            win.ShowDialog();
//        }

//        private void dg_Step_MouseMove(object sender, MouseEventArgs e)
//        {
            
//            if(mySteps != null && mySteps.Count > 0 && e.LeftButton == MouseButtonState.Pressed )
//            {
                
               
//                CodeMemberMethod runMethod = new CodeMemberMethod();
//                runMethod.Attributes = MemberAttributes.Static | MemberAttributes.Public;
//                runMethod.Name = "recordAction";

//                runMethod.Statements.Add(new CodeExpressionStatement(
//                    new CodeMethodInvokeExpression(
//                        new CodeVariableReferenceExpression("SAPTestHelper.Current"),
//                        "SetSession", new CodeExpression[0])));


//                foreach (RecordStep step in mySteps)
//                {
//                    runMethod.Statements.Add(step.GetCodeDetailStatement());
//                }

//                CodeDomProvider provider = CodeDomProvider.CreateProvider("c#");
//                CodeGeneratorOptions options = new CodeGeneratorOptions();
//                options.BracingStyle = "C";
//                StringBuilder sb = new StringBuilder();

               
//                using (TextWriter sourceWriter = new StringWriter(sb))
//                {
//                    provider.GenerateCodeFromMember(runMethod, sourceWriter, options);
                    
//                }
//                DragDrop.DoDragDrop(dg_Step, sb.ToString(), DragDropEffects.Copy);
//            }
//            e.Handled = true;

//        }

//        private List<RecordStep> mySteps;

//        private void dg_Step_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            if(dg_Step.SelectedItems != null)
//            {
//                mySteps = dg_Step.SelectedItems.Cast<RecordStep>().ToList();
//            }
            
//        }

        
        

       

        
//    }
//}
