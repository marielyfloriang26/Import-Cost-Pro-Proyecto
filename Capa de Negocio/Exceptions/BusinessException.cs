using System;

namespace Capa_de_Negocio.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { } // mensaje
    }
}
