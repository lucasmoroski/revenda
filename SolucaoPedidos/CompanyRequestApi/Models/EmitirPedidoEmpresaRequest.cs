using Common.Events;
using Common.Models;

namespace CompanyRequestApi.Models
{
    public class EmitirPedidoEmpresaRequest
    {
        public string IdRevenda { get; set; }
        public List<EventItem> Itens { get; set; }
    }
}
