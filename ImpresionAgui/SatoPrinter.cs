using SATOPrinterAPI;
using System;
using System.IO;

namespace ImpresionAgui
{
    
    public class SatoPrinter
    {
        private Printer printer;

        public SatoPrinter(String ip, String port)
        {
            printer = new Printer();
            // Configurar impresora
            printer.Interface = Printer.InterfaceType.TCPIP;
            printer.TCPIPAddress = ip;
            printer.TCPIPPort = port;
        }

        public void imprimir(String articulo, String cantidad, String lote, String pedido, String albaran, String linea, String numcajas, String epc)
        {
            String PrintCommand = getCommandoImpresion(articulo, cantidad, lote, pedido, albaran, linea, numcajas, epc);
            // Cambiar los caracteres de escape
            PrintCommand = PrintCommand.Replace("<STX>", ((char)02).ToString());
            PrintCommand = PrintCommand.Replace("<ETX>", ((char)03).ToString());
            PrintCommand = PrintCommand.Replace("<ESC>", ((char)27).ToString());

            // Convertir a bytes
            byte[] cmddata = Utils.StringToByteArray(PrintCommand);

            // Enviar comando a la impresora
            try
            {
              printer.Send(cmddata);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                // TODO MOSTRAR ERROR
                //salirConError("Se ha producido un error desconocido al enviar el comando a la impresora. Compruebe la dirección IP y si la impresora está correctamente conectada.", ERROR_CODE_ERROR_DESCONOCIDO);
            }
        }


        private String getCommandoImpresion(String articulo, String cantidad, String lote, String pedido, String albaran, String linea, String numcajas, String epc)
        {
            // Inicio del comando
            String comando = "<STX><ESC>A";

            //Darkness
            //comando += "<ESC>#E3A";

            Console.Write(comando);
            // Definir el EPC a escribir
            comando += "<ESC>IP0e:z,d:" + epc + ";";

            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            string FileName = string.Format("{0}Resources\\agui_bmp.bmp", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));

            //Graphic prueba
            //TODO da error en impresora
            comando += "<ESC>V10<ESC>H540<ESC>PGh0AH<ESC>GH006006";
            comando += Utils.ConvertGraphicToSBPL(FileName);

            // //Articulo y su barCode
            //comando += "<ESC>V10<ESC>H20";
            //comando += "<ESC>B10280*" + articulo + "*";
            //comando += "<ESC>V130<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + articulo;

            //Articulo y su barCode
            comando += "<ESC>V10<ESC>H20";
            comando += "<ESC>B103100*" + articulo + "*";
            comando += "<ESC>V130<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + articulo;

            //Cantidad y su barCode
            comando += "<ESC>V175<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,025,025," + "CANT. " + cantidad;
            comando += "<ESC>V170<ESC>H250";
            comando += "<ESC>B102040*" + cantidad + "*";

            //Albaran 
            comando += "<ESC>V120<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "ALBARAN " + albaran;

            //Pedido
            comando += "<ESC>V145<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "PEDIDO " + pedido;

            //Numero de cajas y caja actual
            //int numeroTotalCajas = Int32.Parse(numcajas);

            //comando += "<ESC>V20<ESC>H500<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + caja + "/" + numcajas;

            //if (caja != numeroTotalCajas)
            //{
            //    caja++;
            //}

            //Linea
            comando += "<ESC>V120<ESC>H560<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "LINEA ";
            comando += "<ESC>V145<ESC>H580<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + linea;

            ////Lote, numero y barcode
            //comando += "<ESC>%1<ESC>V140<ESC>H690<ESC>P4<ESC>L0101<ESC>RDB00,030,030," + "LOTE ";
            //comando += "<ESC>%1<ESC>V140<ESC>H730<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + lote;
            //comando += "<ESC>%1<ESC>V200<ESC>H760";
            //comando += "<ESC>B102040*" + lote + "*";
            //Lote, numero y barcode
            comando += "<ESC>V120<ESC>H650<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "LOTE ";
            comando += "<ESC>V145<ESC>H650<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + lote;
            comando += "<ESC>V170<ESC>H540";
            comando += "<ESC>B102040*" + lote + "*";


            // Cantidad de etiquetas a imprimir
            // comando += "<ESC>Q" + numcajas;

            // Fin del comando
            comando += "<ESC>Z<ETX>";

            return comando;
        }



    }
}
