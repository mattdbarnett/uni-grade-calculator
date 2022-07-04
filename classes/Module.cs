using System;

public class Module
{
	public string Name { get; set;  }

	public int Credits { get; set; }

	public Module(string inputName, int inputCredits)
	{
		Name = inputName;

		Credits = inputCredits;
	}

	public string Format()
    {
		return Name + " (" + Credits.ToString() + ")";
	}
}
