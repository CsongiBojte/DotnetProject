using System.Text.RegularExpressions;
using DotnetProject.Models;

namespace DotnetProject.Services;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class SpecializationStats
{
    public int Rank { get; set; }  
    public string Specialization { get; set; }
    public double Average { get; set; }
    public int Count { get; set; }
}

public class CsvReaderService
{
    private readonly IWebHostEnvironment _env;

    public CsvReaderService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public List<RepartizareModel> LoadAll()
    {
        var filePath = Path.Combine(_env.WebRootPath, "data", "admitere", "repartizare2024.csv");

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        return csv.GetRecords<RepartizareModel>().ToList();
    }
    
    public List<RepartizareModel> LoadByYear(int year)
    {
        var filePath = Path.Combine(_env.WebRootPath, "data", "admitere", $"repartizare{year}.csv");

        if (!File.Exists(filePath))
            return new List<RepartizareModel>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        return csv.GetRecords<RepartizareModel>().ToList();
    }
    
    public List<SpecializationStats> GetAverageBySpecialization(int year)
    {
        var records = LoadByYear(year);

        return records
            .Where(r => !string.IsNullOrWhiteSpace(r.SpecializationHtml) &&
                        double.TryParse(r.AdmissionAverage, out _))
            .GroupBy(r =>
            {
                // Extract: <b>(110) Matematică-Informatică</b><br/>Limba română
                var match = Regex.Match(r.SpecializationHtml ?? "", @"\)\s*(.*?)</b>");
                return match.Success ? match.Groups[1].Value : "Ismeretlen";
            })
            .Select(g => new SpecializationStats
            {
                Specialization = g.Key,
                Average = g.Average(r => double.Parse(r.AdmissionAverage)),
                Count = g.Count()
            })
            .OrderByDescending(s => s.Average)
            .Take(10)
            .ToList();
    }
    
    public List<string> GetAvailableSpecializations()
    {
        var years = new[] { 2021, 2022, 2023, 2024 };
        var allSpecs = new HashSet<string>();

        foreach (var year in years)
        {
            var filePath = Path.Combine(_env.WebRootPath, "data", "admitere", $"repartizare{year}.csv");
            if (!File.Exists(filePath)) continue;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<RepartizareModel>().ToList();

            foreach (var record in records)
            {
                if (!string.IsNullOrWhiteSpace(record.SpecializationHtml))
                {
                    var match = Regex.Match(record.SpecializationHtml, @"\)\s*(.*?)</b>");
                    if (match.Success)
                    {
                        allSpecs.Add(match.Groups[1].Value.Trim());
                    }
                }
            }
        }

        return allSpecs.OrderBy(s => s).ToList();
    }
}