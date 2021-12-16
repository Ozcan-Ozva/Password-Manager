using AutoMapper;
using InformationSecurity.Dtos;
using InformationSecurity.Models;

namespace InformationSecurity.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User,CreateUserDto>().ReverseMap();
            CreateMap<User,UserDto>();

            CreateMap<UserDto, User>()
                .ForMember(user => user.Id, opt => opt.Ignore());

            CreateMap<CreateUserPasswordDto, UserPassword>();

            CreateMap<UserPassword, UserPasswordDto>()
                .ForMember(userPasswordDto => userPasswordDto.MainUsername, opt =>
                {
                    opt.MapFrom(userPassword => userPassword.User.Username);
                });
            CreateMap<UserPasswordDto, UserPassword>()
                .ForPath(userPassword => userPassword.User.Username, opt =>
                {
                    opt.MapFrom(userPasswordDto => userPasswordDto.MainUsername);
                });
            CreateMap<UploadFileDto, UploadFile>();
            CreateMap<UploadFile, UploadFileDto>();
        }
    }
}
