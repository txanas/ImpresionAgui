package com.myruns.impresionagui;


import com.beust.jcommander.Parameter;

public class Configuration {

    @Parameter(names = { "-dbHost" }, description = "Dirección IP donde se encuentra de la base de datos", required = true)
    public String db_host;

    @Parameter(names = { "-dbPort" }, description = "Nombre de la base de datos")
    public int db_port = 3306;

    @Parameter(names = { "-dbName" }, description = "Nombre de la base de datos", required = true)
    public String db_name;

    @Parameter(names = { "-dbUser" }, description = "Usuario de la base de datos", required = true)
    public String db_user;

    @Parameter(names = { "-dbPassword" }, description = "Contraseña de la base de datos", required = true)
    public String db_password;

    @Parameter(names = { "-timeout" }, description = "Timeout de conexion con la impresora (ms)", required = true)
    public int timeout = 15000;

}
