namespace Training.ProductApi.Models;


public class ImageProcessResult
{
    public string Status { get; set; } = "";

    public string? OutputPath { get; set; }

    public int? X { get; set; }

    public int? Y { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? Error { get; set; }
}