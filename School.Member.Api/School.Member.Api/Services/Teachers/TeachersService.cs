using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class TeachersService : ITeachersService
{
    private readonly MemberDbContext dbContext;
    private readonly IKeycloakService keycloakService;
    private readonly IMapper mapper;
    private const string GROUP_NAME = "teachers";

    public TeachersService(
        MemberDbContext dbContext,
        IKeycloakService keycloakService,
        IMapper mapper)
    {
        this.dbContext = dbContext;
        this.keycloakService = keycloakService;
        this.mapper = mapper;
    }

    public async Task<TeacherDto> Create(CreateTeacherDto createTeacherDto)
    {
        await keycloakService.CanCreateUser(createTeacherDto.Email);
        var user = await keycloakService.CreateUser(createTeacherDto);
        await keycloakService.AddUserToGroup(user.Id, GROUP_NAME);
        var teacher = new Teacher(Guid.NewGuid())
        {
            User = user,
            DateOfBorn = createTeacherDto.DateOfBorn,
            FirstName = createTeacherDto.FirstName,
            LastName = createTeacherDto.LastName,
            Synonim = createTeacherDto.Synonim,
        };
        dbContext.Teachers.Add(teacher);
        await dbContext.SaveChangesAsync();
        return mapper.Map<TeacherDto>(teacher);
    }

    public async Task Delete(Guid id)
    {
        var Teachers = await dbContext.Teachers
            .Include(p => p.User)
            .FirstOrFail(p => p.Id == id);
        await keycloakService.Delete(Teachers.User);
        dbContext.Teachers.Remove(Teachers);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IList<TeacherDto>> GetList()
    {
        var query =  dbContext.Teachers
            .AsNoTracking();
        return await mapper.ProjectTo<TeacherDto>(query).ToListAsync();
    }

    public async Task<TeacherDto> GetTeachersDto(Guid id)
    {
        var query =  dbContext.Teachers
            .AsNoTracking();
        return await mapper.ProjectTo<TeacherDto>(query).FirstOrFail(p => p.Id == id);
    }

    public Task<TeacherDto> Update(Guid id, CreateTeacherDto updateDto)
    {
        throw new NotImplementedException();
    }
}