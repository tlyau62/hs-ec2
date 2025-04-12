using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HaystackStore;

[Route("api/store")]
[ApiController]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet("photos/test")]
    public string Test()
    {
        return "OK";
    }

    [HttpGet("photos/{key}")]
    public IActionResult ReadPhoto(int key)
    {
        var photo = _storeService.ReadPhoto(key);

        if (photo == null)
        {
            return NotFound(key);
        }

        return File(photo, "image/png");
    }

    [HttpPost("photos/{key}")]
    public void WritePhoto(int key, IFormFile file)
    {
        using var ms = new MemoryStream();

        file.CopyTo(ms);
        _storeService.WritePhoto(key, ms.ToArray());
    }

    [HttpPost("photos")]
    public void UnpackPhotos(IFormFile file, [FromForm] string keyPattern = "^\\d+")
    {
        using var ms = new MemoryStream();

        file.CopyTo(ms);
        _storeService.UnpackPhotos(keyPattern, ms.ToArray());
    }
}

