using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace ImpresionAgui
{
    class Datos
    {
        [JsonProperty("FECHA")]
        public string FECHA { get; set; }
        [JsonProperty("ARTICULO")]
        public string ARTICULO { get; set; }
        [JsonProperty("CANTIDAD")]
        public string CANTIDAD { get; set; }
        [JsonProperty("LOTE")]
        public string LOTE { get; set; }
        [JsonProperty("PEDIDO")]
        public string PEDIDO { get; set; }
        [JsonProperty("LINEA")]
        public string LINEA { get; set; }
        [JsonProperty("ALBARAN")]
        public string ALBARAN { get; set; }
        [JsonProperty("NCAJAS")]
        public string NCAJAS { get; set; }
        [JsonProperty("EPC")]
        public string EPC { get; set; }
        //[JsonProperty("destino")]
        //public string destino{ get; set; }
    }
}
