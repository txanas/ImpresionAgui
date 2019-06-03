package com.myruns.impresionagui;

/*
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;
*/
import java.io.File;

public class ConfigurationManager {

    private static final String configFileName = "config.xml";

    private static ConfigurationManager myConfigurationManager;

    private Configuration configuration;

    private ConfigurationManager(){
        this.configuration = new Configuration();
    }

    /*
    public void init(File path){
        this.configuration = new Configuration();
        File configFile = new File(path, configFileName);
        System.out.println("Looking for existing configuration file: " + configFile.getAbsolutePath());
        if (configFile.exists()){
            System.out.println("Configuration file found: " + configFile.getAbsolutePath());
            loadConfiguration(configFile);
        }else{
            System.out.println("Configuration file not found");
            createInitialConfig(configFile);
        }
    }

    private void createInitialConfig(File file){
        System.out.println("Creating default configuration file: " + file.getAbsolutePath());
        JAXBContext jaxbContext = null;
        try {
            jaxbContext = JAXBContext.newInstance(Configuration.class);
            Marshaller jaxbMarshaller = jaxbContext.createMarshaller();

            // output pretty printed
            jaxbMarshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true);

            jaxbMarshaller.marshal(configuration, file);
        } catch (JAXBException e) {
            e.printStackTrace();
        }
    }

    private void loadConfiguration(File file){
        System.out.println("Loading configuration file: " + file.getAbsolutePath());
        JAXBContext jaxbContext = null;
        try {
            jaxbContext = JAXBContext.newInstance(Configuration.class);
            Unmarshaller jaxbUnmarshaller = jaxbContext.createUnmarshaller();
            configuration = (Configuration) jaxbUnmarshaller.unmarshal(file);
        } catch (JAXBException e) {
            e.printStackTrace();
        }
    }
    */

    public Configuration getConfiguration(){
        return configuration;
    }

    public static synchronized ConfigurationManager get(){
        if (myConfigurationManager == null){
            myConfigurationManager = new ConfigurationManager();
        }
        return myConfigurationManager;
    }

}