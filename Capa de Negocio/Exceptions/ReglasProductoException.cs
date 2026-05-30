using System;

namespace Capa_de_Negocio.Exceptions 
{
    public class ReglasProductoException : Exception
    {
        public ReglasProductoException(string mensaje) : base(mensaje)
        {
        }
    }
}