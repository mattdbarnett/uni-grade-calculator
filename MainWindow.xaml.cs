/*
- - - - - - - - - - - - - - -
 Title: MainWindow
 Author: Matt Barnett
 Created: 03/07/2022    
 Last Modified: 21/06/2022
- - - - - - - - - - - - - - -
*/

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Security.Cryptography;
using MahApps.Metro.Controls;

namespace uni_grade_calculator
{

    public partial class MainWindow : MetroWindow
    {
        readonly List<Module> ModuleList = new();

        private ObservableCollection<ModuleDisplay> moduleDisplayList = new();
        public ObservableCollection<ModuleDisplay> ModuleDisplayList { get => moduleDisplayList; set => moduleDisplayList = value; }

        // Initalise application
        public MainWindow()
        {
            InitializeComponent();

            LtbxModules.ItemsSource = ModuleDisplayList;

            EnableAddModuleButtons(false);
            EnableAddAssessmentButtons(false);
        }

        // --- Add Module Grid

        // Button for adding a module to the module list
        private void BtnModuleAdd_Click(object sender, RoutedEventArgs e)
        {
            // If no name or credits, return error. Else, add module to module list.
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
                Module NewModule = new(TxtbxModuleName.Text, int.Parse(TxtbxModuleCredits.Text));

                // If completed switch is on, parse percent.
                if(SwitchCompleted.IsOn && TxtbxTotal.Text != "")
                {
                    Assessment NewAssessment = new Assessment("Total Score", 100, 
                        int.Parse(TxtbxTotal.Text.TrimEnd(new char[] { '%', ' ' })));
                    NewModule.AddAssessment(NewAssessment);
                    NewModule.CalculatePerctange();
                }

                ModuleList.Add(NewModule);

                UpdateModuleListBox();
                ClearAddModule();
            }
        }

        // If completed switch is on, show completed textbox.
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

        // Clears add module widgets.
        private void ClearAddModule()
        {
            TxtbxModuleName.Clear();
            TxtbxModuleCredits.Clear();
            TxtbxTotal.Clear();
            SwitchCompleted.IsOn = false;
        }

