using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using WsApiexamen.db;

namespace WsApiexamen
{
    /// <summary>
    /// Descripción breve de WsExamen
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WsExamen : System.Web.Services.WebService
    {
        /// <summary>
        /// Agrega un examen a la tabla tblExamen
        /// </summary>
        /// <param name="nombre">Nombre del examen max 255 caracteres</param>
        /// <param name="descripcion">Descripción max 255 caracteres</param>
        /// <param name="errorMsg">Mensaje de error</param>
        /// <returns>Retorna verdadero si se insertó correctamente, falso si hubo algún error</returns>
        [WebMethod]
        public bool AgregarExamen(string nombre, string descripcion, out string errorMsg)
        {
            //Mensaje de error
            errorMsg = "ok";
            //Valor de retorno
            var retVal = true;
            try
            {
                var examen = new tblExamen();
                examen.Nombre = nombre;
                examen.Descripcion = descripcion;
                if (!examen.EsValido(out string error))
                    throw new Exception(error);
                using (var db = new BdiExamenEntities())
                {
                    db.tblExamen.Add(examen);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Actualiza la información del examen con el id proporcionado
        /// </summary>
        /// <param name="id">Identificador del examen</param>
        /// <param name="nombre">Nombre</param>
        /// <param name="descripcion">Descripción</param>
        /// <param name="errorMsg">Error de actualizacion</param>
        /// <returns>Verdadero si todo OK, falso si hubo error</returns>
        [WebMethod]
        public bool ActualizarExamen(int id, string nombre, string descripcion, out string errorMsg)
        {
            var retVal = true;
            errorMsg = "ok";
            BdiExamenEntities db = null;
            try
            {
                db = new BdiExamenEntities();
                var examen = db.tblExamen.Find(id) ?? throw new Exception($"El examen con el id {id} no existe");
                examen.Nombre = nombre;
                examen.Descripcion = descripcion;
                if (!examen.EsValido(out string error))
                    throw new Exception(error);
                db.Entry(examen).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                retVal = false;
            }
            db?.Dispose();
            return retVal;
        }

        /// <summary>
        /// Elimina un examen dado
        /// </summary>
        /// <param name="id">ID del examen a eliminar</param>
        /// <param name="errorMsg">Error de validacion</param>
        /// <returns>Verdadero si todo OK, falso si hubo error</returns>
        [WebMethod]
        public bool EliminarExamen(int id,out string errorMsg)
        {
            var retVal = true;
            errorMsg = "ok";
            BdiExamenEntities db = null;
            try
            {
                db = new BdiExamenEntities();
                var examen = db.tblExamen.Find(id) ?? throw new Exception($"No se encontró el examen con el id {id}");
                db.tblExamen.Remove(examen);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                retVal = false;
            }
            db?.Dispose();
            return retVal;
        }


        /// <summary>
        /// Consulta todos los examenes coincidentes
        /// </summary>
        /// <param name="nombre">Nombre</param>
        /// <param name="descripcion">descripcion</param>
        /// <returns>Devuelve una lista de examenes que coinciden con el nombre o la descripción</returns>
        [WebMethod]
        public List<tblExamen> ConsultarExamen(int id,string nombre,string descripcion)
        {
            List<tblExamen> examenes;
            using (var db = new BdiExamenEntities())
            {
                var query = "select * from tblExamen ";
                if (id > 0 || !string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(descripcion))
                {//Requiere filtrar
                    query += @"where (@p0 > 0 and idExamen like concat('%', @p0, '%'))
or (len(@p1) > 0 and  Nombre like '%' + @p1 + '%')
or (len(@p2) > 0 and Descripcion like '%' + @p2 + '%' )";
                    examenes = db.tblExamen.SqlQuery(query,
                        id, nombre, descripcion).ToList();
                }
                else
                    examenes = db.tblExamen.SqlQuery(query).ToList();
            }
            return examenes.ToList();
        }
    }
}
