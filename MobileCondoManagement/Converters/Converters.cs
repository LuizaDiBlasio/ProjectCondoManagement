using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileCondoManagement.Converters
{
    public class IsNotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return !string.IsNullOrEmpty(s);
            }
            if (value != null)
            {
                // Se não for uma string, mas for um objeto, pode ser considerado não nulo.
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // A conversão inversa não é necessária para este conversor.
            return value;
        }
    }

    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Colors.Green : Colors.Red;
            }
            return Colors.Red; // Cor padrão para casos inválidos.
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // A conversão inversa não é necessária para este conversor.
            return value;
        }
    }
}
