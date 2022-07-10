using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using TextBox = System.Windows.Controls.TextBox;
using System.Diagnostics;

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

            EnableAddModuleButtons(false);
        }

        // -- Add Module Grid --

        private void BtnModuleAdd_Click(object sender, RoutedEventArgs e)
        {
            if (TxtbxModuleName.Text == null || TxtbxModuleName.Text.Length == 0)
            {
                System.Windows.MessageBox.Show("Please enter a module name.", 
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            } 
            else if (TxtbxModuleCredits.Text == null || TxtbxModuleCredits.Text.Length == 0)
            {
                System.Windows.MessageBox.Show("Please enter the module credits.", 
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            } 
            else
            {
                Module NewModule = new Module(TxtbxModuleName.Text, int.Parse(TxtbxModuleCredits.Text));

                if(SwitchCompleted.IsOn && TxtbxTotal.Text != "")
                {
                    Assessment NewAssessment = new Assessment("Total Score", 100, 
                        int.Parse(TxtbxTotal.Text.TrimEnd(new char[] { '%', ' ' })));
                    NewModule.AddAssessment(NewAssessment);
                    NewModule.CalculatePerctange();
                }

                ModuleList.Add(NewModule);
                LtbxModules.Items.Add(NewModule.Format());

                ClearAddModule();
            }
        }

        private void SwitchCompleted_Toggled(object sender, RoutedEventArgs e)
        {
            bool value = SwitchCompleted.IsOn;

            if(value)
            {
                LblTotal.Visibility = Visibility.Visible;
                TxtbxTotal.Visibility = Visibility.Visible;
            }
            else
            {
                LblTotal.Visibility = Visibility.Hidden;
                TxtbxTotal.Visibility = Visibility.Hidden;
            }
        }

        private void ClearAddModule()
        {
            TxtbxModuleName.Clear();
            TxtbxModuleCredits.Clear();
            TxtbxTotal.Clear();
            SwitchCompleted.IsOn = false;
        }

        private void TxtbxTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtbxTotal.Text = PercentValidationTextBox(TxtbxTotal.Text);
            TxtbxTotal.SelectionStart = TxtbxTotal.Text.Length - 1;
        }


        // -- List Modules Grid --

        private void LtbxModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableAddModuleButtons(true);
            UpdateAssessmentListBox();
        }

        private void BtnAddAs_Click(object sender, RoutedEventArgs e)
        {
            if(LtbxModules.SelectedIndex != -1)
            {
                ShowAssessmentSection();
            }
        }
        private void BtnDelMd_Click(object sender, RoutedEventArgs e)
        {
            int itemIndex = LtbxModules.SelectedIndex;
            ModuleList.RemoveAt(itemIndex);
            LtbxModules.Items.RemoveAt(itemIndex);
        }

        private void BtnClearMd_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete all modules?", 
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                ModuleList.Clear();
                LtbxModules.Items.Clear();
            }
        }

        private void EnableAddModuleButtons(bool value)
        {
            BtnAddAs.IsEnabled = value;
            BtnDelMd.IsEnabled = value;
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

            if (AssessmentName == null || AssessmentName.Length == 0)
            {
                System.Windows.MessageBox.Show("Please enter the assessment name.", 
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AssessmentWeight == 0)
            {
                System.Windows.MessageBox.Show("Assessment weight cannot be zero.", 
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Assessment NewAssessment = new Assessment(AssessmentName, AssessmentWeight, AssessmentMark);

            Module SelectedModule = ModuleList[LtbxModules.SelectedIndex];

            int WeightTotal = AssessmentWeight;
            foreach (var assessment in SelectedModule.Assessments)
            {
                WeightTotal += assessment.Weight;
            }

            if (WeightTotal > 100)
            {
                System.Windows.MessageBox.Show("Total module weight cannot be higher than 100%.", 
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedModule.AddAssessment(NewAssessment);
            UpdateAssessmentListBox();
            ClearAddAssessment();

            SelectedModule.CalculatePerctange();
        }
        
        private void ClearAddAssessment()
        {
            TxtbxAssessmentName.Text = "";
            SliderMark.Value = 0;
            SliderWeight.Value = 0;
        }


        // -- View Assessment Grid --

        private void UpdateAssessmentListBox()
        {
            if (ModuleList.Count > 0 && LtbxModules.SelectedIndex != -1)
            {
                Module SelectedModule = ModuleList[LtbxModules.SelectedIndex];

                LtbxAssessments.Items.Clear();
                foreach (var assessment in SelectedModule.Assessments)
                {
                    LtbxAssessments.Items.Add(assessment.Format());
                }
            }
            else
            {
                ClearAddAssessment();
                EnableAddModuleButtons(false);
            }
        }


        // -- General Control --

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (ModuleList.Count > 0)
            {
                ResultsWindow resultsWindow = new ResultsWindow(ModuleList);
                try
                {
                    resultsWindow.Show();
                }
                catch (InvalidOperationException)
                {
                    System.Windows.MessageBox.Show("An error occured when trying to calculate your grade. " +
                        "Please ensure there is at minimum one module with one marked assessment entered.",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter modules.", 
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ShowModuleSection();
        }

        private void TBBtnNew_Click(Object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Are you sure? You will lose all unsaved progress.",
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                ModuleList.Clear();
                LtbxModules.Items.Clear();
                ClearAddModule();

                LtbxAssessments.Items.Clear();
                ClearAddAssessment();

                ShowModuleSection();
            }
        }

        private void TBBtnHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/mattdbarnett/uni-grade-calculator") { UseShellExecute = true });
        }

        private void ShowModuleSection()
        {
            MAddBorder.Visibility = Visibility.Visible;
            MListBorder.Visibility = Visibility.Visible;

            AAddBorder.Visibility = Visibility.Hidden;
            AListBorder.Visibility = Visibility.Hidden;

            TlbBack.Visibility = Visibility.Hidden;
            TlbCalc.Visibility = Visibility.Visible;

            ClearAddAssessment();
        }
        private void ShowAssessmentSection()
        {
            MAddBorder.Visibility = Visibility.Hidden;
            MListBorder.Visibility = Visibility.Hidden;

            AAddBorder.Visibility = Visibility.Visible;
            AListBorder.Visibility = Visibility.Visible;

            TlbBack.Visibility = Visibility.Visible;
            TlbCalc.Visibility = Visibility.Hidden;



            LblAddAssessment.Content = "Add Assessment to " + FormatLblAddAssessment(ModuleList[LtbxModules.SelectedIndex].Name.ToString());
        }

        private String FormatLblAddAssessment(String input)
        {
            String result;
            if(input.Length > 9)
            {
                result = input.Substring(0, 6) + "...";
            }
            else
            {
                result = input;
            }
            return result;
        }
    }
}
