using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class TeachersController : Controller
{
    private readonly ITeachersService teachersService;

    public TeachersController(ITeachersService teachersService)
    {
        this.teachersService = teachersService;
    }

    [HttpPost]
    public Task<TeacherDto> Create([FromBody] CreateTeacherDto createTeachersDto)
    {
        return teachersService.Create(createTeachersDto);
    }

    [HttpGet("{id}")]
    public Task<TeacherDto> GetTeachersDto(Guid id)
    {
        return teachersService.GetTeachersDto(id);
    }

    [HttpPut("{id}")]
    public Task<TeacherDto> Update(Guid id, [FromBody] CreateTeacherDto updateDto)
    {
        return teachersService.Update(id, updateDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(Guid id)
    {
        return teachersService.Delete(id);
    }

    [HttpGet]
    public Task<IList<TeacherDto>> GetList()
    {
        return teachersService.GetList();
    }
}