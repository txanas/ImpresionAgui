package com.myruns.impresionagui;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JTextField;

/**
 * Created by Propietario on 17/12/2019.
 */
public class Layout extends JFrame  {

    private JLabel text;

    public Layout() {
        super();
        configureLayout();
        text = new JLabel();
        this.add(text);
    }

    private void configureLayout() {
        this.setTitle("Impresión");
        this.setSize(500, 500);
        this.setLocationRelativeTo(null);
        this.setLayout(null);
        this.setResizable(false);
        this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
    }

    public void setWaiting() {

        Font fuente =new Font("TimesRoman", Font.BOLD, 16);

        text.setText("Esperando...");    // colocamos un texto a la etiqueta
        text.setBounds(100, 50, 300, 300);   // colocamos posicion y tamanio al texto (x, y, ancho, alto)
        text.setFont(text.getFont ().deriveFont (34.0f));
    }

    public void setPrinting() {
        Font fuente =new Font("TimesRoman", Font.BOLD, 16);

        text.setText("Imprimiendo...");    // colocamos un texto a la etiqueta
        text.setBounds(100, 50, 300, 300);   // colocamos posicion y tamanio al texto (x, y, ancho, alto)
        text.setFont(text.getFont ().deriveFont (34.0f));
    }

}
