using Orders.API.Models;

namespace Orders.API.Services;

public static class OrderStateTransitionPolicy
{
    public static bool CanChange(OrderStatus current, OrderStatus next) => current switch
    {
        OrderStatus.Pending => next is OrderStatus.Confirmed or OrderStatus.Cancelled,
        OrderStatus.Confirmed => next is OrderStatus.Shipped or OrderStatus.Cancelled,
        OrderStatus.Shipped => next == OrderStatus.Delivered,
        _ => false
    };
}
