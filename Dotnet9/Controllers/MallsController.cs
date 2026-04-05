using AutoMapper;
using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Models.DTO;
using Dotnet9.Repository.Irepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MallsController : ControllerBase
    {
        //private static readonly List<Mall> Malls = new()
        //{
        //    new Mall { Id = 1, Name = "Sunshine Mall", Location = "Downtown", Floors = 5 },
        //    new Mall { Id = 2, Name = "Riverfront Mall", Location = "Riverside", Floors = 3 },
        //    new Mall { Id = 3, Name = "Mountainview Mall", Location = "Hillside", Floors = 4 }
        //};

        private readonly IUnitOfWork _uow;
        private readonly ILogger<MallsController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        string[] allowedExtension = { ".jpg", ".png", ".jpeg", ".pdf", ".docx" };
        public MallsController(IUnitOfWork uow, ILogger<MallsController> logger, IWebHostEnvironment env,IMapper mapper)
        {
            _uow = uow;
            _logger = logger;
            _env = env;
            _mapper = mapper;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            //return Ok("File uploaded successfully.");
            return Ok(new { filePath, fileName });
        }

        [HttpPost("upload-multiple")]
        public async Task<ActionResult> UploadMultipleFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uploadedFiles = new List<object>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtension.Contains(ext))
                    return BadRequest($"File type '{ext}' is not allowed");

                if (file.Length > 10485760)
                    return BadRequest("file size exceeds 10MB");

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                uploadedFiles.Add(new
                {
                    OriginalName = file.FileName,
                    SavedAs = fileName,
                    FullPath = filePath,
                    Size = file.Length
                });
            }
            return Ok(new { UploadedFiles = uploadedFiles });
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }
            var contentType = "application/octet-stream";
            return PhysicalFile(filePath, contentType, fileName);
        }

        [HttpGet("download-bytes/{fileName}")]//small files only
        public async Task<IActionResult> DownloadFileAsBytes(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "application/octet-stream", fileName);
        }

        [HttpGet("files")]
        public IActionResult GetUploadedFiles()
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                return Ok(new List<string>());
            }
            var files = Directory.GetFiles(uploadsFolder)
                .Select(f => new
                {
                    FileName = Path.GetFileName(f),
                    Size = new FileInfo(f).Length,
                    UploadedAt = System.IO.File.GetCreationTime(f)
                }).ToList();
            return Ok(files);
        }

        [HttpGet]
        public async Task<ActionResult> GetMalls()
        {
            //return Ok(await _context.Malls.ToListAsync());
            _logger.LogInformation("Fetching all malls at {Time}",DateTime.Now);
            var malls = await _uow.Malls.GetAll("MallOwner,Shops,Customers,Documents");
            _logger.LogInformation("Retrived all {Count} malls", malls.Count());
            return Ok(malls);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Mall>> GetMall(int id)
        {
            //var mall = _context.Malls.FirstOrDefault(m => m.Id == id);
            _logger.LogDebug("GetMallbyId = {@id}",id);
            var mall = await _uow.Malls.GetById(id);

            if (mall == null)
            {
                _logger.LogError("Mall with id = {id} is not in DB", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved mall name = {name}",mall.Name);
            return Ok(mall);
            //var mall = await _context.Malls.FindAsync(id);
            //return mall is null ? NotFound() : Ok(mall);
            //var mall = await _uow.Malls.GetById(id);
            //return mall is null ? NotFound() : Ok(mall);
        }
        [HttpPost]
        public async Task<ActionResult> CreateMall([FromForm] MallDto dto)
        {
            var mall = _mapper.Map<Mall>(dto);
            await _uow.Malls.Add(mall);
            await _uow.Save();

            if(dto.Documents != null && dto.Documents.Count > 0)
            {
                var savedDocuments = await SavedFilesToDisk(dto.Documents,mall.Id);
                mall.Documents ??= new List<MallDocument>();
                mall.Documents!.AddRange(savedDocuments);
                _uow.Malls.Update(mall);
                await _uow.Save();
            }
            return Ok(mall);
        }

        private async Task<List<MallDocument>> SavedFilesToDisk(List<IFormFile> files,int mallId)
        {
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var savedDocuments = new List<MallDocument>();
            
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtension.Contains(ext))
                    continue;

                if (file.Length > 10485760)
                    continue;
                
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                savedDocuments.Add(new MallDocument
                {
                    MallId = mallId,
                    FileName = file.FileName,
                    SavedFileName = fileName,
                    FilePath = filePath,
                    FileSizeBytes = file.Length,
                    ContentType = file.ContentType,
                    UploadedAt = DateTime.Now,
                });
            }
            return savedDocuments;
        }

        //public async Task<ActionResult<Mall>> CreateMall(Mall mall)
        //{
        //    //if (!ModelState.IsValid)
        //    //{
        //    //    return BadRequest();
        //    //}
        //    //Console.WriteLine($"Entity state start: {_context.Entry(mall).State}");
        //    //_context.Malls.Add(mall);
        //    //await _context.SaveChangesAsync();
        //    //return Ok("Mall created succesfully");

        //    using var scope = _logger.BeginScope(new Dictionary<string, object>
        //    {
        //        ["Operation"] = "CreateMall",
        //        ["MallName"] = mall.Name,
        //        ["RequestedBy"] = HttpContext.User.Identity?.Name ?? "anonymous"
        //    });

        //    await _uow.Malls.Add(mall);
        //    await _uow.Save();
        //    return Ok("Mall created succesfully");
        //}

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateMall(int id,[FromForm] MallDto dto)
        {
            var mall = await _uow.Malls.GetById(id);
            if (mall == null) return NotFound();

            _mapper.Map(dto, mall);
            if(dto.Documents != null && dto.Documents.Any())
            {
                var savedDocuments = await SavedFilesToDisk(dto.Documents, mall.Id);
                mall.Documents ??= new List<MallDocument>();
                mall.Documents.AddRange(savedDocuments);
            }
            _uow.Malls.Update(mall);
            await _uow.Save();
            return NoContent();
        }

        //public async Task<ActionResult> UpdateMall(int id, Mall updatedMall)
        //{
        //    //var mall = await _context.Malls.FindAsync(id);
        //    //if (mall == null)
        //    //{
        //    //    return NotFound();
        //    //}
        //    //mall.Name = updatedMall.Name;
        //    //mall.Location = updatedMall.Location;
        //    //mall.Floors = updatedMall.Floors;
        //    //await _context.SaveChangesAsync();
        //    //return NoContent();

        //    var mall = await _uow.Malls.GetById(id);
        //    if (mall == null)
        //    {
        //        return NotFound();
        //    }
        //    mall.Name = updatedMall.Name;
        //    mall.Location = updatedMall.Location;
        //    mall.Floors = updatedMall.Floors;
        //    _uow.Malls.Update(mall);
        //    await _uow.Save();
        //    return NoContent();
        //}

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteFile(int id)
        {
            var mall = await _uow.Malls.GetById(id);
            if (mall == null) return NotFound();

            if(mall.Documents!= null)
            {
                foreach(var file in mall.Documents)
                {
                    var path = Path.Combine(_env.ContentRootPath, "Uploads", Path.GetFileName(file.FileName));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
            }

            _uow.Malls.Delete(mall);
            await _uow.Save();
            return NoContent();
        }
        //public async Task<ActionResult> DeleteMall(int id)
        //{
        //    //var mall = await _context.Malls.FirstOrDefaultAsync(m => m.Id == id);
        //    //if (mall == null)
        //    //{
        //    //    return NotFound();
        //    //}
        //    //_context.Malls.Remove(mall);
        //    //await _context.SaveChangesAsync();
        //    //return NoContent();
        //    _logger.LogWarning("DeleteMallbyId = {@id}", id);

        //    var mall = await _uow.Malls.GetById(id);
        //    if (mall == null)
        //    {
        //        _logger.LogError("Mall with id = {id} is not in DB", id);
        //        return NotFound();
        //    }
        //    _uow.Malls.Delete(mall);
        //    await _uow.Save();
        //    _logger.LogCritical("AUDIT: Mall id = {id} deleted by user = {user} at {time}",
        //        id,HttpContext.User.Identity?.Name ?? "anonymous",DateTime.Now);
        //    return NoContent();
        //}
    }
}
