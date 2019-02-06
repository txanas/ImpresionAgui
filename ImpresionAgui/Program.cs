using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using SATOPrinterAPI;

namespace ImpresionAgui
{
    static class Program
    {

         class Options
        {
            [Option("ip", Required = true, Default = null, HelpText = "Dirección IP de la impresora")]
            public String ip { get; set; }

        //    [Option("puerto", Default = 9100, HelpText = "Puerto de red la impresora")]
        //    public int port { get; set; }

        //    [Option("epc", Required = true, Default = null, HelpText = "La información a codificar en la etiqueta RFID. Deben ser 24 caracteres hexadecimales [0-9][A-F]")]
        //    public String epc { get; set; }

        //    [Option("linea1", Required = true, Default = null, HelpText = "Texto a escribir en la etiqueta")]
        //    public String linea1 { get; set; }

        //    [Option("linea2", Default = null, HelpText = "Texto a escribir en la etiqueta")]
        //    public String linea2 { get; set; }

        //    [Option("linea3", Default = null, HelpText = "Texto a escribir en la etiqueta")]
        //    public String linea3 { get; set; }

        //    [Option("qr", Default = null, HelpText = "Texto del código QR")]
        //    public String qr { get; set; }

        //    [Option("barCode", Default = null, HelpText = "Texto del código de barras")]
        //    public String barCode { get; set; }

        //    [Option("cantidad", Default = 1, HelpText = "Cantidad de etiquetas a imprimir")]
        //    public int cantidad { get; set; }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormNuevaEtiqueta());

            //Parser.Default.ParseArguments<Options>(args)
            //  .WithParsed<Options>(opts => main(opts))
            //     .WithNotParsed<Options>((errs) => errorDeArgumentos(errs));
        }

        //private static void errorDeArgumentos(IEnumerable<Error> errs)
        //{
        //    salirConError(null, ERROR_CODE_PARAMS);
        //}

        //private static void salirConError(String mensaje, int errorCode)
        //{
        //    if (mensaje != null)
        //    {
        //        Console.WriteLine(mensaje);
        //    }
        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine("Pulse enter para salir");
        //    var exitInput = Console.ReadLine();
        //    Environment.Exit(errorCode);
        //}

    }
}