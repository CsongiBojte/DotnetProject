using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DotnetProject.Models;

namespace DotnetProject.Services;

public class HighSchoolResultsService
{
    private readonly IWebHostEnvironment _env;
    private List<StudentResult> _allResults;

    public HighSchoolResultsService(IWebHostEnvironment env)
    {
        _env = env;
        LoadData();
    }

    private void LoadData()
    {
        var path = Path.Combine(_env.WebRootPath, "data","bac", "2022_final.csv");
        if (!File.Exists(path))
        {
            _allResults = new List<StudentResult>();
            return;
        }

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null,
            PrepareHeaderForMatch = args => args.Header.ToLower()
        });

        _allResults = csv.GetRecords<StudentResult>().ToList();
    }

    public StudentResult SearchByCode(string code)
    {
        return _allResults.FirstOrDefault(r => r.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }

    public List<StudentResult> GetAll() => _allResults;
}