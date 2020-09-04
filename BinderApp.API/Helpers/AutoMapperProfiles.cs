using System.Linq;
using AutoMapper;
using BinderApp.API.DTOs;
using BinderApp.API.Models;

namespace BinderApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(x => x.PhotoUrl, y => 
                                        y.MapFrom(z => z.Photos
                                        .FirstOrDefault(p => p.IsMain == true).Url))
                .ForMember(x => x.Age, y => 
                                   y.MapFrom(z => z.DateOfBirth.CalculateAge()));

            CreateMap<User, UserForDetailedDto>()
                .ForMember(x => x.PhotoUrl, y => 
                                        y.MapFrom(z => z.Photos
                                        .FirstOrDefault(p => p.IsMain == true).Url))
                .ForMember(x => x.Age, y => 
                                   y.MapFrom(z => z.DateOfBirth.CalculateAge())) ;

            CreateMap<Photo, PhotosForDetailedDto>();

            CreateMap<UserForUpdateDto, User>();

            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<PhotoForCreationDto, Photo>();

            CreateMap<UserForRegisterDto, User>();

            CreateMap<MessageForCreationDto, Message>().ReverseMap();

            CreateMap<Message, MessageToReturnDto>()
            .ForMember(x => x.SenderPhotoUrl, opt => opt
                .MapFrom(u => u.Sender.Photos
                .FirstOrDefault(p => p.IsMain).Url))
            .ForMember(x => x.RecipientPhotoUrl, opt => opt
                .MapFrom(u => u.Recipient.Photos
                .FirstOrDefault(p => p.IsMain).Url));
        }
    }
}