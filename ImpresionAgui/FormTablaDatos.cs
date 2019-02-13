using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public PairData pairData;

        public FormTablaDatos()
        {
            InitializeComponent();
            pairData = ConfigurationManager.getInstance().getPairData();
            buscarenBD();
        }

        private static void salirConError(String mensaje, int errorCode)
        {
            if (mensaje != null)
            {
                Console.WriteLine(mensaje);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Pulse enter para salir");
            var exitInput = Console.ReadLine();
            Environment.Exit(errorCode);
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

            //GET: para recibir informacion SIN enviar datos
            var response = await httpClient.GetAsync("api/leer_basedatos.php");
            var contents = await response.Content.ReadAsStringAsync();

            //deserializarDatos
            data = JsonConvert.DeserializeObject<List<Datos>>(contents);
            tabla = ToDataTable(data);
            dataGridDatosBD.DataSource = tabla;
            DataGridViewColumn column = dataGridDatosBD.Columns[8];
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridDatosBD.Rows[0];

            imprimir();
            MessageBox.Show("Imprimiendo etiqueta...");
        }

        private void imprimir()
        {
            // Configurar impresora
            SatoPrinter satoPrinter = new SatoPrinter(pairData.IP, pairData.Port);

            String articulo = dataGridDatosBD.CurrentRow.Cells["Articulo"].Value.ToString();
            String cantidad = dataGridDatosBD.CurrentRow.Cells["Cantidad"].Value.ToString();
            String lote = dataGridDatosBD.CurrentRow.Cells["Lote"].Value.ToString();
            String pedido = dataGridDatosBD.CurrentRow.Cells["Pedido"].Value.ToString();
            String albaran = dataGridDatosBD.CurrentRow.Cells["Albaran"].Value.ToString();
            String control = dataGridDatosBD.CurrentRow.Cells["Control"].Value.ToString();
            String numcajas = dataGridDatosBD.CurrentRow.Cells["Ncajas"].Value.ToString();
            String epc = dataGridDatosBD.CurrentRow.Cells["EPC"].Value.ToString();

            satoPrinter.imprimir(articulo, cantidad, lote, pedido, albaran, control, numcajas, epc);
        }

        int caja;

        private void filtrar()
        {
            string fecha = $"fecha LIKE '{txtFecha.Text}%'";
            string albaran = $"albaran LIKE '{txtAlbaran.Text}%'";
            string articulo = $"articulo LIKE '{txtArticulo.Text}%'";
            tabla.DefaultView.RowFilter = fecha + " AND " + albaran + " AND " + articulo;
        }

        private void txtFecha_TextChanged(object sender, EventArgs e)
        {
            filtrar();
        }

        private void txtAlbaran_TextChanged(object sender, EventArgs e)
        {
            filtrar();
        }

        private void txtArticulo_TextChanged(object sender, EventArgs e)
        {
            filtrar();
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
