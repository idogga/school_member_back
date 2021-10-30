using AutoMapper;

public class PupilProfile : Profile
{
    public PupilProfile()
    {
        AllowNullCollections = true;
        CreateMap<Pupil, PupilDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
    }
}