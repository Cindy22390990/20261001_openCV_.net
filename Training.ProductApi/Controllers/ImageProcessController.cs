using Microsoft.AspNetCore.Mvc;
using Training.ProductApi.Models;
using Training.ProductApi.Services;


namespace Training.ProductApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{

    private readonly ImageService service;



    public ImageController(
        ImageService service)
    {
        this.service = service;
    }



    [HttpPost("process")]
    public async Task<IActionResult> Process(
        ImageProcessRequest request)
    {

        var result =
            await service.ProcessAsync(request);


        return Ok(result);

    }

}