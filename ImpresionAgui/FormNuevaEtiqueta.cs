﻿using System;
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
using System.Collections.Specialized;
using System.Configuration;

namespace ImpresionAgui
{
    public partial class FormNuevaEtiqueta : Form
    {
        private static readonly int ERROR_CODE_ERROR_DESCONOCIDO = -4;

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

        public async Task enviarDatos()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://agui.myruns.com");
            //httpClient.BaseAddress = new Uri("http://localhost:800");

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
            Printer SATOPrinter = new Printer();
            SATOPrinter.Interface = Printer.InterfaceType.TCPIP;
            SATOPrinter.TCPIPAddress = pairData.IP;
            SATOPrinter.TCPIPPort = pairData.Port;

            // Generar comando de impresión
            //String PrintCommand = getCommandoImpresion(opts.cantidad, opts.epc, opts.linea1, opts.linea2, opts.linea3, opts.qr, opts.barCode);

            int numCajasFila = 0;

            for (numeroFila = 0; numeroFila < tablaDatos.RowCount-1; numeroFila++)
            {
                numCajasFila = Int32.Parse(tablaDatos.Rows[numeroFila].Cells["Ncajas"].Value.ToString());
                for (numCaja = 0; numCaja < numCajasFila; numCaja++)
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

        }

        private String getCommandoImpresion()
        {

            // Inicio del comando
            String comando = "<STX><ESC>A";

            String articulo = tablaDatos.Rows[numeroFila].Cells["Articulo"].Value.ToString();
            String cantidad = tablaDatos.Rows[numeroFila].Cells["Cantidad"].Value.ToString();
            String lote = tablaDatos.Rows[numeroFila].Cells["Lote"].Value.ToString();
            String pedido = tablaDatos.Rows[numeroFila].Cells["Pedido"].Value.ToString();
            String albaran = tablaDatos.Rows[numeroFila].Cells["Albaran"].Value.ToString();
            String control = tablaDatos.Rows[numeroFila].Cells["Control"].Value.ToString();
            String numcajas = tablaDatos.Rows[numeroFila].Cells["Ncajas"].Value.ToString();
            //String destino = tablaDatos.CurrentRow.Cells["Destino"].Value.ToString();

            //Console.Write(comando);

            String epc = ListaEPC[numeroFila,numCaja];
            // Definir el EPC a escribir
            comando += "<ESC>IP0e:z,d:" + epc + ";";

            Console.Write(comando);

            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            string FileName = string.Format("{0}Resources\\agui_negro.png", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));

            //Graphic prueba
            comando += "<ESC>V10<ESC>H540<ESC>PGh0AH<ESC>GH006006";
            comando += Utils.ConvertGraphicToSBPL(FileName);

            //Articulo y su barCode
            comando += "<ESC>V00<ESC>H20";
            comando += "<ESC>B103100*" + articulo + "*";
            comando += "<ESC>V120<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + articulo;

            //Cantidad y su barCode
            comando += "<ESC>V165<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,025,025," + "CANT. " + cantidad;
            comando += "<ESC>V160<ESC>H250";
            comando += "<ESC>B103040*" + cantidad + "*";

            //Albaran 
            comando += "<ESC>V110<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "ALBARAN " + albaran;

            //Pedido
            comando += "<ESC>V135<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "PEDIDO " + pedido;

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

            //Lote, numero y barcode
            comando += "<ESC>%1<ESC>V140<ESC>H690<ESC>P4<ESC>L0101<ESC>RDB00,030,030," + "LOTE ";
            comando += "<ESC>%1<ESC>V140<ESC>H730<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + lote;
            comando += "<ESC>%1<ESC>V160<ESC>H760";
            comando += "<ESC>B103040*" + lote + "*";

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
