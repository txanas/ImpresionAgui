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

        private void imprimir()
        {
            // Configurar impresora
            Printer SATOPrinter = new Printer();
            SATOPrinter.Interface = Printer.InterfaceType.TCPIP;
            SATOPrinter.TCPIPAddress = "192.168.1.52";
            //SATOPrinter.TCPIPPort = opts.port.ToString();

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

            String articulo = tablaDatos.CurrentRow.Cells["Articulo"].Value.ToString();
            String cantidad = tablaDatos.CurrentRow.Cells["Cantidad"].Value.ToString();
            String lote = tablaDatos.CurrentRow.Cells["Lote"].Value.ToString();
            String pedido = tablaDatos.CurrentRow.Cells["Pedido"].Value.ToString();
            String albaran = tablaDatos.CurrentRow.Cells["Albaran"].Value.ToString();
            String control = tablaDatos.CurrentRow.Cells["Control"].Value.ToString();
            String numcajas = tablaDatos.CurrentRow.Cells["Ncajas"].Value.ToString();
            //String destino = tablaDatos.CurrentRow.Cells["Destino"].Value.ToString();

            Console.Write(comando);

            //Articulo y su barCode
            comando += comando += "<ESC>V20<ESC>H60";
            comando += "<ESC>B103120*" + articulo + "*";
            comando += "<ESC>V120<ESC>H60<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + articulo;

            //Cantidad y su barCode
            comando += "<ESC>V180<ESC>H60<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + "CANT. " +  cantidad;
            comando += comando += "<ESC>V170<ESC>H300";
            comando += "<ESC>B103120*" + cantidad + "*";

            //Albaran 
            comando += "<ESC>V120<ESC>H400<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + "ALBARAN " + albaran;

            //Pedido
            comando += "<ESC>V160<ESC>H400<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + "PEDIDO " + pedido;

            //Numero de cajas y caja actual
            int numeroTotalCajas = Int32.Parse(numcajas);

            comando += "<ESC>V20<ESC>H570<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + caja + "/" + numcajas;

            if (caja != numeroTotalCajas)
            {
                caja++;
            }

            //Control
            comando += "<ESC>V120<ESC>H570<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + "CONTROL ";
            comando += "<ESC>V150<ESC>H570<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + control;

            //Lote, numero y barcode (faltan el codigo y barcode, pero hay que darles la vuelta)
            comando += "<ESC>V20<ESC>H640<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + "LOTE ";

            //Graphic prueba
            comando += "<ESC>V10<ESC>H10<ESC>PGh0AH<ESC>GH006006";
            comando += Utils.ConvertGraphicToSBPL("C:\\Users\\Propietario\\Pictures\\agui.png");
            //comando += Utils.ConvertGraphicToSBPL(open.FileName);


            // Definir el EPC a escribir
            //comando += "<ESC>IP0e:z,d:" + epc + ";";

            // QR Code
            //if (qr != null)
            //{
            //    comando += "<ESC>V20<ESC>H600";
            //    comando += "<ESC>2D30,H,07,0,0";
            //    //comando += "<ESC>DS1," + qr;
            //}

            //if (barCode != null)
            //{
            //    comando += "<ESC>V20<ESC>H300";
            //    comando += "<ESC>B103120*1234AB*";
            //}

            // Texto a imprimir
            //comando += "<ESC>V20<ESC>H50<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + linea1;

            //if (linea2 != null)
            //{
            //    //comando += "<ESC>V80<ESC>H50<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + linea2;
            //}

            //if (linea3 != null)
            ////{
            //    comando += "<ESC>V130<ESC>H50<ESC>P4<ESC>L0101<ESC>RDB00,060,060," + linea3;
            //}

            // Cantidad de etiquetas a imprimir
            //comando += "<ESC>Q" + cantidad;

            // Fin del comando
            comando += "<ESC>Z<ETX>";

            return comando;
        }

        Datos dato = new Datos();

        public async void enviarDatos()
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

                int numTotalCajas = Int32.Parse(dato.ncajas);
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

                    EPC = contents;

                    Console.WriteLine(contents);
                }
            }
        }

        public async void getEPC()
        {
            HttpResponseMessage response = await httpClient.GetAsync("sdca/api/checkPermissions");
        }

        private void btn_imprimir_Click(object sender, EventArgs e)
        {
            //enviar los datos al servidor
            enviarDatos();

            //imprimir etiqueta con su epc
            imprimir();

            MessageBox.Show("Imprimiendo etiqueta...");
        }

        private void btnExistente_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form eleccion = new FormTablaDatos();
            eleccion.ShowDialog();
        }
    }
}
