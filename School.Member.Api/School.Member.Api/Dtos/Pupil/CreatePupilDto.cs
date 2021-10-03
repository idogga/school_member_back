public record CreatePupilDto
{
    public string Email { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}