using System;
using System.Linq;

using Xamarin.Forms;

namespace KegMaster.Core.Utilities
{
    public class InputValidation
    {
        public InputValidation()
        {
        }
        async void NumericValidationBehavior(object sender, TextChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x)); //Make sure all characters are numbers

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }
        }
    }
}
