using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class PupilService : IPupilService
{
    private readonly MemberDbContext dbContext;
    private readonly IKeycloakService keycloakService;
    private readonly IMapper mapper;
    private const string GROUP_NAME = "pupils";

    public PupilService(
        MemberDbContext dbContext,
        IKeycloakService keycloakService,
        IMapper mapper)
    {
        this.dbContext = dbContext;
        this.keycloakService = keycloakService;
        this.mapper = mapper;
    }

    public async Task<PupilDto> Create(CreatePupilDto createPupilDto)
    {
        await keycloakService.CanCreateUser(createPupilDto.Email);
        var user = await keycloakService.CreateUser(createPupilDto);
        await keycloakService.AddUserToGroup(user.Id, GROUP_NAME);
        var pupil = new Pupil(Guid.NewGuid())
        {
            User = user,
            DateOfBorn = createPupilDto.DateOfBorn
        };
        dbContext.Pupils.Add(pupil);
        await dbContext.SaveChangesAsync();
        return mapper.Map<PupilDto>(pupil);
    }

    public async Task Delete(Guid id)
    {
        var pupil = await dbContext.Pupils
            .Include(p => p.User)
            .FirstOrFail(p => p.Id == id);
        await keycloakService.Delete(pupil.User);
        dbContext.Pupils.Remove(pupil);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IList<PupilDto>> GetList()
    {
        var query =  dbContext.Pupils
            .AsNoTracking();
        return await mapper.ProjectTo<PupilDto>(query).ToListAsync();
    }

    public async Task<PupilDto> GetPupilDto(Guid id)
    {
        var query =  dbContext.Pupils
            .AsNoTracking();
        return await mapper.ProjectTo<PupilDto>(query).FirstOrFail(p => p.Id == id);
    }

    public Task<PupilDto> Update(Guid id, CreatePupilDto updateDto)
    {
        throw new NotImplementedException();
    }
}