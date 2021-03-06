using System;

public class Pupil : Model
{
    public Pupil(Guid id) : base(id)
    {
    }

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime DateOfBorn { get; set; }
}