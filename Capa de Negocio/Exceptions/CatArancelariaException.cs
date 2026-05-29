using System;

namespace Capa_de_Negocio.Exceptions;
    public class CatArancelariaException : Exception
    {
        public CatArancelariaException() : base() { }

        public CatArancelariaException(string message) : base(message) { }

        public CatArancelariaException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
