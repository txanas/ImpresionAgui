using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using SATOPrinterAPI;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;


namespace ImpresionAgui
{
    public partial class FormNuevaEtiqueta : Form
    {
        public FormNuevaEtiqueta()
        {
            InitializeComponent();
        }

        private static readonly int ERROR_CODE_PARAMS = -1;
        private static readonly int ERROR_CODE_EPC_NOT_HEX = -2;
        private static readonly int ERROR_CODE_EPC_NOT_24_CHARS = -3;
        private static readonly int ERROR_CODE_ERROR_DESCONOCIDO = -4;

        private HttpClient httpClient;

        private String EPC;
        string[] ListaEPC = new string[20];
        int numTotalCajas;


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
        int caja;

        public async Task enviarDatos()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://agui.myruns.com");
            //httpClient.BaseAddress = new Uri("http://localhost:800");

            for (int i = 0; i < tablaDatos.RowCount - 1; i++)
            {
                //Articulo
                dato.articulo = tablaDatos.Rows[i].Cells["Articulo"].Value.ToString();
                Console.WriteLine("Articulo: " + dato.articulo + " ");
                //Cantidad
                dato.cantidad = tablaDatos.Rows[i].Cells["Cantidad"].Value.ToString();
                Console.WriteLine("Cantidad: " + dato.cantidad + " ");
                //Lote
                dato.lote = tablaDatos.Rows[i].Cells["Lote"].Value.ToString();
                Console.WriteLine("Lote: " + dato.lote + " ");
                //Pedido
                dato.pedido = tablaDatos.Rows[i].Cells["Pedido"].Value.ToString();
                Console.WriteLine("Pedido: " + dato.pedido + " ");
                //Albaran
                dato.albaran = tablaDatos.Rows[i].Cells["Albaran"].Value.ToString();
                Console.WriteLine("Albaran: " + dato.albaran + " ");
                //Control
                dato.control = tablaDatos.Rows[i].Cells["Control"].Value.ToString();
                Console.WriteLine("Control: " + dato.control + " ");
                //Numero cajas
                dato.ncajas = tablaDatos.Rows[i].Cells["Ncajas"].Value.ToString();
                Console.WriteLine("Ncajas: " + dato.ncajas + " ");
                //Destino
                //dato.destino = tablaDatos.Rows[i].Cells["Destino"].Value.ToString();
                //Console.WriteLine("Destino: " + dato.destino + " ");

                numTotalCajas = Int32.Parse(dato.ncajas);
                caja = 1;
                for (int j = 0; j < numTotalCajas; j++)
                {
                    var values = new Dictionary<string, string>
                    {
                        { "Articulo",    dato.articulo},
                        { "Cantidad",    dato.cantidad},
                        { "Lote",        dato.lote},
                        { "Pedido",      dato.pedido},
                        { "Albaran",     dato.albaran},
                        { "Control",     dato.control},
                        { "NCajas",      dato.ncajas}
                       //{ "Destino",     dato.destino},
                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await httpClient.PostAsync("api/pruebas_post.php", content);

                    //Haciendo echo en pruebas_post recibimos el epc que queremos.
                    var contents = await response.Content.ReadAsStringAsync();

                    //EPC = contents;
                    ListaEPC[j] = contents;

                    //Console.WriteLine(contents);
                }
            }
        }
        int numCaja;

        private void imprimir()
        {
            // Configurar impresora
            Printer SATOPrinter = new Printer();
            SATOPrinter.Interface = Printer.InterfaceType.TCPIP;
            SATOPrinter.TCPIPAddress = "192.168.1.52";
            SATOPrinter.TCPIPPort = "9100";

            // Generar comando de impresión
            //String PrintCommand = getCommandoImpresion(opts.cantidad, opts.epc, opts.linea1, opts.linea2, opts.linea3, opts.qr, opts.barCode);

            for (numCaja = 0; numCaja < numTotalCajas; numCaja++)
            {
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
        }

        private String getCommandoImpresion()
        {

            // Inicio del comando
            String comando = "<STX><ESC>A";

            String articulo = tablaDatos.CurrentRow.Cells["Articulo"].Value.ToString();
            String cantidad = tablaDatos.CurrentRow.Cells["Cantidad"].Value.ToString();
            String lote = tablaDatos.CurrentRow.Cells["Lote"].Value.ToString();
            String pedido = tablaDatos.CurrentRow.Cells["Pedido"].Value.ToString();
            String albaran = tablaDatos.CurrentRow.Cells["Albaran"].Value.ToString();
            String control = tablaDatos.CurrentRow.Cells["Control"].Value.ToString();
            String numcajas = tablaDatos.CurrentRow.Cells["Ncajas"].Value.ToString();
            //String destino = tablaDatos.CurrentRow.Cells["Destino"].Value.ToString();

            //Console.Write(comando);

            // Definir el EPC a escribir
            comando += "<ESC>IP0e:z,d:" + ListaEPC[numCaja] + ";";

            Console.Write(comando);

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
            int numeroTotalCajas = Int32.Parse(numcajas);

            //comando += "<ESC>V20<ESC>H500<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + numCaja + "/" + numeroTotalCajas;

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
            string FileName = string.Format("{0}Resources\\agui.png", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));



            //Graphic prueba
            comando += "<ESC>V10<ESC>H540<ESC>PGh0AH<ESC>GH006006";
            //comando += Utils.ConvertGraphicToSBPL(FileName);
            //comando += Utils.ConvertGraphicToSBPL(open.FileName);

            // Cantidad de etiquetas a imprimir
            //comando += "<ESC>Q" + numcajas;

            // Fin del comando
            comando += "<ESC>Z<ETX>";

            return comando;
        }

        private void btnExistente_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form eleccion = new FormTablaDatos();
            eleccion.ShowDialog();
        }
    }
}
