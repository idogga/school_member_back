using System;

public class Pupil : Model
{
    public Pupil(Guid id) : base(id)
    {
    }

    public Guid UserId {get;set;}

    public virtual User User{get;set;}

    
}