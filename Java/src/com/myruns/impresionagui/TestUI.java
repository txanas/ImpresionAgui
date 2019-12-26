package com.myruns.impresionagui;

import com.myruns.impresionagui.ui.Layout;
import com.myruns.impresionagui.ui.UserInterfaceUtils;

public class TestUI {


    public static void main(String[] args) {
        UserInterfaceUtils.setSystemLookAndFeel();
        Layout layout = new Layout();
        layout.setWaiting();
        layout.setVisible(true);
    }



}
