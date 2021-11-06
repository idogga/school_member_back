using System;

public class NotFoundException : ApplicationException
{
    public NotFoundException (Type entityType)
        : base ($"{entityType.Name} not found!")
    {
    }
}