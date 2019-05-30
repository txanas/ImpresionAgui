package com.myruns.impresionagui;

import java.io.IOException;
import java.net.Socket;

public class Main {

    public static void main(String[] args) {
        imprimir();
    }

    public static void imprimir(){
        String comando = SatoCommand.getCommandoImpresion("Articulo", "3", "001", "023042", "020", "4835", "000000000000000000001234");
        SatoCommand satoCommand = new SatoCommand(comando);
        try {
            Socket socket = new Socket("192.168.1.200", 9100);
            socket.getOutputStream().write(satoCommand.convertToSatoCommand());
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

}
