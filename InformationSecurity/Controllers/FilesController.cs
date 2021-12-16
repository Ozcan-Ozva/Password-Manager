using AutoMapper;
using InformationSecurity.Dtos;
using InformationSecurity.Models;
using InformationSecurity.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace InformationSecurity.Controllers
{
    [ApiController]
    public class FilesController : Controller
    {
        private readonly IWebHostEnvironment _host;
        private readonly IUserPasswordsRepository _userPasswordsRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadFileRepository _uploadFileRepository;
        private readonly UploadFileSettings uploadFileSettings;

        public FilesController(IWebHostEnvironment host, IUserPasswordsRepository userPasswordsRepository,
            IMapper mapper, IUnitOfWork unitOfWork, IUploadFileRepository uploadFileRepository,
            IOptionsSnapshot<UploadFileSettings> options )
        {
            this.uploadFileSettings = options.Value;
            _host = host;
            _userPasswordsRepository = userPasswordsRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _uploadFileRepository = uploadFileRepository;
        }

        [HttpPost]
        [Route("/userPassword/{userPasswordId}/files")]
        public async Task<IActionResult> UploadPdfFile(Guid userPasswordId, IFormFile file)
        {
            var userPasswordBeforeAddFile = await _userPasswordsRepository.GetUserPasswordAsync(userPasswordId);
            if (userPasswordBeforeAddFile == null)
                return NotFound();
            if (file == null)
                return BadRequest("Null file");
            if (file.Length == 0)
                return BadRequest("file is empty");
            if (file.Length > uploadFileSettings.MaxBytes)
                return BadRequest("Max size file is excceds");
            if (!uploadFileSettings.isSupported(file.FileName))
                return BadRequest("Invalid File type");
            var uploadsFolderPath = Path.Combine(_host.WebRootPath, "uploads");
            if(!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            UploadFile uploadFile = new()
            {
                Id = Guid.NewGuid(),
                Name = fileName,
                UserPasswordId = userPasswordId,
            };
            Collection<UploadFile> collection = new Collection<UploadFile>();
            if(userPasswordBeforeAddFile.UploadFiles != null)
                collection = userPasswordBeforeAddFile.UploadFiles;
            collection.Add(uploadFile);
            UserPassword userPasswordAfterAddFile = userPasswordBeforeAddFile with
            {
                UploadFiles = collection
            };
            _mapper.Map(userPasswordAfterAddFile,userPasswordBeforeAddFile);
            await _uploadFileRepository.CreateUploadFileAsync(uploadFile);
            await _unitOfWork.CompleteAsync();
            return Ok(_mapper.Map<UploadFileDto>(uploadFile));
        }

        [HttpGet]
        [Route("/userPassword/files/{userId}")]
        public async Task<IActionResult> GetUploadedFilesByUserId(Guid userId)
        {
            var files = await _uploadFileRepository.GetUploadFilesByUserIdAsync(userId);
            var clientFiles = files.Select(file => _mapper.Map<UploadFileDto>(file));
            Collection<UploadFileDto> collection = new Collection<UploadFileDto>();
            foreach (var file in clientFiles)
            {
                UploadFileDto fileAfterUrl = file with
                {
                    FileUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/uploads/{file.Name}"
                };
                collection.Add(fileAfterUrl);
            }
            return Ok(collection);
        }
    }
}
