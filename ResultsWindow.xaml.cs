using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;

namespace uni_grade_calculator
{

    public partial class ResultsWindow : MetroWindow
    {

        List<Module> ModuleList = new List<Module>();

        int TotalCredits = 0;

        double TotalOverall = 0;

        double TotalCompleted = 0;

        double TotalAchieved = 0;


        public ResultsWindow(List<Module> moduleList)
        {
            InitializeComponent();
            ModuleList = moduleList;
            TotalCredits = GetTotalCredits();

            String GradeAchieved = CalculateAchieved();
            LblOnTrackValue.Content = GradeAchieved;

            switch (GradeAchieved)
            {
                case "Fail":
                    LblOnTrackValue.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case "Third-Class Honours":
                    LblOnTrackValue.Foreground = new SolidColorBrush(Colors.Green);
                    break;
                case "Lower Second-Class Honours":
                    LblOnTrackValue.Foreground = new SolidColorBrush(Colors.Green);
                    break;
                case "Upper Second-Class Honours":
                    LblOnTrackValue.Foreground = new SolidColorBrush(Colors.Green);
                    break;
                case "First-Class Honours":
                    LblOnTrackValue.Foreground = new SolidColorBrush(Colors.Gold);
                    break;
            }

            CalculateOverall();
            CalculateCompleted();

            SetProgress();
        }

        private int GetTotalCredits()
        {
            int TotalCredits = 0;
            foreach (Module module in ModuleList)
            {
                TotalCredits += module.Credits;
            }
            return TotalCredits;
        }

        public void CalculateOverall()
        {
            TotalOverall = 0;
            foreach (Module module in ModuleList)
            {
                double ModuleOverallMod = module.OverallPercentage / 100;
                double ModuleCredits = module.Credits;
                TotalOverall += ModuleCredits * ModuleOverallMod;
            }

            TotalOverall = (TotalOverall / TotalCredits) * 100;
            TotalOverall = Math.Round(TotalOverall);
        }

        public void CalculateCompleted()
        {
            TotalCompleted = 0;
            foreach (Module module in ModuleList)
            {
                double ModuleCompletedMod = module.CompletedPercentage / 100;
                double ModuleCredits = module.Credits;
                TotalCompleted += ModuleCredits * ModuleCompletedMod;
            }

            TotalCompleted = (TotalCompleted / TotalCredits) * 100;
            TotalCompleted = Math.Round(TotalCompleted);
        }

        public String CalculateAchieved()
        {
            TotalAchieved = 0;
            int AchievedCredits = TotalCredits;
            foreach(Module module in ModuleList)
            {
                if (module.Assessments.Count > 0)
                {
                    double ModuleAchievedMod = module.AchievedPercentage / 100;
                    double ModuleCredits = module.Credits;
                    TotalAchieved += ModuleCredits * ModuleAchievedMod;
                }
                else
                {
                    AchievedCredits -= module.Credits;
                }
            }

            if (AchievedCredits > 0)
            {
                TotalAchieved = (TotalAchieved / AchievedCredits) * 100;
                TotalAchieved = Math.Round(TotalAchieved);
                Debug.WriteLine(TotalAchieved);
            } 
            else
            {
                this.Close();
            }

            if (TotalAchieved < 40)
            {
                return "Fail";
            }

            if(TotalAchieved <= 50)
            {
                return "Third-Class Honours";
            }

            if(TotalAchieved <= 60)
            {
                return "Lower Second-Class Honours";
            }

            if(TotalAchieved <= 70)
            {
                return "Upper Second-Class Honours";
            }

            if(TotalAchieved > 70)
            {
                return "First-Class Honours";
            }

            return "ERROR ???";
        }

        private void SetProgress()
        {
            PBAchieved.Value = TotalAchieved;
            TBoxAchieved.Text = "If you graduated tomorrow, with only your current work accounted for, you would have achieved " + PBAchieved.Value.ToString() + "% of the possible marks.";
            PBOverall.Value = TotalOverall;
            TBoxOverall.Text = "Including the assessments that haven't been marked or submitted yet, you have achieved " + PBOverall.Value.ToString() + "% of the marks in the modules you've started.";
            PBCompleted.Value = TotalCompleted;
            TBoxCompleted.Text = "You have completed " + PBCompleted.Value.ToString() + "% of your submitted modules.";
        }
    }
}
