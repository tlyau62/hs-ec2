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
    public byte[] ReadPhoto(int key)
    {
        return _storeService.ReadPhoto(key);
    }

    [HttpPost("photos/{key}")]
    public void WritePhoto(int key, IFormFile file)
    {
        using var ms = new MemoryStream();

        file.CopyTo(ms);
        _storeService.WritePhoto(key, ms.ToArray());
    }
}

