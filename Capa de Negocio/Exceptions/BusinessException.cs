namespace Capa_de_Negocio.Exceptions
{
    // Heredamos de Exception para que actúe como un error nativo de C#
    public class BusinessException : Exception
    {
        // El constructor recibe el mensaje de error personalizado y se lo pasa 
        // directamente a la clase base (Exception) mediante el operador : base(message)
        public BusinessException(string message) : base(message) { }
    }
}