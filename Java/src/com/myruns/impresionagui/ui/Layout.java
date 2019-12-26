package com.myruns.impresionagui.ui;
import java.awt.*;
import javax.swing.*;

/**
 * Created by Propietario on 17/12/2019.
 */
public class Layout extends JFrame  {

    private JLabel text;

    public Layout() {
        super();
        text = new JLabel("", SwingConstants.CENTER);
        setLayout(new BorderLayout());
        this.add(text, BorderLayout.CENTER);
        configureLayout();
    }

    private void configureLayout() {
        this.setTitle("MYRUNS Impresión");
        setBackground(Color.WHITE);
        getContentPane().setBackground(Color.WHITE);
        this.setSize(500, 500);
        this.setLocationRelativeTo(null);
        this.setResizable(false);
        setIconImages(UserInterfaceUtils.getIcons());
        this.setDefaultCloseOperation(WindowConstants.EXIT_ON_CLOSE);
        text.setFont(text.getFont().deriveFont(34.0f));
    }

    public void setConectando() {
        text.setText("Conectando...");    // colocamos un texto a la etiqueta
    }

    public void setError() {
        text.setText("Error de conexión");    // colocamos un texto a la etiqueta
    }

    public void setWaiting() {
        text.setText("Esperando...");    // colocamos un texto a la etiqueta
    }

    public void setPrinting() {
        text.setText("Imprimiendo...");    // colocamos un texto a la etiqueta
    }

}
