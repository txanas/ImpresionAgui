package com.myruns.impresionagui.ui;

import javax.swing.*;
import java.awt.*;
import java.util.ArrayList;

public class UserInterfaceUtils {

    private static ArrayList<Image> icons;

    private UserInterfaceUtils(){

    }

    public synchronized static ArrayList<Image> getIcons(){
        if (icons == null){
            icons = new ArrayList<>(4);
            icons.add(new ImageIcon(UserInterfaceUtils.class.getResource("logo_small_16x16.png")).getImage());
            icons.add(new ImageIcon(UserInterfaceUtils.class.getResource("logo_small_32x32.png")).getImage());
            icons.add(new ImageIcon(UserInterfaceUtils.class.getResource("logo_small_64x64.png")).getImage());
            icons.add(new ImageIcon(UserInterfaceUtils.class.getResource("logo_small_128x128.png")).getImage());
        }
        return icons;
    }

    public static void showCentered(JFrame jFrame){
        Dimension dimension = Toolkit.getDefaultToolkit().getScreenSize();
        int x = (int) ((dimension.getWidth() - jFrame.getWidth()) / 2);
        int y = (int) ((dimension.getHeight() - jFrame.getHeight()) / 2);
        jFrame.setLocation(x, y);
        jFrame.setVisible(true);
    }


    /**
     * Sets the system look and feel for Swing components instead of the Java's default theme
     */
    public static void setSystemLookAndFeel(){
        // Set systems default look and feel
        try {
            UIManager.setLookAndFeel(
                    UIManager.getSystemLookAndFeelClassName());
        } catch (ClassNotFoundException | InstantiationException | IllegalAccessException | UnsupportedLookAndFeelException e) {
            // There is no problem as the default theme will be used
        }
    }

    public static ImageIcon getScaledLogo(){
        return getImageScaled("logo_blue.png");
    }

    public static ImageIcon getImageScaled(String name){
        ImageIcon icon = new ImageIcon(UserInterfaceUtils.class.getResource(name));
        return new ImageIcon(icon.getImage().getScaledInstance(564/3, 279/3, Image.SCALE_SMOOTH));
    }

    public static void showErrorMessage(String title, String message){
        showErrorMessage(title, message, null);
    }

    public static void showErrorMessage(String title, String message, Exception exception){
        if (exception != null){
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.append(message);
            stringBuilder.append("\n\n");
            StackTraceElement[] stackTrace = exception.getStackTrace();
            for (StackTraceElement element: stackTrace){
                stringBuilder.append(element.toString());
                stringBuilder.append("\n");
            }
            message = stringBuilder.toString();
        }
        JOptionPane.showMessageDialog(null, message + "\n\n" , title, JOptionPane.ERROR_MESSAGE);
    }

}
