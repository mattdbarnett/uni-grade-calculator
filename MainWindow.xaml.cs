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
using static Module;

namespace uni_grade_calculator
{

    public partial class MainWindow : MetroWindow
    {

        List<Module> ModuleList = new List<Module>();

        List<TextBox> AddTextboxesList;

        List<Slider> AddSlidersList;

        public MainWindow()
        {
            InitializeComponent();

            TextBox[] addTextboxes = { TxtbxAssessmentName, TxtbxWeightValue, TxtbxMarkValue };
            AddTextboxesList = new List<TextBox>(addTextboxes);

            Slider[] addSliders = { SliderMark, SliderWeight };
            AddSlidersList = new List<Slider>(addSliders);

            SetAddAssessmentEnabled(false);
        }

        // -- Add Module Grid --

        private void BtnModuleAdd_Click(object sender, RoutedEventArgs e)
        {
            if (TxtbxModuleName.Text == null || TxtbxModuleName.Text.Length == 0)
            {
                MessageBox.Show("Please enter a module name.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            } 
            else if (TxtbxModuleCredits.Text == null || TxtbxModuleCredits.Text.Length == 0)
            {
                MessageBox.Show("Please enter the module credits.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            } 
            else
            {
                Module NewModule = new Module(TxtbxModuleName.Text, int.Parse(TxtbxModuleCredits.Text));

                ModuleList.Add(NewModule);
                LtbxModules.Items.Add(NewModule.Format());

                TxtbxModuleName.Clear();
                TxtbxModuleCredits.Clear();
            }
        }


        // -- List Modules Grid --

        private void LtbxModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAddAssessmentEnabled(true);

            UpdateAssessmentListBox();
        }


        // -- Add Assessment Grid --

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

        private void BtnAssessmentAdd_Click(object sender, RoutedEventArgs e)
        {
            String AssessmentName = TxtbxAssessmentName.Text;
            int AssessmentWeight = int.Parse(TxtbxWeightValue.Text.TrimEnd(new char[] { '%', ' ' }));
            int AssessmentMark = int.Parse(TxtbxMarkValue.Text.TrimEnd(new char[] { '%', ' ' }));

            if(AssessmentName == null || AssessmentName.Length == 0)
            {
                MessageBox.Show("Please enter the assessment name.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(AssessmentWeight == 0)
            {
                MessageBox.Show("Assessment weight cannot be zero.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Assessment NewAssessment = new Assessment(AssessmentName, AssessmentWeight, AssessmentMark);

            Module SelectedModule = ModuleList[LtbxModules.SelectedIndex];

            SelectedModule.AddAssessment(NewAssessment);
            UpdateAssessmentListBox();
            ClearAddAssessment();
        }

        private void SetAddAssessmentEnabled(bool input)
        {
            foreach (var textbox in AddTextboxesList)
            {
                textbox.IsEnabled = input;
            }
            foreach (var slider in AddSlidersList)
            {
                slider.IsEnabled = input;
            }
            BtnAssessmentAdd.IsEnabled = input;
        }
        
        private void ClearAddAssessment()
        {
            foreach (var textbox in AddTextboxesList)
            {
                textbox.Text = string.Empty;
            }
            foreach (var slider in AddSlidersList)
            {
                slider.Value = 0;
            }
        }

        // -- View Assessment Grid --#

        private void UpdateAssessmentListBox()
        {
            Module SelectedModule = ModuleList[LtbxModules.SelectedIndex];

            LtbxAssessments.Items.Clear();
            foreach (var assessment in SelectedModule.Assessments)
            {
                LtbxAssessments.Items.Add(assessment.Format());
            }
        }

    }
}
