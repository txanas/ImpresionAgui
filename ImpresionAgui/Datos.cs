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
        [JsonProperty("articulo")]
        public string articulo { get; set; }
        [JsonProperty("cantidad")]
        public string cantidad { get; set; }
        [JsonProperty("lote")]
        public string lote { get; set; }
        [JsonProperty("pedido")]
        public string pedido { get; set; }
        [JsonProperty("albaran")]
        public string albaran { get; set; }
        [JsonProperty("control")]
        public string control { get; set; }
        [JsonProperty("ncajas")]
        public string ncajas { get; set; }
        [JsonProperty("destino")]
        public string destino{ get; set; }
    }
}
