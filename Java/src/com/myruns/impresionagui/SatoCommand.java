package com.myruns.impresionagui;

import java.io.*;
import java.text.Normalizer;
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

    private byte[] convertToSatoCommand(){
        String command = text;
        for (int i=0; i<replaceKeys.length; i++){
            command = command.replaceAll(replaceKeys[i], replaceResult[i]);
        }
        return command.getBytes();
    }

    public static void sendComandoImpresion(OutputStream outputStream, Articulo articulo, File imageFile) throws IOException {
        sendComandoImpresion(outputStream, articulo.articulo, String.valueOf(articulo.cantidad), articulo.lote, String.valueOf(articulo.pedido), articulo.albaran, String.valueOf(articulo.numLinea), articulo.epc, articulo.control, imageFile);
    }

    public static void sendComandoImpresion(OutputStream output, String articulo, String cantidad, String lote, String pedido, String albaran, String linea, String epc, String control, File image) throws IOException {
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
        comando += "<ESC>BG03100" + unaccent(articulo.replaceAll("Ñ", "N").replaceAll("ñ", "n").toUpperCase());
        comando += "<ESC>V130<ESC>H20<ESC>P4<ESC>L0101<ESC>RDB@0,040,040," + articulo;

        //Cantidad y su barCode
        comando += "<ESC>V175<ESC>H250<ESC>P4<ESC>L0101<ESC>RDB00,040,040," + "CANT. " + cantidad;
        comando += "<ESC>V170<ESC>H20";
        comando += "<ESC>B102040*" + cantidad + "*";

        //Albaran
        comando += "<ESC>V120<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "ALBARAN " + albaran;

        //Pedido
        comando += "<ESC>V145<ESC>H310<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "PEDIDO " + pedido;

        //EPC
        comando += "<ESC>V10<ESC>H580<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "EPC " + epc.substring(epc.length() - 4);

        //Control
        if (control != null && control.trim().length() > 0){
            comando += "<ESC>V35<ESC>H580<ESC>P4<ESC>L0101<ESC>RDB00,050,050," + control;
        }


        //Linea
        comando += "<ESC>V120<ESC>H560<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "LINEA ";
        comando += "<ESC>V145<ESC>H580<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + linea;

        //Lote, numero y barcode
        comando += "<ESC>V120<ESC>H650<ESC>P4<ESC>L0101<ESC>RDB00,020,020," + "LOTE ";
        comando += "<ESC>V145<ESC>H650<ESC>P4<ESC>L0101<ESC>RDB00,035,035," + lote;
        comando += "<ESC>V170<ESC>H540";
        comando += "<ESC>B102040*" + lote + "*";

        output.write(new SatoCommand(comando).convertToSatoCommand());

        // imprimir imagen
        output.write(new SatoCommand(SatoCommand.getComandoImagen(image.length())).convertToSatoCommand());
        InputStream fis=new FileInputStream(image);
        byte[] buff = new byte[1024];
        int read;
        while((read=fis.read(buff))>=0){
            output.write(buff,0,read);
        }

        output.write(new SatoCommand("<ESC>Z<ETX>").convertToSatoCommand());
        output.flush();
    }

    private static String getComandoImagen(long size){

        String comando = "<ESC>V10<ESC>H700";
        // Tamano de la imagen
        comando += "<ESC>L0101";
        comando += "<ESC>GM" + String.format("%05d", size);
        comando += ",";
        return comando;

    }

    /**
     * Remove toda a acentuação da string substituindo por caracteres simples sem acento.
     */
    public static String unaccent(String src) {
        return Normalizer
                .normalize(src, Normalizer.Form.NFD)
                .replaceAll("[^\\p{ASCII}]", "");
    }

}
