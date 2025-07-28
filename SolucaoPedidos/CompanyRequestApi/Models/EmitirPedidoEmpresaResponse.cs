namespace CompanyRequestApi.Models
{
    public class EmitirPedidoEmpresaResponse
    {
        public string NumeroPedidoEmpresa { get; set; }
        public List<ItemPedidoEmpresa> ItensSolicitados { get; set; }
    }
}
