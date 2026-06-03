using System;

namespace Capa_de_Negocio.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
        public BusinessException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class NotFoundException : BusinessException
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationException : BusinessException
    {
        public ValidationException(string message) : base(message) { }
    }

    public class ConflictException : BusinessException
    {
        public ConflictException(string message) : base(message) { }
    }
}
