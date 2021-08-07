using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Panini.ValidationRules
{
    class IntegerValidationRule : ValidationRule
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            int number;
            bool success = int.TryParse((string)value, out number);
            if (!success)
            {
                return new ValidationResult(false, $"Enter an integer value between {Min} and {Max}!");
            }
            return ValidationResult.ValidResult;
        }
    }
}
