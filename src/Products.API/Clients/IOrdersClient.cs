// [TP Microservicios E-Commerce v7] Define la comunicación de Products.API con otro microservicio sin compartir modelos internos.
namespace Products.API.Clients;

public interface IOrdersClient
{
    bool HasActiveOrders(Guid productId);
}
