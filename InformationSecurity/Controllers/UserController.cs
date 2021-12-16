using AutoMapper;
using InformationSecurity.Data;
using InformationSecurity.Dtos;
using InformationSecurity.Filters;
using InformationSecurity.Models;
using InformationSecurity.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace InformationSecurity.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _repository;
        private readonly IUserKeyRepository _userKeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserController(IUserRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IUserKeyRepository userKeyRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userKeyRepository = userKeyRepository;
        }

        [RequestHeaderMACFilter]
        [HttpGet] // Get user/getusers
        [Route("/user")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repository.GetUsersAsync();
            List<EncryptedResponse> response = new List<EncryptedResponse>();
            foreach (var user in users)
            {
                List<string> data = new List<string>();
                data.Add(user.Username);
                data.Add(user.Password.ToString());
                response.Add(new EncryptedResponse
                {
                    Response = AesGcmEncryptionAlgorithm.EncryptData(data, user.Password.ToString()),
                });
            }
            //return Ok(users.Select(user => _mapper.Map<UserDto>(user)));
            return Ok(response);
        }
        
        [HttpGet] // Get user/{id}
        [Route("/user/{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _repository.GetUserAsync(id);
            if (user == null) return BadRequest();
            List<string> data = new List<string>();
            data.Add(user.Username);
            data.Add(user.Password.ToString());
            EncryptedResponse response = new()
            {
                Response = AesGcmEncryptionAlgorithm.EncryptData(data, user.Password.ToString()),
            };
            //return Ok(_mapper.Map<UserDto>(user));
            return Ok(response);
        }

        [HttpPost]
        //[HMACAuthentication]
        [Route("/user")]
        public async Task<IActionResult> CreateUser(CreateUserDto createUser)
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = createUser.Username,
                Password = SHA256Algorithm.ComputeSha256Hash(createUser.Password),
            };
            UserKey key = new()
            {
                Id = Guid.NewGuid(),
                PrivateKey = _userKeyRepository.Encrypt(user.Password.ToString(), user.Password.ToString()),
                PublicKey = Guid.NewGuid(),
                UserId = user.Id,
            };
            await _repository.CreateUserAsync(user);
            await _userKeyRepository.CreateUserKeyAsync(key);
            await _unitOfWork.CompleteAsync();
            var userDto = _mapper.Map<UserDto>(user) with
            {
                PrivateKey = Convert.ToBase64String(key.PrivateKey),
                PublicKey = key.PublicKey.ToString(),
            };
            return Ok(userDto);
        }

        [HttpPut]
        [Route("/user/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, CreateUserDto updateUser)
        {
            if (updateUser == null) return BadRequest();
            var userInDb = await _repository.GetUserAsync(id);
            _mapper.Map(updateUser,userInDb);
            await _unitOfWork.CompleteAsync();
            return Ok(_mapper.Map<UserDto>(userInDb));
        }

        [HttpDelete]
        [Route("/user/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _repository.DeleteUserAsync(id);
            await _unitOfWork.CompleteAsync();
            return Ok("Deleted is done");
        }
    }
}
