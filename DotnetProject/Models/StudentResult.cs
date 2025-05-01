namespace DotnetProject.Models;

using CsvHelper.Configuration.Attributes;

public class StudentResult
{
    [Name("code")]
    public string Code { get; set; }

    [Name("school")]
    public string School { get; set; }

    [Name("county")]
    public string County { get; set; }

    [Name("specialization")]
    public string Specialization { get; set; }

    [Name("avg")]
    public double Average { get; set; }

    [Name("passed")]
    public int Passed { get; set; }

    [Name("year")]
    public int Year { get; set; }

    [Name("full_school_name")]
    public string FullSchoolName { get; set; }

    [Name("mandatory_subject")]
    public string MandatorySubject { get; set; }

    [Name("chosen_subject")]
    public string ChosenSubject { get; set; }

    [Name("romanian_grade_initial")]
    public double RomanianGradeInitial { get; set; }

    [Name("mandatory_grade_initial")]
    public double MandatoryGradeInitial { get; set; }

    [Name("chosen_grade_initial")]
    public double ChosenGradeInitial { get; set; }
}