using DotnetProject.Models;

namespace DotnetProject.Services;

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

public class AdmissionService
{
    private readonly IWebHostEnvironment _env;

    public AdmissionService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<List<AdmissionRecord>> LoadAllCsvAsync()
    {
        var dataFolder = Path.Combine(_env.WebRootPath, "data", "admitere");
        var files = Directory.GetFiles(dataFolder, "repartizare*.csv");
        var allRecords = new List<AdmissionRecord>();

        foreach (var file in files)
        {
            var year = ExtractYearFromFilename(file);
            using var reader = new StreamReader(file);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                HeaderValidated = null, // ne dobjon hibát hiányzó fejlécre
                MissingFieldFound = null
            });

            var records = csv.GetRecords<AdmissionRecord>().ToList();
            records.ForEach(r => r.Year = year);
            allRecords.AddRange(records);
        }

        return allRecords;
    }

    private int ExtractYearFromFilename(string filePath)
    {
        var match = Regex.Match(filePath, @"repartizare(\d{4})");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}