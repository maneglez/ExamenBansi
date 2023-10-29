using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiExamenLib
{
    /// <summary>
    /// Indica el tipo de conexion para recuperar datos de la BD
    /// </summary>
    public enum TipoConexion
    {
        /// <summary>
        /// Utilizando el tipo por defecto
        /// </summary>
        Default,
        /// <summary>
        /// Utilizando un web service
        /// </summary>
        WebService,
        /// <summary>
        /// Utilizando consultas a BD
        /// </summary>
        SQLStoredProcedures
    }
}
