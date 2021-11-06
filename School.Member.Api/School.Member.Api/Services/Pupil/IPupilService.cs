using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPupilService
{
    Task<PupilDto> Create(CreatePupilDto createPupilDto);

    Task<PupilDto> GetPupilDto(Guid id);

    Task<PupilDto> Update(Guid id, CreatePupilDto updateDto);

    Task Delete(Guid id);

    Task<IList<PupilDto>> GetList();
}