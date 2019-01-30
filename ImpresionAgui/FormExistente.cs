using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

namespace ImpresionAgui
{
    public partial class FormExistente : Form
    {
        private String data;
        public FormExistente()
        {
            InitializeComponent();
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {

            //llamar a la base de datos y recoger los datos
            await buscarenBD();

            if(data != null)
            {
                Form datos = new FormTablaDatos(data);
                this.Hide();
                datos.ShowDialog();
            } else
            {
                Form datos = new FormTablaDatos();
                this.Hide();
                datos.ShowDialog();
            }

        }

        private HttpClient httpClient;

        //Busqueda por fecha: aaaa-mm-dd
        public async Task buscarenBD()
        {
            httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://agui.myruns.com");
            httpClient.BaseAddress = new Uri("http://localhost:800");

            //Articulo
            string fecha = txtFecha.Text;
            string albaran = txtAlbaran.Text;
            string articulo = txtArticulo.Text;

            var values = new Dictionary<string, string>
            {{"Fecha", fecha}, {"Albaran", albaran}, {"Articulo", articulo}};

            var content = new FormUrlEncodedContent(values);
            var response = await httpClient.PostAsync("api/leer_basedatos.php", content);
            //Haciendo echo en pruebas_post recibimos el epc que queremos.
            var contents = await response.Content.ReadAsStringAsync();
            data = contents;
            Console.WriteLine(contents);
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form eleccion = new FormNuevaEtiqueta();
            eleccion.ShowDialog();
        }
    }
    
}

