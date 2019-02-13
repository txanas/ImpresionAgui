using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;

namespace ImpresionAgui
{
    public partial class FormNuevaEtiqueta : Form
    {

        public PairData pairData;
        
        private HttpClient httpClient;

        private String EPC;
        string[,] ListaEPC = new string[20,20];
        int numTotalCajas;


        public FormNuevaEtiqueta()
        {
            InitializeComponent();
            pairData = ConfigurationManager.getInstance().getPairData();
        }
    

        private static async Task checkResponseForError(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new NetworkException(response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }

        private async void btn_imprimir_Click(object sender, EventArgs e)
        {
            //enviar los datos al servidor
            await enviarDatos();

            //imprimir etiqueta con su epc
            imprimir();

            MessageBox.Show("Imprimiendo etiqueta...");

        }

        Datos dato = new Datos();

        public async Task enviarDatos()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://agui.myruns.com");

            for (int i = 0; i < tablaDatos.RowCount - 1; i++)
            {
                //Articulo
                dato.ARTICULO = tablaDatos.Rows[i].Cells["Articulo"].Value.ToString();
                Console.WriteLine("Articulo: " + dato.ARTICULO + " ");
                //Cantidad
                dato.CANTIDAD = tablaDatos.Rows[i].Cells["Cantidad"].Value.ToString();
                Console.WriteLine("Cantidad: " + dato.CANTIDAD + " ");
                //Lote
                dato.LOTE = tablaDatos.Rows[i].Cells["Lote"].Value.ToString();
                Console.WriteLine("Lote: " + dato.LOTE + " ");
                //Pedido
                dato.PEDIDO = tablaDatos.Rows[i].Cells["Pedido"].Value.ToString();
                Console.WriteLine("Pedido: " + dato.PEDIDO + " ");
                //Albaran
                dato.ALBARAN = tablaDatos.Rows[i].Cells["Albaran"].Value.ToString();
                Console.WriteLine("Albaran: " + dato.ALBARAN + " ");
                //Control
                dato.CONTROL = tablaDatos.Rows[i].Cells["Control"].Value.ToString();
                Console.WriteLine("Control: " + dato.CONTROL + " ");
                //Numero cajas
                dato.NCAJAS = tablaDatos.Rows[i].Cells["Ncajas"].Value.ToString();
                Console.WriteLine("Ncajas: " + dato.NCAJAS + " ");
                //Destino
                //dato.destino = tablaDatos.Rows[i].Cells["Destino"].Value.ToString();
                //Console.WriteLine("Destino: " + dato.destino + " ");

                numTotalCajas = Int32.Parse(dato.NCAJAS);
                for (int j = 0; j < numTotalCajas; j++)
                {
                    var values = new Dictionary<string, string>
                    {
                        { "Articulo",    dato.ARTICULO},
                        { "Cantidad",    dato.CANTIDAD},
                        { "Lote",        dato.LOTE},
                        { "Pedido",      dato.PEDIDO},
                        { "Albaran",     dato.ALBARAN},
                        { "Control",     dato.CONTROL},
                        { "NCajas",      dato.NCAJAS}
                       //{ "Destino",     dato.destino},
                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await httpClient.PostAsync("api/pruebas_post.php", content);

                    //Haciendo echo en pruebas_post recibimos el epc que queremos.
                    var contents = await response.Content.ReadAsStringAsync();

                    //EPC = contents;
                    ListaEPC[i,j] = contents;

                    //Console.WriteLine(contents);
                }
            }
        }
        int numCaja;
        int numeroFila;

        private void imprimir()
        {
            // Configurar impresora
            SatoPrinter satoPrinter = new SatoPrinter(pairData.IP, pairData.Port);

            int numCajasFila = 0;

            for (numeroFila = 0; numeroFila < tablaDatos.RowCount-1; numeroFila++)
            {
                numCajasFila = Int32.Parse(tablaDatos.Rows[numeroFila].Cells["Ncajas"].Value.ToString());
                for (numCaja = 0; numCaja < numCajasFila; numCaja++)
                {  

                    var fila = tablaDatos.Rows[numeroFila].Cells;

                    String articulo = fila["Articulo"].Value.ToString();
                    String cantidad = fila["Cantidad"].Value.ToString();
                    String lote = fila["Lote"].Value.ToString();
                    String pedido = fila["Pedido"].Value.ToString();
                    String albaran = fila["Albaran"].Value.ToString();
                    String control = fila["Control"].Value.ToString();
                    String numcajas = fila["Ncajas"].Value.ToString();

                    String epc = ListaEPC[numeroFila, numCaja];

                    satoPrinter.imprimir(articulo, cantidad, lote, pedido, albaran, control, numcajas, epc);
                }
            }

        }

        private void btnExistente_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form eleccion = new FormTablaDatos();
            eleccion.ShowDialog();
        }
    }
}
