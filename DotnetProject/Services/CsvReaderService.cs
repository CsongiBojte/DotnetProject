using System.Text.RegularExpressions;
using DotnetProject.Models;

namespace DotnetProject.Services;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class SpecializationStats
{
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
    
    public Dictionary<string, List<TrendPoint>> GetSpecializationTrends(List<string> specNames)
    {
        var trends = new Dictionary<string, List<TrendPoint>>();
        var years = new[] { 2021, 2022, 2023, 2024 };

        foreach (var spec in specNames)
        {
            var points = new List<TrendPoint>();

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

                var filtered = records
                    .Where(r =>
                        !string.IsNullOrWhiteSpace(r.SpecializationHtml) &&
                        r.SpecializationHtml.Contains(spec, StringComparison.OrdinalIgnoreCase) &&
                        double.TryParse(r.AdmissionAverage, out _)
                    )
                    .Select(r => double.Parse(r.AdmissionAverage))
                    .ToList();

                if (filtered.Any())
                {
                    points.Add(new TrendPoint
                    {
                        Year = year,
                        Average = filtered.Average()
                    });
                }
            }

            if (points.Any())
                trends[spec] = points.OrderBy(p => p.Year).ToList();
        }

        return trends;
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
    
    public List<TrendPoint> GetTrendFor(string county, string schoolName, string specializationName)
    {
        var years = new[] { 2021, 2022, 2023, 2024 };
        var result = new List<TrendPoint>();

        foreach (var year in years)
        {
            var path = Path.Combine(_env.WebRootPath, "data", "admitere", $"repartizare{year}.csv");
            if (!File.Exists(path)) continue;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<RepartizareModel>().ToList();

            var filtered = records
                .Where(r =>
                    r.County == county &&
                    (r.CleanedSchoolName?.Contains(schoolName, StringComparison.OrdinalIgnoreCase) ?? false) &&
                    (r.CleanedSpecialization?.Contains(specializationName, StringComparison.OrdinalIgnoreCase) ?? false) &&
                    double.TryParse(r.AdmissionAverage, out _))
                .Select(r => double.Parse(r.AdmissionAverage))
                .ToList();

            if (filtered.Any())
            {
                result.Add(new TrendPoint
                {
                    Year = year,
                    Average = filtered.Average()
                });
            }
        }

        return result;
    }
}