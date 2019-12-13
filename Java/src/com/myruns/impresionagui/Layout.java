package com.myruns.impresionagui;

import javax.swing.*;
import java.awt.*;

/**
 * Created by Propietario on 13/12/2019.
 */
public class Layout {

    public static void addWaiting(Container pane) {
        pane.setLayout(new FlowLayout(FlowLayout.CENTER,150,150 ));
        JLabel texto = new JLabel ("Esperando...");
        texto.setFont(texto.getFont ().deriveFont (34.0f));
        pane.add(texto);

    }

    public static void addPrinting(Container pane) {
        pane.setLayout(new FlowLayout(FlowLayout.CENTER,150,150 ));
        JLabel texto = new JLabel ("Imprimiendo...");
        texto.setFont(texto.getFont ().deriveFont (34.0f));
        pane.add(texto);

    }

    /**
     * Create the GUI and show it.  For thread safety,
     * this method should be invoked from the
     * event-dispatching thread.
     */
    public static void setEsperando() {
        //Create and set up the window.
        JFrame frame = new JFrame("Impresion");
        //frame.setSize(1000,2000);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        addWaiting(frame.getContentPane());

        //Display the window.
        frame.pack();
        frame.setVisible(true);
    }

    /**
     * Create the GUI and show it.  For thread safety,
     * this method should be invoked from the
     * event-dispatching thread.
     */
    public static void setImprimiendo() {
        //Create and set up the window.
        JFrame frame = new JFrame("Impresion");
        //frame.setSize(1000,2000);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        addPrinting(frame.getContentPane());

        //Display the window.
        frame.pack();
        frame.setVisible(true);
    }

}
