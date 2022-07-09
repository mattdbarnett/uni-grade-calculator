﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using ControlzEx.Theming;
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

            // TO-DO: Implement theme sync
            //ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            //ThemeManager.Current.SyncTheme();

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

        private void EnableAddModuleButtons(bool value)
        {
            BtnAddAs.IsEnabled = value;
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
                MessageBox.Show("Please enter the assessment name.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AssessmentWeight == 0)
            {
                MessageBox.Show("Assessment weight cannot be zero.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Total module weight cannot be higher than 100%.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedModule.AddAssessment(NewAssessment);
            UpdateAssessmentListBox();
            ClearAddAssessment();

            SelectedModule.CalculatePerctange();
            //Console.WriteLine(SelectedModule.OverallPercentage);
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


        // -- View Assessment Grid --

        private void UpdateAssessmentListBox()
        {
            Module SelectedModule = ModuleList[LtbxModules.SelectedIndex];

            LtbxAssessments.Items.Clear();
            foreach (var assessment in SelectedModule.Assessments)
            {
                LtbxAssessments.Items.Add(assessment.Format());
            }
        }


        // -- General Control --

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            ResultsWindow resultsWindow = new ResultsWindow(ModuleList);
            resultsWindow.Show();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ShowModuleSection();
        }

        private void ShowModuleSection()
        {
            MAddBorder.Visibility = Visibility.Visible;
            MListBorder.Visibility = Visibility.Visible;

            AAddBorder.Visibility = Visibility.Hidden;
            AListBorder.Visibility = Visibility.Hidden;

            TlbBack.Visibility = Visibility.Hidden;
            TlbCalc.Visibility = Visibility.Visible;
        }
        private void ShowAssessmentSection()
        {
            MAddBorder.Visibility = Visibility.Hidden;
            MListBorder.Visibility = Visibility.Hidden;

            AAddBorder.Visibility = Visibility.Visible;
            AListBorder.Visibility = Visibility.Visible;

            TlbBack.Visibility = Visibility.Visible;
            TlbCalc.Visibility = Visibility.Hidden;

            LblAddAssessment.Content = "Add Assessment to " + ModuleList[LtbxModules.SelectedIndex].Name.ToString();
        }
    }
}
