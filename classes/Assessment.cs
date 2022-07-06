public class Assessment
{
	public string Name { get; set; }

	public int Weight { get; set; }

	public int Mark { get; set; }

	public Assessment(string inputName, int inputWeight, int inputMarks)
	{
		Name = inputName;
		Weight = inputWeight;
		Mark = inputMarks;
	}

	public string Format()
	{
		return Name + " (Weight: " + Weight.ToString() + "%, Marks: " + Mark.ToString() + "%)";
	}
}
