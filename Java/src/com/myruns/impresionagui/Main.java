package com.myruns.impresionagui;

import java.io.File;
import java.io.IOException;
import java.io.OutputStream;
import java.net.Socket;
import java.net.URISyntaxException;
import java.sql.SQLException;
import java.util.ArrayList;

public class Main {

    public static void main(String[] args) throws URISyntaxException {
        // Configuration
        File file = new File(Main.class.getProtectionDomain().getCodeSource().getLocation().toURI().getPath()).getParentFile();
        //ConfigurationManager.get().init(file);
        DatabaseManager databaseManager = new DatabaseManager();
        try {
            databaseManager.connect();

            while (true){
                ArrayList<Articulo> articulosParaImprimir = databaseManager.getArticulosPendientesParaImprimir();
                if (articulosParaImprimir.size() > 0){
                    System.out.println("Hay " + articulosParaImprimir.size() + " impresiones pendientes");
                    for (Articulo articulo: articulosParaImprimir){
                        SatoCommand satoCommand = new SatoCommand(SatoCommand.getCommandoImpresion(articulo));
                        try {
                            Socket socket = new Socket(articulo.printerIP, 9100);
                            OutputStream output = socket.getOutputStream();
                            output.write(satoCommand.convertToSatoCommand());
                            output.flush();
                            Thread.sleep(300);
                            output.close();
                            socket.close();
                            databaseManager.marcarImpreso(articulo);
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                    }
                }else{
                    System.out.println("Nada para imprimir");
                }
                Thread.sleep(100);
            }
        } catch (SQLException e) {
            e.printStackTrace();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

}
