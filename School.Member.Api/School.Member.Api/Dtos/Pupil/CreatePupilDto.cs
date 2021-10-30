using System;

public record CreatePupilDto
{
    public string Email { get; init; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public DateTime DateOfBorn {get;set;}
}