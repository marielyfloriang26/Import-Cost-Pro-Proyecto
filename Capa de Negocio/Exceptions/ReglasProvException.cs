using System;

namespace Capa_de_Negocio.Exceptions 
{
    public class ReglasProvException : Exception
    {
        public ReglasProvException(string mensaje) : base(mensaje)
        {
        }
    }
}