        // When completed textbox has input, add percent to end.
        private void TxtbxTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtbxTotal.Text = PercentValidationTextBox(TxtbxTotal.Text);
            TxtbxTotal.SelectionStart = TxtbxTotal.Text.Length - 1;
        }


        // --- List Modules Grid

        // Class used to display modules in modules listbox.
        // Instanced for each module in module list.
        public class ModuleDisplay
        {
            public string Name { get; set; }
            public string Percent { get; set; }

            public ModuleDisplay(string inputName, string inputPercent)
            {
                Name = inputName;

                Percent = inputPercent;
            }
        }

        // Creates ModuleDisplay for each module in module list.
        public void UpdateModuleListBox()
        {
            ModuleDisplayList.Clear();
            foreach(Module module in ModuleList)
            {
                ModuleDisplay moduleDisplay = new ModuleDisplay(module.Name, module.AchievedPercentage.ToString() + "%");
                ModuleDisplayList.Add(moduleDisplay);
            }
        }

        // If module is selected, enable module buttons and update assessment content.
        private void LtbxModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableAddModuleButtons(true);
            UpdateAssessmentListBox();
        }

        // If module is selected, switch to assessment section for selected module.
        private void BtnAddMd_Click(object sender, RoutedEventArgs e)
        {
            if(LtbxModules.SelectedIndex != -1)
            {
                ShowAssessmentSection();
            }
        }

        // If module is selected, delete module from module list.
        private void BtnDelMd_Click(object sender, RoutedEventArgs e)
        {
            if (LtbxModules.SelectedIndex != -1)
            {
                int itemIndex = LtbxModules.SelectedIndex;
                ModuleList.RemoveAt(itemIndex);
                UpdateModuleListBox();
            }
        }

        // If yes response given, resets module list.
        private void BtnClearMd_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete all modules?", 
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                ModuleList.Clear();
                ModuleDisplayList.Clear();
            }
        }

        // Toggles add module buttons based on input value.
        private void EnableAddModuleButtons(bool value)
        {
            BtnAddMd.IsEnabled = value;
            BtnDelMd.IsEnabled = value;
        }


        // --- Add Assessment Grid 

        // Ensures content is only numeric.
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //<--- Reference start (https://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf)
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            // Reference end --->
        }

        // Ensures content is between 0 - 100 and is a percentage.
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

        // Adjusts weight value when slider is changed.
        private void SliderWeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtbxWeightValue.Text = SliderWeight.Value.ToString();
        }

        // Adjusts mark value when slider is changed.
        private void SliderMark_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtbxMarkValue.Text = Math.Round(SliderMark.Value, 0).ToString() + "%";
        }

        // Validates content inputted into weight textbox.
        private void TxtbxWeightValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtbxWeightValue.Text = PercentValidationTextBox(TxtbxWeightValue.Text);
            TxtbxWeightValue.SelectionStart = TxtbxWeightValue.Text.Length - 1;
        }

        // Validates content inputted into mark textbox.
        private void TxtbxMarkValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtbxMarkValue.Text = PercentValidationTextBox(TxtbxMarkValue.Text);
            TxtbxMarkValue.SelectionStart = TxtbxMarkValue.Text.Length - 1;
        }

        // If name are weight are present, adds assessment to current module.
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

        // Clears add assessment widgets.
        private void ClearAddAssessment()
        {
            TxtbxAssessmentName.Text = "";
            SliderMark.Value = 0;
            SliderWeight.Value = 0;
        }


        // -- View Assessment Grid --

        // Updates the content in the assessment list box if a module is selected.
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

        // If an assessment is selected, enable add assessment buttons.
        private void LtbxAssessments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LtbxAssessments.SelectedIndex != -1)
            {
                EnableAddAssessmentButtons(true);
            } 
            else
            {
                EnableAddAssessmentButtons(false);
            }
        }

        // Deletes selected assessment from current module.
        private void BtnDelAs_Click(object sender, RoutedEventArgs e)
        {
            Module SelectedModule = ModuleList[LtbxModules.SelectedIndex];
            int itemIndex = LtbxAssessments.SelectedIndex;
            SelectedModule.Assessments.RemoveAt(itemIndex);
            SelectedModule.CalculatePerctange();
            UpdateAssessmentListBox();
        }

        // If response is yes, clears all assessments from the current module.
        private void BtnClearAs_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete all assessments?",
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                Module SelectedModule = ModuleList[LtbxModules.SelectedIndex];
                SelectedModule.Assessments.Clear();
                SelectedModule.CalculatePerctange();
                LtbxAssessments.Items.Clear();
            }
        }

        // Toggles add assessment buttons based on input value.
        private void EnableAddAssessmentButtons(bool value)
        {
            BtnDelAs.IsEnabled = value;
        }


        // --- General Control

        // If module list has valid content, creates results window using module list.
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

        // Returns user from assessment section to module section.
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ShowModuleSection();
        }

        // If response is yes, clears all content.
        private void TBBtnNew_Click(Object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Are you sure? You will lose all unsaved progress.",
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                ModuleList.Clear();
                ClearAddModule();

                LtbxAssessments.Items.Clear();
                ClearAddAssessment();

                ShowModuleSection();
            }
        }

        // To be implemented!!!
        private void TBBtnOpen_Click(Object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
        }

        // Saves encrypted progress as file to selected folder.
        private void TBBtnSave_Click(Object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                String path = folderBrowserDialog.SelectedPath;
                List<string> saveList = new List<string>();
                foreach (Module module in ModuleList)
                {
                    String newItem = "||MODULE//";
                    newItem += module.Name + "//";
                    newItem += module.Credits + "//";
                    foreach (Assessment assessment in module.Assessments)
                    {
                        newItem += assessment.Name + "//";
                        newItem += assessment.Weight + "//";
                        newItem += assessment.Mark + "//";
                    }
                    saveList.Add(newItem);
                }

                List<string> encryptedSaveList = new List<string>();
                foreach (String module in saveList)
                {
                    if (OperatingSystem.IsWindows())
                    {
                        encryptedSaveList.Add(Convert.ToBase64String(
                                     ProtectedData.Protect(
                                     Encoding.Unicode.GetBytes(module),
                                     null,
                                     DataProtectionScope.LocalMachine)));
                    } 
                }

                File.WriteAllLinesAsync(path + "/newsave.unicalc", encryptedSaveList);
            }
        }

        // Opens GitHub repository in default browser.
        private void TBBtnHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/mattdbarnett/uni-grade-calculator") { UseShellExecute = true });
        }

        // If response is yes, closes application.
        private void TBBtnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("Are you sure? You will lose all unsaved progress.",
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        // Switches to module section, hides all assessment content.
        private void ShowModuleSection()
        {
            MAddBorder.Visibility = Visibility.Visible;
            MListBorder.Visibility = Visibility.Visible;

            AAddBorder.Visibility = Visibility.Hidden;
            AListBorder.Visibility = Visibility.Hidden;

            TlbBack.Visibility = Visibility.Hidden;
            TlbCalc.Visibility = Visibility.Visible;

            UpdateModuleListBox();
            ClearAddAssessment();
        }

        // Switches to assessment section, hides all module content.
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

        // Formats currently selected module name for use in add assessment title.
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
