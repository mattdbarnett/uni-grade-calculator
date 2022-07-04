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
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;

namespace uni_grade_calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        // -- Add Assessment Grid
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //<--- Reference start (https://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf)
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            // Reference end --->
        }

        private String PercentValidationTextBox(String value)
        {
            int input;
            bool success = int.TryParse(value.TrimEnd(new char[] { '%', ' ' }), out input);
            if (success)
            {
                if (input > 100)
                {
                    return "100%";
                }
                else if (input < 0)
                {
                    return "0%";
                }
                else
                {
                    return input.ToString() + "%";
                }
            }
            return "0%";
        }

        private void SliderWeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtbxWeightValue.Text = SliderWeight.Value.ToString();
        }

        private void SliderMark_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtbxMarkValue.Text = Math.Round(SliderMark.Value, 0).ToString() + "%";
        }

        private void TxtbxWeightValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtbxWeightValue.Text = PercentValidationTextBox(TxtbxWeightValue.Text);
            TxtbxWeightValue.SelectionStart = TxtbxWeightValue.Text.Length - 1;
        }

        private void TxtbxMarkValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtbxMarkValue.Text = PercentValidationTextBox(TxtbxMarkValue.Text);
            TxtbxMarkValue.SelectionStart = TxtbxMarkValue.Text.Length - 1;
        }
    }
}
