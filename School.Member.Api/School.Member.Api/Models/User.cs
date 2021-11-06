using System;

public class User
{
    public User(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
}