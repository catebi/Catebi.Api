using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController(IFileService FileService) : ControllerBase
{

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        var file = await FileService.GetFileByName(fileName);
        if (file == null)
        {
            return NotFound();
        }

        return File(file.Content, file.ContentType, file.FileName);
    }
}
