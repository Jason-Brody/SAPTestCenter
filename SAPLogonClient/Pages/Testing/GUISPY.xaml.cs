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
using SAPFEWSELib;
using SAPGuiAutomationLib;
using System.Timers;
using FirstFloor.ModernUI.Windows.Controls;
using System.Reflection;
using SAPLogonClient.ViewModel;
using System.Windows.Media.Animation;
using SAPTestRunTime;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace SAPLogonClient.Pages.Testing
{
    /// <summary>
    /// Interaction logic for GUISPY.xaml
    /// </summary>
    public partial class GUISPY : UserControl
    {

        public GUISPY()
        {
            InitializeComponent();
        }

        private async void btn_ShowData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    SAPTestHelper.Current.SetSession();
                    SAPAutomationHelper.Current.SetSession(SAPTestHelper.Current.SAPGuiSession);
                });
                
                //if(SAPTestHelper.Current.SAPGuiSession == null)
                    
            }
            catch (Exception ex)
            {
                ModernDialog.ShowMessage(ex.Message, "Error", MessageBoxButton.OK);
                return;
            }

            if (SAPAutomationHelper.Current.SAPGuiApiAssembly == null)
                SAPAutomationHelper.Current.SetSAPApiAssembly();


            SAPAutomationHelper.Current.SetVisualMode(true);
            SAPAutomationHelper.Current.Spy((c) =>
            {
                displayParent(c);
                displayData(c);
            });

           

        }

        private string displayCode(CodeExpression Expression)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("c#");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "c";
            StringBuilder sb = new StringBuilder();

            using (TextWriter sourceWriter = new StringWriter(sb))
            {
                provider.GenerateCodeFromExpression(Expression, sourceWriter, options);

            }
            return sb.ToString();
        }

        private void displayParent(WrapComp c)
        {
            
            Stack<SapCompInfo> comps = new Stack<SapCompInfo>();
            do
            {
                SapCompInfo ci = new SapCompInfo();
                ci.Id = c.Comp.Id;
                ci.Name = c.Comp.Name;
                ci.Type = c.Comp.Type;
                comps.Push(ci);
                c.Comp = c.Comp.Parent;
            }
            while ((c.Comp is GuiConnection) == false);

            tv_Elements.Dispatcher.BeginInvoke(new Action(() => {
                tv_Elements.Items.Clear();
                TreeViewItem RootItem = new TreeViewItem();
                RootItem.IsExpanded = true;
                RootItem.Header = comps.Pop();
                tv_Elements.Items.Add(RootItem);
                
                while (comps.Count > 0)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = comps.Pop();
                    item.IsExpanded = true;
                    RootItem.Items.Add(item);
                    RootItem = item;
                }



                RootItem.IsSelected = true;
            }));
            
            
        }

        private void displayData(WrapComp c)
        {
            Type detailType = null; GuiShell shellObj = null;
            if (c.Comp.Type.ToLower().Contains("shell"))
            {
                shellObj = c.Comp as GuiShell;
                if (shellObj != null)
                {
                    foreach (Type t in SAPAutomationHelper.Current.SAPGuiApiAssembly.GetTypes().Where(tp => tp.IsInterface))
                    {
                        if (t.Name.Contains("Gui" + shellObj.SubType))
                        {
                            detailType = t;
                            break;
                        }
                    }
                }
            }

            string typeName = detailType == null ? c.Comp.Type : detailType.Name;

            var props = SAPAutomationHelper.Current.GetSAPTypeInfoes<PropertyInfo>(typeName, t => t.GetProperties().Where(p => p.IsSpecialName == false));
            {
                List<SAPElementProperty> pps = new List<SAPElementProperty>();
                foreach (var p in props)
                {
                    SAPElementProperty prop = new SAPElementProperty();
                    prop.Name = p.Name;
                    try
                    {
                        prop.Value = p.GetValue(c.Comp).ToString();
                    }
                    catch
                    {
                        prop.Value = "";
                    }
                    pps.Add(prop);
                }
                List<string> mds = getMethods(typeName);
                lv_Props.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SAPAutomationHelper.Current.SetVisualMode(false);
                    lv_Props.DataContext = pps;
                    lv_Methods.DataContext = mds;
                }));

            }
        }


        private void mi_Name_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (lv_Props.SelectedValue != null)
            {
                SAPElementProperty prop = lv_Props.SelectedValue as SAPElementProperty;
                Clipboard.SetData(DataFormats.Text, prop.Name);
            }
        }

        private void mi_Value_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (lv_Props.SelectedValue != null)
            {
                SAPElementProperty prop = lv_Props.SelectedValue as SAPElementProperty;
                Clipboard.SetData(DataFormats.Text, prop.Value);
            }
        }

        private void rd_Method_Checked(object sender, RoutedEventArgs e)
        {
            startAnimation();
        }

        private void rd_Prop_Checked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = 0;
            animation.Duration = TimeSpan.FromSeconds(0.5);
            gd_Content.BeginAnimation(Canvas.LeftProperty, animation);
        }

        private void startAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = -gd_Info.ActualWidth;
            animation.Duration = TimeSpan.FromSeconds(0.5);
            gd_Content.BeginAnimation(Canvas.LeftProperty, animation);
        }

        private List<string> getMethods(string typeName)
        {
            List<string> methods = new List<string>();
            var ms = SAPAutomationHelper.Current.GetSAPTypeInfoes<MethodInfo>(typeName, t => t.GetMethods().Where(m => m.IsSpecialName == false));
            foreach (var m in ms)
            {
                string method = string.Empty;
                method += m.ReturnType.Name + " " + m.Name;

                ParameterInfo[] paInfoes = m.GetParameters();
                if (paInfoes.Count() > 0)
                {
                    method += "(";
                    foreach (var p in paInfoes)
                    {
                        if (p.IsOptional)
                        {
                            method += "[Optional]";
                        }
                        method += p.ParameterType.Name + " " + p.Name + ",";
                    }
                    method = method.Substring(0, method.Length - 1);
                    method += ")";
                }
                else
                {
                    method += "()";
                }

                methods.Add(method);

                
            }
            return methods;
        }

        private void tv_Elements_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(tv_Elements.SelectedItem != null)
            {
                SapCompInfo ci = (tv_Elements.SelectedItem as TreeViewItem).Header as SapCompInfo;
                if (ci != null)
                {
                    try
                    {
                        WrapComp wc = new WrapComp() { Comp = SAPTestHelper.Current.GetElementById(ci.Id) };
                        displayData(wc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            
        }

        private void tv_Elements_MouseMove(object sender, MouseEventArgs e)
        {
            SapCompInfo ci = (tv_Elements.SelectedItem as TreeViewItem).Header as SapCompInfo;
            
            if (ci != null && e.LeftButton == MouseButtonState.Pressed)
            {
                //DragDrop.DoDragDrop(tv_Elements.SelectedItem as TreeViewItem, displayCode(ci.GetFindCode()), DragDropEffects.Copy);
            }
        }

        

       
    }
}
