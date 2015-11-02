using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace SAPLogonClient.Pages.Testing
{
    class MyDataTemplateSelector:DataTemplateSelector
    {
        public DataTemplate ElementTemplate { get; set; }

        public DataTemplate LoadTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = item as XmlElement;
            if(element != null)
            {
                if(element.GetAttribute("type")=="load")
                    return LoadTemplate;
                
            }
            return ElementTemplate;
        }
    }
}
