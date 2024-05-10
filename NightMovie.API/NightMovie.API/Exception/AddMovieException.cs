using System;

public class FilmAlreadyExist : Exception
{
    new string Message = "Le film a déjà été ajouté par vous ou un autre utilisateur";
    public FilmAlreadyExist()
    {
    }

    public FilmAlreadyExist(string message)
        : base(message)
    {
    }

    public FilmAlreadyExist(string message, Exception inner)
        : base(message, inner)
    {
    }
}