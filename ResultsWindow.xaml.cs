using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;

namespace uni_grade_calculator
{

    public partial class ResultsWindow : MetroWindow
    {

        List<Module> ModuleList = new List<Module>();

        int TotalCredits = 0;

        double TotalAchieved;

        public ResultsWindow(List<Module> moduleList)
        {
            InitializeComponent();
            ModuleList = moduleList;
            TotalCredits = GetTotalCredits();
            LblOnTrackValue.Content = CalculateAchieved();
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

        public String CalculateAchieved()
        {
            TotalAchieved = 0;
            foreach(Module module in ModuleList)
            {
                double ModuleAchievedMod = module.AchievedPerctange / 100;
                double ModuleCredits = module.Credits;
                TotalAchieved += ModuleCredits * ModuleAchievedMod;
            }

            TotalAchieved = (TotalAchieved/TotalCredits) * 100;
            TotalAchieved = Math.Round(TotalAchieved);

            if(TotalAchieved < 40)
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
    }
}
