using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class PupilController : Controller
{
    private readonly IPupilService pupilService;

    public PupilController(IPupilService pupilService)
    {
        this.pupilService = pupilService;
    }

    [HttpPost]
    public Task<PupilDto> Create([FromBody] CreatePupilDto createPupilDto)
    {
        return pupilService.Create(createPupilDto);
    }

    [HttpGet("{id}")]
    public Task<PupilDto> GetPupilDto(Guid id)
    {
        return pupilService.GetPupilDto(id);
    }

    [HttpPut("{id}")]
    public Task<PupilDto> Update(Guid id, [FromBody] CreatePupilDto updateDto)
    {
        return pupilService.Update(id, updateDto);
    }

    [HttpDelete("{id}")]
    public Task Delete(Guid id)
    {
        return pupilService.Delete(id);
    }

    [HttpGet]
    public Task<IList<PupilDto>> GetList()
    {
        return pupilService.GetList();
    }
}