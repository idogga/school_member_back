using AutoMapper;

public class TeachersProfile : Profile
{
    public TeachersProfile()
    {
        AllowNullCollections = true;
        CreateMap<Teacher, TeacherDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
    }
}