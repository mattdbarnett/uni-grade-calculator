using System;

public class Assessment
{
	public string Name { get; set; }

	public int Weight { get; set; }

	public int Marks { get; set; }

	public Assessment(string inputName, int inputWeight, int inputMarks)
	{
		Name = inputName;
		Weight = inputWeight;
		Marks = inputMarks;
	}

	public string Format()
	{
		return Name + " (Weight:" + Weight.ToString() + "%, Marks:" + Marks.ToString() + "%)";
	}
}
