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
using SATOPrinterAPI;
using System.Resources;
using System.IO;


namespace ImpresionAgui
{
    public partial class FormTablaDatos : Form
    {
        private String datosBD;
        private List<Datos> data;
        DataTable tabla;
        // private String data;

        private static readonly int ERROR_CODE_PARAMS = -1;
        private static readonly int ERROR_CODE_EPC_NOT_HEX = -2;
        private static readonly int ERROR_CODE_EPC_NOT_24_CHARS = -3;
        private static readonly int ERROR_CODE_ERROR_DESCONOCIDO = -4;

        public FormTablaDatos()
        {
            InitializeComponent();
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
            DataGridViewColumn column = dataGridDatosBD.Columns[8];
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridDatosBD.Rows[0];

            //llamar a la funcion imprimir del form nuevaetiqueta o crear un nuevo metodo
            //FormNuevaEtiqueta etiqueta = new FormNuevaEtiqueta();
            //etiqueta.imprimir();
            //imprimir();
            MessageBox.Show("Imprimiendo etiqueta...");
        }

        private void imprimir()
        {
            // Configurar impresora
            Printer SATOPrinter = new Printer();
            SATOPrinter.Interface = Printer.InterfaceType.TCPIP;
            SATOPrinter.TCPIPAddress = "192.168.1.52";
            SATOPrinter.TCPIPPort = "9100";

            // Generar comando de impresión
            //String PrintCommand = getCommandoImpresion(opts.cantidad, opts.epc, opts.linea1, opts.linea2, opts.linea3, opts.qr, opts.barCode);

            String PrintCommand = getCommandoImpresion();
            // Cambiar los caracteres de escape
            PrintCommand = PrintCommand.Replace("<STX>", ((char)02).ToString());
            PrintCommand = PrintCommand.Replace("<ETX>", ((char)03).ToString());
            PrintCommand = PrintCommand.Replace("<ESC>", ((char)27).ToString());

            // Convertir a bytes
            byte[] cmddata = Utils.StringToByteArray(PrintCommand);

            // Enviar comando a la impresora
            try
            {
                SATOPrinter.Send(cmddata);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                salirConError("Se ha producido un error desconocido al enviar el comando a la impresora. Compruebe la dirección IP y si la impresora está correctamente conectada.", ERROR_CODE_ERROR_DESCONOCIDO);
            }
        }

        int caja;

        private String getCommandoImpresion()
        {
            // Inicio del comando
            String comando = "<STX><ESC>A";

            //Darkness
            //comando += "<ESC>#E3A";

             //DataGridViewRow selectedRow = dataGridDatosBD.Rows[0];

            String articulo = dataGridDatosBD.CurrentRow.Cells["Articulo"].Value.ToString();
            String cantidad = dataGridDatosBD.CurrentRow.Cells["Cantidad"].Value.ToString();
            String lote = dataGridDatosBD.CurrentRow.Cells["Lote"].Value.ToString();
            String pedido = dataGridDatosBD.CurrentRow.Cells["Pedido"].Value.ToString();
            String albaran = dataGridDatosBD.CurrentRow.Cells["Albaran"].Value.ToString();
            String control = dataGridDatosBD.CurrentRow.Cells["Control"].Value.ToString();
            String numcajas = dataGridDatosBD.CurrentRow.Cells["Ncajas"].Value.ToString();
            String epc = dataGridDatosBD.CurrentRow.Cells["EPC"].Value.ToString();
            //String destino = tablaDatos.CurrentRow.Cells["Destino"].Value.ToString();

            Console.Write(comando);
            // Definir el EPC a escribir
            comando += "<ESC>IP0e:z,d:" + epc + ";";

            //Articulo y su barCode
            comando += comando += "<ESC>V05<ESC>H20";
            comando += "<ESC>B103100*" + articulo + "*";
            comando += "<ESC>V120<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + articulo;

            //Cantidad y su barCode
            comando += "<ESC>V170<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,025,025," + "CANT. " + cantidad;
            comando += comando += "<ESC>V155<ESC>H250";
            comando += "<ESC>B103040*" + cantidad + "*";

            //Albaran 
            comando += "<ESC>V115<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "ALBARAN " + albaran;

            //Pedido
            comando += "<ESC>V140<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "PEDIDO " + pedido;

            //Numero de cajas y caja actual
            //int numeroTotalCajas = Int32.Parse(numcajas);

            //comando += "<ESC>V20<ESC>H500<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + caja + "/" + numcajas;

            //if (caja != numeroTotalCajas)
            //{
            //    caja++;
            //}

            //Control
            comando += "<ESC>V90<ESC>H540<ESC>P4<ESC>L0101<ESC>RDB00,025,025," + "CONTROL ";
            comando += "<ESC>V120<ESC>H560<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + control;

            //Lote, numero y barcode (faltan el codigo y barcode, pero hay que darles la vuelta)
            comando += "<ESC>%1<ESC>V140<ESC>H690<ESC>P4<ESC>L0101<ESC>RDB00,030,030," + "LOTE ";
            comando += "<ESC>%1<ESC>V140<ESC>H730<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + lote;
            comando += comando += "<ESC>V190<ESC>H760";
            comando += "<ESC>B103040*" + lote + "*";

            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            string FileName = string.Format("{0}Resources\\agui_negro.png", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));

            String[] path = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames(); ;

            //Graphic prueba
            comando += "<ESC>V10<ESC>H540<ESC>PGh0AH<ESC>GH006006";
            comando += Utils.ConvertGraphicToSBPL(FileName);
            //comando += Utils.ConvertGraphicToSBPL(open.FileName);

            // Cantidad de etiquetas a imprimir
            comando += "<ESC>Q" + numcajas;

            // Fin del comando
            comando += "<ESC>Z<ETX>";

            return comando;
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
