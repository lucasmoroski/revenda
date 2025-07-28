using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class PedidoEmpresaFalhaEmissaoEvent
    {
        public string IdPedidoEmpresa { get; set; }
        public string IdRevenda { get; set; }
        public string IdPedidoClienteOriginal { get; set; }
        public string MotivoFalha { get; set; }
        public string Status { get; set; }
    }
}
