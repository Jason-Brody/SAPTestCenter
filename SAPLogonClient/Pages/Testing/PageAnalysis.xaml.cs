using SAPFEWSELib;
using SAPGuiAutomationLib;
using SAPLogonClient.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Xml;
using SAPTestRunTime;
using System.CodeDom.Compiler;
using System.IO;

namespace SAPLogonClient.Pages.Testing
{
    /// <summary>
    /// Interaction logic for SAPPageAnalysis.xaml
    /// </summary>
    public partial class PageAnalysis : UserControl
    {
        private int _maxCount;
        private GuiVComponent _lastHighlight;
        XmlDocument xDoc;
        public PageAnalysis()
        {
            InitializeComponent();
        }

        private async void btn_GetElements_Click(object sender, RoutedEventArgs e)
        {
            _lastHighlight = null;
            xDoc = new XmlDocument();
            try
            {
                if (SAPAutomationHelper.Current.SAPGuiApiAssembly == null)
                    SAPAutomationHelper.Current.SetSAPApiAssembly();
                XmlElement root = xDoc.CreateElement("Node");
                root.SetAttribute("name", "root");
                Task loopNodeTask = new Task(() =>
                {
                    SAPTestHelper.Current.SetSession();
                    SAPAutomationHelper.Current.SetSession(SAPTestHelper.Current.SAPGuiSession);
                    WrapComp comp = new WrapComp() { Comp = SAPTestHelper.Current.SAPGuiSession as GuiComponent };

                    _maxCount = 20;


                    SAPAutomationHelper.Current.LoopSAPComponents<XmlElement>(comp, root, 0, _maxCount, addNode);

                    xDoc.AppendChild(root.FirstChild);
                });
                setWorking(true);
                loopNodeTask.Start();
                await loopNodeTask;
                await Task.Run(() =>
                {
                    var props = SAPAutomationHelper.Current.GetSAPTypeInfoes<PropertyInfo>("GuiSessionInfo", t => t.GetProperties().Where(p => p.IsSpecialName == false));
                    {
                        List<SAPElementProperty> pps = new List<SAPElementProperty>();
                        foreach (var p in props)
                        {
                            SAPElementProperty prop = new SAPElementProperty();
                            prop.Name = p.Name;
                            try
                            {
                                prop.Value = p.GetValue(SAPTestHelper.Current.SAPGuiSession.Info).ToString();
                            }
                            catch
                            {
                                prop.Value = "";
                            }
                            pps.Add(prop);
                        }
                        lv_PageInfo.Dispatcher.BeginInvoke(new Action(() => { lv_PageInfo.DataContext = pps; }));
                    }
                });
                tv_Elements.DataContext = xDoc;

                setWorking(false);
            }
            catch (Exception ex)
            {
                setWorking(false);
                MessageBox.Show(ex.Message);
            }
        }

        private XmlElement addNode(WrapComp comp, XmlElement item, int count)
        {
            XmlElement newItem = xDoc.CreateElement("Node");
            newItem.SetAttribute("name", comp.Comp.Name);
            newItem.SetAttribute("id", comp.Comp.Id);
            newItem.SetAttribute("type", comp.Comp.Type);
            newItem.SetAttribute("num", count.ToString());
            if (comp.Comp is GuiVComponent)
            {
                try
                {
                    newItem.SetAttribute("text", ((GuiVComponent)comp.Comp).Tooltip);
                }
                catch { }
            }
            item.AppendChild(newItem);
            return newItem;
        }

        private void tv_Elements_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                
                XmlElement element = tv_Elements.SelectedItem as XmlElement;
                            
                

                GuiVComponent comp = SAPTestHelper.Current.GetElementById<GuiVComponent>(element.GetAttribute("id"));
                if(_lastHighlight != null)
                {
                    _lastHighlight.Visualize(false);
                    
                }
                _lastHighlight = comp;
                _lastHighlight.Visualize(true);
            }
            catch { }
        }

        private void setWorking(bool isWorking)
        {
            btn_GetElements.IsEnabled = !isWorking;
            pb_Working.IsIndeterminate = isWorking;

        }

        private void btn_test_Click(object sender, RoutedEventArgs e)
        {
            XmlElement root = (sender as Grid).DataContext as XmlElement;
            XmlElement root1 = tv_Elements.SelectedItem as XmlElement;
            
            string id = root.GetAttribute("id");
            GuiComponent cp = SAPTestHelper.Current.GetElementById(id);
            WrapComp comp = new WrapComp() { Comp = cp };
            SAPAutomationHelper.Current.LoopSAPComponents<XmlElement>(comp, root, 0, _maxCount, addNode);
            //var list = root.LastChild.ChildNodes;
            //root.RemoveChild(root.LastChild);
            //foreach(XmlNode child in list)
            //{
            //    root.AppendChild(child);
            //}
            //XmlElement root = tv_Elements.SelectedItem as XmlElement;
            //string id = root.GetAttribute("id");
            //GuiComponent cp = SAPTestHelper.Current.GetElementById(id);
            //WrapComp comp = new WrapComp() { Comp = cp };
            //SAPTestHelper.Current.LoopSAPComponents<XmlElement>(comp, root, 50, _maxCount, addNode);
        }


        private void mi_Load_Click(object sender, RoutedEventArgs e)
        {
            object obj = sender;
            if (tv_Elements.SelectedItem != null)
            {
                XmlElement root = tv_Elements.SelectedItem as XmlElement;
                XmlElement rootCopy = root.Clone() as XmlElement;
                string id = root.GetAttribute("id");
                GuiComponent cp = SAPTestHelper.Current.GetElementById(id);
                WrapComp comp = new WrapComp() { Comp = cp };
                SAPAutomationHelper.Current.LoopSAPComponents<XmlElement>(comp, root, root.ChildNodes.Count, _maxCount, addNode);
                root = tv_Elements.SelectedItem as XmlElement;
                var lastNode = root.LastChild.Clone();
                root.RemoveChild(root.LastChild);
                int count = lastNode.ChildNodes.Count;
                int abc = 0;
                for (int i = 0; i < count; i++)
                {
                    abc++;
                    root.AppendChild(lastNode.ChildNodes[0]);
                }

            }
        }

        private void tv_Elements_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (tv_Elements.SelectedItem == null)
                mi_Load.IsEnabled = false;
            else
                mi_Load.IsEnabled = true;
           
        }

        private void tv_Elements_MouseMove(object sender, MouseEventArgs e)
        {
            if(tv_Elements.SelectedItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                XmlElement element = tv_Elements.SelectedItem as XmlElement;
                SapCompInfo ci = new SapCompInfo();
                ci.Id = element.GetAttribute("id");
                ci.Type = element.GetAttribute("type");
                ci.Name = element.GetAttribute("name");
                //ci.FindMethod = ci.GetFindCode();

                CodeDomProvider provider = CodeDomProvider.CreateProvider("c#");
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                StringBuilder sb = new StringBuilder();

                try
                {
                    using (TextWriter sourceWriter = new StringWriter(sb))
                    {
                        provider.GenerateCodeFromExpression(ci.FindMethod, sourceWriter, options);


                    }
                    DragDrop.DoDragDrop(tv_Elements, sb.ToString(), DragDropEffects.Copy);
                }
                catch
                {

                }
                
            }
        }
    }
}
