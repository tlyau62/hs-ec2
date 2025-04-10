using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HaystackStore;

[Route("api/[controller]")]
[ApiController]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet("photos/{key}")]
    public byte[] ReadPhoto(int key)
    {
        return _storeService.ReadPhoto(key);
    }

    [HttpPost("photos/{key}")]
    public void WritePhoto(int key, byte[] data)
    {
        _storeService.WritePhoto(key, data);
    }
}

