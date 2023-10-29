using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsApiexamen.db
{
    public static class DbModelsExtensions
    {
        public static bool EsValido(this tblExamen examen, out string errorMsg)
        {
            errorMsg = "ok";
            var retVal = true;

            try
            {
                if (string.IsNullOrWhiteSpace(examen.Nombre))
                    throw new Exception($"El nombre no puede estar vacío");

                if (string.IsNullOrWhiteSpace(examen.Descripcion))
                    throw new Exception("La descripción no puede estar vacía");

                if (examen.Nombre.Length > 255)
                    throw new Exception("La cantidad de caracteres del nombre no puede ser mayor a 255");

                if (examen.Descripcion.Length > 255)
                    throw new Exception("La cantidad de caracteres de la descripción no puede ser mayor a 255");
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                retVal = false;
            }
            return retVal;
        }
    }
}