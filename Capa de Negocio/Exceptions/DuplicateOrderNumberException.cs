namespace Capa_de_Negocio.Exceptions;

public class DuplicateOrderNumberException : Exception
{
    public DuplicateOrderNumberException(string message) : base(message) { }
}
