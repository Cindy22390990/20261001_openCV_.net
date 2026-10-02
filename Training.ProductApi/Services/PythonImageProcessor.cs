using System.Diagnostics;
using System.Text.Json;
using Training.ProductApi.Models;


namespace Training.ProductApi.Services;


public class PythonImageProcessor
{

    private readonly IConfiguration configuration;
    private readonly ILogger<PythonImageProcessor> logger;


    public PythonImageProcessor(
        IConfiguration configuration,
        ILogger<PythonImageProcessor> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }



    public async Task<ImageProcessResult> ExecuteAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken)
    {

        var pythonPath =
            configuration["Python:ExecutablePath"];


        var scriptPath =
            configuration["Python:ScriptPath"];



        var info = new ProcessStartInfo
        {
            FileName = pythonPath,

            RedirectStandardOutput = true,

            RedirectStandardError = true,

            UseShellExecute = false,

            CreateNoWindow = true
        };



        info.ArgumentList.Add(scriptPath!);


        info.ArgumentList.Add("--input");
        info.ArgumentList.Add(inputPath);


        info.ArgumentList.Add("--output");
        info.ArgumentList.Add(outputPath);



        using var process = new Process
        {
            StartInfo = info
        };


        process.Start();



        var outputTask =
            process.StandardOutput.ReadToEndAsync();


        var errorTask =
            process.StandardError.ReadToEndAsync();

        
        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                process.Kill(true);
            }

            return new ImageProcessResult
            {
                Status = "failed",
                Error = "Timeout"
            };
        }


        var output =
            await outputTask;


        var error =
            await errorTask;

        logger.LogInformation(
        "Python Output:{Output}",
        output);

        logger.LogError(
            "Python Error:{Error}",
            error);

        logger.LogInformation(
            "Python ExitCode:{Code}",
            process.ExitCode);

        if (process.ExitCode != 0)
        {
            return new ImageProcessResult
            {
                Status = "failed",
                Error = error
            };
        }

        if (string.IsNullOrWhiteSpace(output))
        {
            return new ImageProcessResult
            {
                Status = "failed",
                Error = "Python output is empty."
            };
        }

        ImageProcessResult? result;

        try
        {
            result = JsonSerializer.Deserialize<ImageProcessResult>(
                output,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException)
        {
            return new ImageProcessResult
            {
                Status = "failed",
                Error = "Invalid JSON."
            };
        }

        if (result == null)
        {
            return new ImageProcessResult
            {
                Status = "failed",
                Error = "Empty JSON."
            };
        }

        if (result.Status != "succeeded")
        {
            return result;
        }

        if (string.IsNullOrWhiteSpace(result.OutputPath) ||
            result.X == null ||
            result.Y == null ||
            result.Width == null ||
            result.Height == null)
        {
            return new ImageProcessResult
            {
                Status = "failed",
                Error = "Missing required fields."
            };
        }

        if (!File.Exists(result.OutputPath))
        {
            return new ImageProcessResult
            {
                Status = "failed",
                Error = "ROI file not found."
            };
        }

        var file = new FileInfo(result.OutputPath);

        if (file.Length == 0)
        {
            return new ImageProcessResult
            {
                Status = "failed",
                Error = "ROI file is empty."
            };
        }

        return result;
    }

}