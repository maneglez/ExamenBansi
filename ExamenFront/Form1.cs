using ApiExamenLib;
using ApiExamenLib.WsExamenApiRef;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExamenFront
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //vincular funciones a eventos del form
            btnActualizar.Click += (o, e) => Actualizar();
            btnAgregar.Click += (o, e) => Agregar();
            btnConsultar.Click += (o, e) => Consultar();
            btnEliminar.Click += (o, e) => Eliminar();
            //Evitar generar columnas en automatico
            dgvDatos.AutoGenerateColumns = false;
            //Color arternante para destacar columnas pares
            dgvDatos.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            //Llenar comboboxes
            cbTipoConexion.Items.Add(TipoConexion.SQLStoredProcedures);
            cbTipoConexion.Items.Add(TipoConexion.WebService);
            cbTipoConexion.SelectedIndex = 0;
        }

        #region propiedades
        public int idExamen { get { 
                if (int.TryParse(tbId.Text, out int val))
                    return val;
                return 0;
            }
            set => tbId.Text = value.ToString();
        }
        public string Nombre { get => tbNombre.Text; set => tbNombre.Text = value; }
        public string Descripcion { get => tbDescripcion.Text; set => tbDescripcion.Text = value; }
        public TipoConexion TipoConn { get => (TipoConexion)cbTipoConexion.SelectedItem; set => cbTipoConexion.SelectedItem = value; }
        #endregion;

        #region Funciones
        private void Agregar()
        {
            Cursor = Cursors.WaitCursor;
            if (ClsExamen.AgregarExamen(Nombre, Descripcion, out string error, TipoConn))
            {
                LogMsg("Agregado correctamente!");
                Limpiar();
            }
            else LogMsg(error);
            Cursor = Cursors.Default;
        }

        private void LogMsg(string message)
        {
            rtbMensajes.Text += $"{DateTime.Now:T} [{TipoConn}] -> {message}{Environment.NewLine}";
        }

        private void Actualizar()
        {
            Cursor = Cursors.WaitCursor;
            if (ClsExamen.ActualizarExamen(idExamen, Nombre, Descripcion, out string error, TipoConn))
            {
                LogMsg("Actualizado correctamente! Examen " + idExamen);
                Limpiar();
            } else LogMsg(error);
            Cursor = Cursors.Default;
        }
        private void Eliminar()
        {
            Cursor = Cursors.WaitCursor;
            if (ClsExamen.EliminarExamen(idExamen, out string error, TipoConn))
            {
                LogMsg("Eliminado correctamente! Examen " + idExamen);
                Limpiar();
            }  
            else LogMsg(error);
            Cursor = Cursors.Default;
        }

        private void Consultar()
        {
            Cursor = Cursors.WaitCursor;
            var data = ClsExamen.ConsultarExamen(idExamen, Nombre, Descripcion, TipoConn);
            dgvDatos.DataSource = data;
            LogMsg($"Consulta realizada, se obtuvieron {data.Count} resultados");
            Cursor = Cursors.Default;
        }
        private void Limpiar()
        {
            tbId.Text =
            Descripcion =
            Nombre = "";
            Consultar();
        }

        #endregion

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            var examen = dgvDatos.Rows[e.RowIndex].DataBoundItem as tblExamen;
            idExamen = examen.idExamen;
            Nombre = examen.Nombre;
            Descripcion = examen.Descripcion;
        }
    }
}
