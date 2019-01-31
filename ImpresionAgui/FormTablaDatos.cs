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
using System.Net.Http;

namespace ImpresionAgui
{
    public partial class FormTablaDatos : Form
    {
        private String datosBD;
        private List<Datos> data;
        DataTable tabla;
       // private String data;

        public FormTablaDatos()
        {
            InitializeComponent();
            buscarenBD();
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form existente = new FormNuevaEtiqueta();
            existente.ShowDialog();
        }

        private HttpClient httpClient;

        public async Task buscarenBD()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://agui.myruns.com");
            //httpClient.BaseAddress = new Uri("http://localhost:800");

            /* POST: Para recibir información ENVIANDO DATOS
            //Articulo
            string fecha = txtFecha.Text;
            string albaran = txtAlbaran.Text;
            string articulo = txtArticulo.Text;

            var values = new Dictionary<string, string>
            {{"Fecha", fecha}, {"Albaran", albaran}, {"Articulo", articulo}};

            var content = new FormUrlEncodedContent(values); 
            var response = await httpClient.PostAsync("api/leer_basedatos.php", content);
            var contents = await response.Content.ReadAsStringAsync();
          //  data = contents;
            Console.WriteLine(contents);*/

            //GET: para recibir informacion SIN enviar datos

            var response = await httpClient.GetAsync("api/leer_basedatos.php");
            var contents = await response.Content.ReadAsStringAsync();

            //deserializarDatos
            data = JsonConvert.DeserializeObject<List<Datos>>(contents);
            tabla = ToDataTable(data);
            dataGridDatosBD.DataSource = tabla;
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridDatosBD.Rows[0];

            //llamar a la funcion imprimir del form nuevaetiqueta o crear un nuevo metodo
        }

        private void txtFecha_TextChanged(object sender, EventArgs e)
        {
            tabla.DefaultView.RowFilter = $"fecha LIKE '{txtFecha.Text}%'";
        }

        private void txtAlbaran_TextChanged(object sender, EventArgs e)
        {
            tabla.DefaultView.RowFilter = $"albaran LIKE '{txtAlbaran.Text}%'";
        }

        private void txtArticulo_TextChanged(object sender, EventArgs e)
        {
            tabla.DefaultView.RowFilter = $"articulo LIKE '{txtArticulo.Text}%'";
        }

        public DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

    }
}
