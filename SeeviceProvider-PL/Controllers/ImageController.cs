using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImageController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload/{folder}")]
        public async Task<IActionResult> UploadImage(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var validFolders = new[] { "vendors", "products", "categories", "subcategories" };
            if (!validFolders.Contains(folder.ToLower()))
                return BadRequest("Invalid folder name.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", folder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{folder}/{fileName}";
            return Ok(new { imageUrl });
        }
    }
}
