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
           // httpClient.BaseAddress = new Uri("http://localhost:800");

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
                //Linea
                dato.LINEA = tablaDatos.Rows[i].Cells["Linea"].Value.ToString();
                Console.WriteLine("Linea: " + dato.LINEA + " ");
                //Albaran
                dato.ALBARAN = tablaDatos.Rows[i].Cells["Albaran"].Value.ToString();
                Console.WriteLine("Albaran: " + dato.ALBARAN + " ");
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
                        { "Linea",       dato.LINEA},
                        { "Albaran",     dato.ALBARAN},
                        { "NCajas",      dato.NCAJAS}
                       //{ "Destino",     dato.destino},
                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await httpClient.PostAsync("api/pruebas_post.php", content);

                    //Haciendo echo en pruebas_post recibimos el epc que queremos.
                    var contents = await response.Content.ReadAsStringAsync();

                    //EPC = contents;
                    ListaEPC[i,j] = contents;

                    Console.WriteLine(contents);
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
                    String linea = fila["Linea"].Value.ToString();
                    String albaran = fila["Albaran"].Value.ToString();
                    String numcajas = fila["Ncajas"].Value.ToString();

                    String epc = ListaEPC[numeroFila, numCaja];

                    satoPrinter.imprimir(articulo, cantidad, lote, pedido, albaran, linea, numcajas, epc);
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
