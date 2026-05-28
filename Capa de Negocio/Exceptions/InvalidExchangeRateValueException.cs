using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_de_Negocio.Exceptions
{
    public class InvalidExchangeRateValueException(string message) : Exception(message)
    {
    }
}
