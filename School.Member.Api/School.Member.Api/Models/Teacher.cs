
using System;

public class Teacher : Model
{
    public Teacher(Guid id) : base(id)
    {
    }

    public Guid UserId {get;set;}

    public virtual User User { get; set; } = null!;

    public string Synonim { get; set; } = null!;
}