using System.Text.RegularExpressions;
using CsvHelper.Configuration.Attributes;

public class RepartizareModel
{
    [Name("ja")]
    public string County { get; set; }

    [Name("madm")]
    public string AdmissionAverage { get; set; }

    [Name("h")]
    public string HighSchoolHtml { get; set; }

    [Name("sp")]
    public string SpecializationHtml { get; set; }
    
    public string CleanedSchoolName => Regex.Match(HighSchoolHtml ?? "", @"<b>(.*?)</b>")?.Groups[1].Value ?? "";
    public string CleanedSpecialization => Regex.Match(SpecializationHtml ?? "", @"\)\s*(.*?)</b>")?.Groups[1].Value ?? "";
}

