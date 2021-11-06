using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ITeachersService
{
    Task<TeacherDto> Create(CreateTeacherDto createTeachersDto);

    Task<TeacherDto> GetTeachersDto(Guid id);

    Task<TeacherDto> Update(Guid id, CreateTeacherDto updateDto);

    Task Delete(Guid id);

    Task<IList<TeacherDto>> GetList();
}