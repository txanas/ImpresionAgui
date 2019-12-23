package com.myruns.impresionagui;

import com.beust.jcommander.JCommander;


import java.io.File;
import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.URISyntaxException;
import java.sql.SQLException;
import java.util.ArrayList;

public class Main {

    public static void main(String[] args) throws URISyntaxException {

        File logo = null;

        Configuration configuration = new Configuration();
        JCommander jCommander = new JCommander(configuration, args);


//        File aguiLogo = new File("agui_logo.bmp");
//        if (!aguiLogo.exists()){
//            System.out.println("Fichero de logo no encontrado:");
//            System.out.println(aguiLogo.getAbsolutePath());
//        }
        Layout Lay = new Layout();
        Lay.setVisible(true);
        Lay.setWaiting();

        while (true) {
            DatabaseManager databaseManager = new DatabaseManager();
            try {

                databaseManager.connect(configuration);
                ArrayList<Articulo> articulosParaImprimir = databaseManager.getArticulosPendientesParaImprimir();
                if (articulosParaImprimir.size() > 0){
                    System.out.println("Hay " + articulosParaImprimir.size() + " impresiones pendientes");
                    Lay.setPrinting();


                    for (Articulo articulo: articulosParaImprimir){
                        try {
                            Socket socket = new Socket();
                            socket.connect(new InetSocketAddress(articulo.printerIP, 9100), configuration.timeout);
                            OutputStream output = socket.getOutputStream();
                            char word = articulo.epc.charAt(16);
                            boolean aguiMode = false;
                            switch (word){
                                case 'A' :
                                    logo = new File("agui_logo.bmp");
                                    if (!logo.exists()){
                                        System.out.println("Fichero de logo no encontrado:");
                                        System.out.println(logo.getAbsolutePath());
                                    }
                                    aguiMode = true;
                                    break;
                                case 'B' :
                                    logo = new File("bidean_logo.bmp");
                                    if (!logo.exists()){
                                        System.out.println("Fichero de logo no encontrado:");
                                        System.out.println(logo.getAbsolutePath());
                                    }
                                    break;
                            }

                            SatoCommand.sendComandoImpresion(output, articulo, logo, aguiMode);
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
                    Lay.setWaiting();

                }
            } catch (SQLException e) {
                e.printStackTrace();
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    }
}
