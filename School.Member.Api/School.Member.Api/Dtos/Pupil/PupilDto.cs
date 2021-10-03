using System;

public record PupilDto
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}