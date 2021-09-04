using System;

public abstract class Model
{
    public Model(Guid id)
    {
        Id=id;
    }
    
    public Guid Id {get;set;}
}