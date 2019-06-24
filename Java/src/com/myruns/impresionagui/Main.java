package com.myruns.impresionagui;

import com.beust.jcommander.JCommander;


import java.io.IOException;
import java.io.OutputStream;
import java.net.Socket;
import java.net.URISyntaxException;
import java.sql.SQLException;
import java.util.ArrayList;

public class Main {

    public static void main(String[] args) throws URISyntaxException {

        Configuration configuration = new Configuration();
        JCommander jCommander = new JCommander(configuration, args);


        DatabaseManager databaseManager = new DatabaseManager();
        try {
            databaseManager.connect(configuration);

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
