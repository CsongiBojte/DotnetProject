using System.Text.Json.Serialization;

namespace DotnetProject.Models;

public class PredictionResponse
{
    [JsonPropertyName("predicted_madm")]
    public double PredictedMadm { get; set; }
}