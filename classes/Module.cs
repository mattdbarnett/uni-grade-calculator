using System;
using System.Collections.Generic;

public class Module
{
	public string Name { get; set;  }

	public int Credits { get; set; }

	public List<Assessment> Assessments { get; set; } = new List<Assessment>();

	public Module(string inputName, int inputCredits)
	{
		Name = inputName;

		Credits = inputCredits;

		// -- TEMP TEST --
		//Assessment TestAssessment = new Assessment("TEST", 20, 20);
		//AddAssessment(TestAssessment);
	}

	public string Format()
    {
		return Name + " (" + Credits.ToString() + ")";
	}

	public void AddAssessment(Assessment assessment)
    {
		Assessments.Add(assessment);
    }
}
