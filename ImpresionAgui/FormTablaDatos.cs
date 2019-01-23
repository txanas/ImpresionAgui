using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ImpresionAgui
{
    public partial class FormTablaDatos : Form
    {
        private String datosBD;

        public FormTablaDatos()
        {
            InitializeComponent();
        }
        public FormTablaDatos(String datosRecibidos)
        {
            InitializeComponent();
            this.datosBD = datosRecibidos;
            deserializarDatos();
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form existente = new FormExistente();
            existente.ShowDialog();
        }

        private void deserializarDatos()
        {
            List<Datos> data = JsonConvert.DeserializeObject<List<Datos>>(this.datosBD);
            dataGridDatosBD.DataSource = data;
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridDatosBD.Rows[0];

            //llamar a la funcion imprimir del form nuevaetiqueta o crear un nuevo metodo
        }
    }
}
