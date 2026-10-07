using CustomerServiceCampaign.Api.Services.Purchases;
using Microsoft.AspNetCore.Mvc;

namespace CustomerServiceCampaign.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController(
    IPurchaseImportService purchaseImportService) : ControllerBase
{
    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest(new
            {
                error = "CSV file is empty."
            });
        }

        try
        {
            await using var stream = file.OpenReadStream();

            var importedCount =
                await purchaseImportService.ImportAsync(stream);

            return Ok(new
            {
                importedCount
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                error = exception.Message
            });
        }
    }
}