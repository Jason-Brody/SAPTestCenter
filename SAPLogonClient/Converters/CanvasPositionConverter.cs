using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SAPLogonClient.Converters
{
    class CanvasLeftConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double elementWidth = double.Parse(values[0].ToString());
            double canvasWidth = double.Parse(values[1].ToString());
            return (canvasWidth - elementWidth) / 2;
        }
        public object[] ConvertBack(object values, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CanvasTopConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double elementHeight = double.Parse(values[0].ToString());
            double canvasHeight = double.Parse(values[1].ToString());
            bool direction = bool.Parse(values[2].ToString());
            if(direction)
            {
                return (canvasHeight - elementHeight)/2;
            }
            else
            {
                return 0;
            }
            
        }
        public object[] ConvertBack(object values, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
