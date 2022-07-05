using System;
using System.Collections.Generic;

public class Module
{
	public string Name { get; set;  }

	public int Credits { get; set; }

	public List<Assessment> Assessments { get; set; } = new List<Assessment>();

	public double OverallPerctange { get; set; } // Percentage achieved in total

	public double AchievedPerctange { get; set; } // Percentage achieved not including not-completed modules

	public double CompletedPercentage { get; set; }	// Percentage of how much of the module has been completed

	public Module(string inputName, int inputCredits)
	{
		Name = inputName;

		Credits = inputCredits;
	}

	public string Format()
    {
		return Name + " (" + Credits.ToString() + ")";
	}

	public void AddAssessment(Assessment assessment)
    {
		Assessments.Add(assessment);
    }

	public void CalculatePerctange()
    {

		// Calculate overall percentage
		OverallPerctange = 0;
		foreach (var assessment in Assessments)
        {
			double WeightPercent = assessment.Weight;
			WeightPercent /= 100;
			double CurrentMark = assessment.Mark * WeightPercent;
			OverallPerctange += CurrentMark;
        }

		// Calculate completed percentage
		CompletedPercentage = 0;
		foreach (var assessment in Assessments)
        {
			CompletedPercentage += assessment.Weight;
        }

		// Calculate achieved percentage
		AchievedPerctange = 0;
		foreach (var assessment in Assessments)
        {
			double WeightPercent = assessment.Weight;
			WeightPercent /= CompletedPercentage;
			double CurrentMark = assessment.Mark * WeightPercent;
			OverallPerctange += CurrentMark;
		}
    }
}
