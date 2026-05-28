namespace Capa_de_Negocio.Exceptions;

public class InactiveEntityException : Exception
{
    public InactiveEntityException(string message) : base(message) { }
}
