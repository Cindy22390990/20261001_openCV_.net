using Training.ProductApi.Models;


namespace Training.ProductApi.Services;


public class ImageService
{

    private readonly PythonImageProcessor processor;


    public ImageService(
        PythonImageProcessor processor)
    {
        this.processor = processor;
    }



    public async Task<ImageProcessResult> ProcessAsync(
    ImageProcessRequest request)
    {
        using var cts =
            new CancellationTokenSource(
                TimeSpan.FromSeconds(10)
            );

        return await processor.ExecuteAsync(
            request.InputPath,
            request.OutputPath,
            cts.Token
        );
    }


}

