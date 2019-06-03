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
        return getCommandoImpresion(articulo.articulo, String.valueOf(articulo.cantidad), articulo.lote, String.valueOf(articulo.pedido), articulo.albaran, String.valueOf(articulo.numLinea), articulo.epc);
    }

    public static String getCommandoImpresion(String articulo, String cantidad, String lote, String pedido, String albaran, String linea, String epc)
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
        comando += "<ESC>B103100*" + articulo.toUpperCase() + "*";
        comando += "<ESC>V130<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + articulo;

        //Cantidad y su barCode
        comando += "<ESC>V175<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB00,025,025," + "CANT. " + cantidad;
        comando += "<ESC>V170<ESC>H250";
        comando += "<ESC>B103040*" + cantidad + "*";

        //Albaran
        comando += "<ESC>V120<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "ALBARAN " + albaran;

        //Pedido
        comando += "<ESC>V145<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "PEDIDO " + pedido;

        //Linea
        comando += "<ESC>V90<ESC>H540<ESC>P4<ESC>L0101<ESC>RDB00,025,025," + "LINEA ";
        comando += "<ESC>V120<ESC>H560<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + linea;

        //Lote, numero y barcode
        comando += "<ESC>%1<ESC>V140<ESC>H690<ESC>P4<ESC>L0101<ESC>RDB00,030,030," + "LOTE ";
        comando += "<ESC>%1<ESC>V140<ESC>H730<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + lote;
        comando += "<ESC>%1<ESC>V200<ESC>H760";
        comando += "<ESC>B103040*" + lote + "*";

        // Cantidad de etiquetas a imprimir
        // comando += "<ESC>Q" + numcajas;

        // Fin del comando
        comando += "<ESC>Z<ETX>";

        return comando;
    }

}
