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
}