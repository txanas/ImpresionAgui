package com.myruns.impresionagui;

import com.beust.jcommander.JCommander;


import java.io.File;
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

        File aguiLogo = new File("agui_logo.bmp");
        if (!aguiLogo.exists()){
            System.out.println("Fichero de logo no encontrado:");
            System.out.println(aguiLogo.getAbsolutePath());
        }

        DatabaseManager databaseManager = new DatabaseManager();
        try {
            databaseManager.connect(configuration);
                ArrayList<Articulo> articulosParaImprimir = databaseManager.getArticulosPendientesParaImprimir();
                if (articulosParaImprimir.size() > 0){
                    System.out.println("Hay " + articulosParaImprimir.size() + " impresiones pendientes");
                    for (Articulo articulo: articulosParaImprimir){

                        try {
                            Socket socket = new Socket(articulo.printerIP, 9100);
                            OutputStream output = socket.getOutputStream();
                            SatoCommand.sendComandoImpresion(output, articulo, aguiLogo);
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
        } catch (SQLException e) {
            e.printStackTrace();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

}
