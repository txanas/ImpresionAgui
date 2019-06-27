package com.myruns.impresionagui;

import java.util.ArrayList;

public class SatoCommand {

    private static final String COMMAND_START = "<STX><ESC>A";
    private static final String COMMAND_END = "Z<ETX>";
    public static final String COMMAND_ESCAPE = "<ESC>";
    private static final String QUANTITY_COMMAND = "Q";
    private static final String EPC_WRITE_COMMAND = "IP0e:z,d:%s;";

    private static final String[] replaceKeys = {"<STX>", "<ESC>", "<ETX>"};
    private static final String[] replaceResult = {"\u0002", "\u001B", "\u0003"};

    private String text;

    public SatoCommand(String text){
        this.text = text;
    }

    public byte[] convertToSatoCommand(){
        String command = text;
        for (int i=0; i<replaceKeys.length; i++){
            command = command.replaceAll(replaceKeys[i], replaceResult[i]);
        }
        return command.getBytes();
    }

    public static String getCommandoImpresion(Articulo articulo)
    {
        return getCommandoImpresion(articulo.articulo, String.valueOf(articulo.cantidad), articulo.lote, String.valueOf(articulo.pedido), articulo.albaran, String.valueOf(articulo.numLinea), articulo.epc, articulo.control);
    }

    public static String getCommandoImpresion(String articulo, String cantidad, String lote, String pedido, String albaran, String linea, String epc, String control)
    {
        // Inicio del comando
        String comando = "<STX><ESC>A";

        // Definir el EPC a escribir
        comando += "<ESC>IP0e:z,d:" + epc + ";";

        //string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
        //string FileName = string.Format("{0}Resources\\agui_bmp.bmp", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));

        // Uppercase para que funcione el barcode
        //Articulo y su barCode

        //Articulo y su barCode
        comando += "<ESC>V10<ESC>H20";
        comando += "<ESC>B103100*" + articulo.replaceAll("Ñ", "N").replaceAll("ñ", "n").toUpperCase() + "*";
        comando += "<ESC>V130<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB@0,040,040," + articulo;

        //Cantidad y su barCode
        comando += "<ESC>V175<ESC>H250<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + "CANT. " + cantidad;
        comando += "<ESC>V170<ESC>H20";
        comando += "<ESC>B102040*" + cantidad + "*";

        //Albaran
        comando += "<ESC>V120<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "ALBARAN " + albaran;

        //Pedido
        comando += "<ESC>V145<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "PEDIDO " + pedido;

        //Pedido
        comando += "<ESC>V10<ESC>H680<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "EPC " + epc.substring(epc.length() - 4);

        //Control
        if (control != null && control.trim().length() > 0){
            comando += "<ESC>V35<ESC>H680<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "Control " + control;
        }


        //Linea
        comando += "<ESC>V120<ESC>H560<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "LINEA ";
        comando += "<ESC>V145<ESC>H580<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + linea;

        //Lote, numero y barcode
        comando += "<ESC>V120<ESC>H650<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "LOTE ";
        comando += "<ESC>V145<ESC>H650<ESC>P4<ESC>L0101<ESC>RDB00,035,035," + lote;
        comando += "<ESC>V170<ESC>H540";
        comando += "<ESC>B102040*" + lote + "*";

        // Cantidad de etiquetas a imprimir
        // comando += "<ESC>Q" + numcajas;

        // Fin del comando
        comando += "<ESC>Z<ETX>";

        return comando;
    }

}
