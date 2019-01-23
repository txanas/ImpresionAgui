using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImpresionAgui
{
    public partial class FormEleccion : Form
    {
        public FormEleccion()
        {
            InitializeComponent();
        }

        private void btnNueva_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form nueva = new FormNuevaEtiqueta();
            nueva.ShowDialog();
        }

        private void btnExistente_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form existente = new FormExistente();
            existente.ShowDialog();
        }
    }
}
