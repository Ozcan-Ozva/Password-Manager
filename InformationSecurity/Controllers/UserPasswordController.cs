using AutoMapper;
using InformationSecurity.Data;
using InformationSecurity.Dtos;
using InformationSecurity.Filters;
using InformationSecurity.Models;
using InformationSecurity.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InformationSecurity.Controllers
{
    [ApiController]
    public class UserPasswordController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserPasswordsRepository _userPasswordsRepository;
        private readonly IUserRepository _userRepository;
        private IUnitOfWork _unitOfWork;

        public UserPasswordController(IMapper mapper, IUserPasswordsRepository userPasswordsRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userPasswordsRepository = userPasswordsRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("/userpassword")]
        public async Task<IActionResult> GetUserPasswords()
        {
            var usersPassword = await _userPasswordsRepository.GetUserPasswordsAsync();
            return Ok(usersPassword.Select(userPassword => _mapper.Map<UserPasswordDto>(userPassword)));
        }

        [HttpGet]
        [Route("/userpassword/{id}")]
        public async Task<IActionResult> GetUserPassword(Guid id)
        {
            var userPassword = await _userPasswordsRepository.GetUserPasswordAsync(id);
            if (userPassword == null)
                return BadRequest();
            return Ok(_mapper.Map<UserPasswordDto>(userPassword));
        }

        //[RequestHeaderMACFilter]
        [HttpPost]
        [Route("/userpassword")]
        public async Task<IActionResult> CreateUserPassword(EncryptedResponse encryptRequest)
        {
            if (encryptRequest.Response == null)
                return BadRequest();
            int mainUserPassword = 5524879;// this should get it with the request
            var createUserPassword = AesGcmEncryptionAlgorithm.DecryptData(encryptRequest.Response, mainUserPassword.ToString());
            UserPassword userPassword = new()
            {
                Id = Guid.NewGuid(),
                UserName = createUserPassword.ElementAt(0), //createUserPassword.UserName
                Password = (createUserPassword.ElementAt(4)), // createUserPassword.Password
                Description = createUserPassword.ElementAt(1), //createUserPassword.Description
                Address = createUserPassword.ElementAt(2), //createUserPassword.Address
                UserId = Guid.Parse(createUserPassword.ElementAt(3)), // createUserPassword.UserId
                User = await _userRepository.GetUserAsync(Guid.Parse(createUserPassword.ElementAt(3))),
            };
            // still need to convert the file to binary array
            await _userPasswordsRepository.CreateUserPasswordAsync(userPassword);
            await _unitOfWork.CompleteAsync();
            return Ok(_mapper.Map<UserPasswordDto>(userPassword));
        }

        [HMACAuthentication]
        [HttpPut]
        [Route("/userpassword/{id}")]
        public async Task<IActionResult> UpdateUserPassword(Guid id, CreateUserPasswordDto updateUserPassword)
        {
            if (updateUserPassword == null)
                return BadRequest();
            var userPasswordInDb = await _userPasswordsRepository.GetUserPasswordAsync(id);
            _mapper.Map(updateUserPassword, userPasswordInDb);
            await _unitOfWork.CompleteAsync();
            return Ok(_mapper.Map<UserPasswordDto>(userPasswordInDb));
        }
        [HttpDelete]
        [Route("/userpassword/{id}")]
        public async Task<IActionResult> DeleteUserPassword(Guid id)
        {
            await _userPasswordsRepository.DeleteUserPasswordAsync(id);
            await _unitOfWork.CompleteAsync();
            return Ok(id);
        }
    }
}
