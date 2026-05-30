namespace Capa_de_Negocio.Exceptions;

public class InvalidOrderStatusTransitionException : Exception
{
    public InvalidOrderStatusTransitionException(string message) : base(message) { }
}
