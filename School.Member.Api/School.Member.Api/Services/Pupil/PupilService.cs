using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PupilService : IPupilService
{
    private readonly MemberDbContext dbContext;

    public PupilService(MemberDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<PupilDto> Create(CreatePupilDto createPupilDto)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IList<PupilDto>> GetList()
    {
        throw new NotImplementedException();
    }

    public Task<PupilDto> GetPupilDto(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<PupilDto> Update(Guid id, CreatePupilDto updateDto)
    {
        throw new NotImplementedException();
    }
}