namespace DotnetProject.Models;

public class AdmissionRecord
{
    public string County { get; set; } // ja
    public string Code { get; set; }   // n
    public string SchoolName { get; set; } // jp
    public string Institution { get; set; } // s
    public int StudentCount { get; set; } // sc
    public double? Madm { get; set; }
    public double? Mev { get; set; }
    public double? Mabs { get; set; }
    public double? Nro { get; set; }
    public string SpecializationHtml { get; set; } // sp
    public int Year { get; set; }
}