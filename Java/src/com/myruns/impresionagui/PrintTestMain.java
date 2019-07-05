package com.myruns.impresionagui;

import java.io.*;
import java.net.Socket;

public class PrintTestMain {

    public static void main(String[] args) throws IOException, InterruptedException {

        File aguiLogo = new File("agui_logo.bmp");
        if (aguiLogo.exists()){

            System.out.print(aguiLogo.getAbsolutePath());
        }
        System.out.println(aguiLogo.getAbsolutePath());

        Articulo articulo = new Articulo();
        articulo.articulo = "Prueba";
        articulo.epc = "616775690101010100000001";
        articulo.albaran = "Albaran";
        articulo.lote = "5000";
        articulo.cantidad = 2000;
        articulo.numLinea = 1;
        articulo.pedido = 12345;
        articulo.consigna = "CS";
        articulo.control = "CN";

        SatoCommand satoCommand = new SatoCommand(SatoCommand.getCommandoImpresion(articulo));

        Socket socket = new Socket("192.168.1.200", 9100);
        OutputStream output = socket.getOutputStream();
        output.write(satoCommand.convertToSatoCommand());

        // Send image
        output.write(new SatoCommand(SatoCommand.getComandoImagen(aguiLogo.length())).convertToSatoCommand());
        InputStream fis=new FileInputStream(aguiLogo);
        byte[] buff = new byte[1024];
        int read;
        while((read=fis.read(buff))>=0){
            output.write(buff,0,read);
        }

        output.write(new SatoCommand("<ESC>Z<ETX>").convertToSatoCommand());
        output.flush();
        Thread.sleep(300);
        output.close();
        socket.close();
    }

}
