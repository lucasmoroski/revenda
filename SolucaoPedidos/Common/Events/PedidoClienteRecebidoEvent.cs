using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class PedidoClienteRecebidoEvent
    {
        public string IdPedidoCliente { get; set; }
        public string IdRevenda { get; set; }
        public List<EventItem> Itens { get; set; } = new List<EventItem>();
    }
}
