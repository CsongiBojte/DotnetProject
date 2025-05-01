namespace DotnetProject.Models;

public class TrendPoint
{
    public int Year { get; set; }
    public string YearString => Year.ToString();

    public double LastMadm { get; set; }
}