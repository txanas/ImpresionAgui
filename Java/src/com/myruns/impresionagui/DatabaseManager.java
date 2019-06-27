package com.myruns.impresionagui;

import java.sql.*;
import java.util.ArrayList;
import java.util.TimeZone;

public class DatabaseManager {

    public Connection connection;

    public void connect(Configuration conf) throws SQLException {

        closeConnection();


        // IP address
        String dbIP = "jdbc:mysql://" + conf.db_host + ":" + String.valueOf(conf.db_port) + "/" + conf.db_name + "?serverTimezone=" + TimeZone.getDefault().getID() + "&characterEncoding=utf8";

        System.out.println("Conectando a base de datos...");
        connection = DriverManager.getConnection(dbIP, conf.db_user, conf.db_password);
        System.out.println("Conectado correctamente a la base de datos");
    }

    public void closeConnection(){
        if (connection != null){
            try {
                System.out.println("Cerrando conexion con base de datos...");
                connection.close();
                System.out.println("Conexion con base de datos cerrada");
            } catch (SQLException e) {
                e.printStackTrace();
            }
        }
        connection = null;
    }

    public ArrayList<Articulo> getArticulosPendientesParaImprimir() throws SQLException {

        ArrayList<Articulo> articulos = new ArrayList<>();

        Statement st = connection.createStatement();

        // execute the query, and get a java resultset
        ResultSet rs = st.executeQuery("SELECT Articulos.*, Impresoras.ip AS impresora FROM ColaImpresion LEFT JOIN Articulos ON ColaImpresion.epc=Articulos.epc LEFT JOIN Impresoras ON Impresoras.id=ColaImpresion.idImpresora;");

        // iterate through the java resultset
        while (rs.next())
        {

            Articulo articulo = new Articulo();
            articulo.id = rs.getInt("idarticulos");
            articulo.tipoPedido = rs.getString("tipoPedido");
            articulo.pedido = rs.getInt("pedido");
            articulo.articulo = rs.getString("articulo");
            articulo.numLinea = rs.getInt("linea");
            articulo.cantidad = rs.getInt("cantidad");
            articulo.lote = rs.getString("lote");
            articulo.localizacion = rs.getString("localizacion");
            articulo.albaran = rs.getString("albaran");
            articulo.estadoSpyro = rs.getInt("estadoSpyro");
            articulo.epc = rs.getString("epc");
            articulo.control = rs.getString("control");
            articulo.printerIP = rs.getString("impresora");

            articulos.add(articulo);
        }
        st.close();

        return articulos;
    }

    public void marcarImpreso(Articulo articulo) throws SQLException {
        PreparedStatement ps = connection.prepareStatement("DELETE FROM ColaImpresion WHERE epc=?;");
        ps.setString(1, articulo.epc);
        ps.addBatch();
        ps.executeBatch();
    }

}
