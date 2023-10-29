using ApiExamenLib.WsExamenApiRef;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace ApiExamenLib
{
    public static class ClsExamen
    {
        private static TipoConexion tipoConexionDefault = TipoConexion.SQLStoredProcedures;

        /// <summary>
        /// Indica cual será el tipo de conexión por defecto
        /// </summary>
        public static TipoConexion TipoConexionDefault { get => tipoConexionDefault; set => 
                tipoConexionDefault = value == TipoConexion.Default ? //Evitar que se coloque este valor para evitar un stack overflow
                throw new Exception("Valor no válido para esta propiedad") 
                : value; }
        /// <summary>
        /// Agrega un examen
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="descripcion"></param>
        /// <param name="errorMsg"></param>
        /// <param name="tipoCon"></param>
        /// <returns></returns>
        public static bool AgregarExamen(string nombre,string descripcion,out string errorMsg,TipoConexion tipoCon = TipoConexion.Default)
        {
            var retVal = false;
            errorMsg = "ok";
            try
            {
                switch (tipoCon)
                {
                    case TipoConexion.Default:
                        retVal = AgregarExamen(nombre, descripcion, out errorMsg, TipoConexionDefault);
                        break;
                    case TipoConexion.WebService:
                        using (var ws = new WsExamen())
                        {
                            retVal = ws.AgregarExamen(nombre, descripcion, out errorMsg);
                        }
                        break;
                    case TipoConexion.SQLStoredProcedures:
                        retVal = ValidarResultado(EjecutarConsulta("exec spAgregar @p0,@p1", nombre, descripcion),
                            out errorMsg);
                        break;
                    default:
                        break;
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
        /// Elimina un examen específico
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorMsg"></param>
        /// <param name="tipoCon"></param>
        /// <returns></returns>
        public static bool EliminarExamen(int id, out string errorMsg, TipoConexion tipoCon = TipoConexion.Default)
        {
            var retVal = false;
            errorMsg = "ok";
            try
            {
                switch (tipoCon)
                {
                    case TipoConexion.Default:
                        retVal = EliminarExamen(id, out errorMsg, TipoConexionDefault);
                        break;
                    case TipoConexion.WebService:
                        using (var ws = new WsExamen())
                        {
                            retVal = ws.EliminarExamen(id, out errorMsg);
                        }
                        break;
                    case TipoConexion.SQLStoredProcedures:
                        retVal = ValidarResultado(EjecutarConsulta("exec spEliminar @p0", id),
                            out errorMsg);
                        break;
                    default:
                        break;
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
        /// Actualiza la información del examen
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nombre"></param>
        /// <param name="descripcion"></param>
        /// <param name="errorMsg"></param>
        /// <param name="tipoCon"></param>
        /// <returns></returns>
        public static bool ActualizarExamen(int id,string nombre, string descripcion, out string errorMsg, TipoConexion tipoCon = TipoConexion.Default)
        {
            var retVal = false;
            errorMsg = "ok";
            try
            {
                switch (tipoCon)
                {
                    case TipoConexion.Default:
                        retVal = ActualizarExamen(id, nombre, descripcion, out errorMsg, TipoConexionDefault);
                        break;
                    case TipoConexion.WebService:
                        using (var ws = new WsExamen())
                        {
                            retVal = ws.ActualizarExamen(id, nombre, descripcion, out errorMsg);
                        }
                        break;
                    case TipoConexion.SQLStoredProcedures:
                        retVal = ValidarResultado(EjecutarConsulta("exec spActualizar @p0,@p1,@p2", id, nombre, descripcion),
                            out errorMsg);
                        break;
                    default:
                        break;
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
        /// Consulta examens
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nombre"></param>
        /// <param name="descripcion"></param>
        /// <param name="tipoCon"></param>
        /// <returns>Devuelve una lista de examenes que coinciden con los criterios de búsqueda</returns>
        public static List<tblExamen> ConsultarExamen(int id, string nombre, string descripcion, TipoConexion tipoCon = TipoConexion.Default)
        {
            var examenes = new List<tblExamen>();
            try
            {
                switch (tipoCon)
                {
                    case TipoConexion.Default:
                        return ConsultarExamen(id, nombre, descripcion, TipoConexionDefault);
                    case TipoConexion.WebService:
                        using (var ws = new WsExamen())
                        {
                            examenes.AddRange(ws.ConsultarExamen(id, nombre, descripcion));
                        }
                        break;
                    case TipoConexion.SQLStoredProcedures:
                        examenes = EjecutarConsulta("exec spConsultar @p0,@p1,@p2", id, nombre, descripcion)
                            .ToExamenCollection();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
               
            }
            return examenes;
        }
       
        #region helpers
        /// <summary>
        /// Valida el resultado de los SP para el crud de examenes
        /// </summary>
        /// <param name="dt">tabla de resultado, debe de contener las Columnas: Estado(int),Mensaje(string)</param>
        /// <param name="errorMsg">indica el mensaje de resultado</param>
        /// <returns>Verdadero si el resultado es favorable, de lo contrario falso</returns>
        private static bool ValidarResultado(DataTable dt,out string errorMsg)
        {
            errorMsg = "ok";
            var retVal = true;
            try
            {
                if (dt.Rows.Count == 0)
                    throw new Exception("La tabla de resultado contiene 0 registros");
                var value = dt.Rows[0]["Estatus"];
                var estatus = value == DBNull.Value ? 0 : (int)value;
                value = dt.Rows[0]["Mensaje"];
                errorMsg = value == DBNull.Value ? $"Estatus: {estatus}" : $"Estatus: {estatus} {(string)value}";
                retVal = estatus == 0;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                retVal = false;
            }
            return retVal;
        }
        /// <summary>
        /// Ejecuta una consulta a base de datos
        /// </summary>
        /// <param name="consulta">consulta</param>
        /// <param name="parametros">parametros</param>
        /// <returns>Devuelve un data table con el resultado de la consulta</returns>
        private static DataTable EjecutarConsulta(string consulta, params object[] parametros)
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(Properties.Settings.Default.Conexion))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = consulta;
                    for (int i = 0; i < parametros.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@p{i}", parametros[i]);
                    }
                    using (var adapter = new SqlDataAdapter(cmd))
                        adapter.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// Convierte una tabla a una coleccion de objetos
        /// </summary>
        /// <returns>Coleccion de examenes</returns>
        private static List<tblExamen> ToExamenCollection(this DataTable dt)
        {
            var lst = new List<tblExamen>();
            if (dt.Rows.Count == 0)
                return lst;
            foreach (DataRow r in dt.Rows)
            {
                var examen = new tblExamen();
                examen.idExamen = (int)r[nameof(examen.idExamen)];
                examen.Nombre = r[nameof(examen.Nombre)] == DBNull.Value ? "" : (string)r[nameof(examen.Nombre)];
                examen.Descripcion = r[nameof(examen.Descripcion)] == DBNull.Value ? "" : (string)r[nameof(examen.Descripcion)];
                lst.Add(examen);
            }
            return lst;
        }

        #endregion
    }
}
