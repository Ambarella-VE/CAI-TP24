using Cart.API.Clients;
using Cart.API.DTOs;
using Cart.API.Models;
using Shared.Contracts.Errors;
using CartModel = Cart.API.Models.Cart;
namespace Cart.API.Services;
public class CartService
{
    private readonly List<CartModel> _carts = []; private readonly IProductsClient _products;
    public CartService(IProductsClient products) => _products = products;
    public CartResponse AddItem(Guid userId, AddCartItemRequest request)
    {
        if (request.Cantidad <= 0) throw new DomainException("CRT-004");
        var product = _products.Get(request.ProductoId) ?? throw new DomainException("CRT-002");
        var cart = _carts.FirstOrDefault(item => item.UserId == userId) ?? CreateCart(userId);
        var item = cart.Items.FirstOrDefault(existing => existing.ProductId == request.ProductoId);
        var quantity = (item?.Quantity ?? 0) + request.Cantidad;
        if (quantity > product.Stock) throw new DomainException("CRT-003");
        if (item is null) cart.Items.Add(new CartItem { ProductId = request.ProductoId, Quantity = quantity }); else item.Quantity = quantity;
        cart.UpdatedAt = DateTime.UtcNow; return ToResponse(cart);
    }
    public CartResponse Get(Guid userId) => ToResponse(Find(userId));
    public CartResponse UpdateItem(Guid userId, Guid productId, UpdateCartItemRequest request)
    {
        if (request.Cantidad <= 0) throw new DomainException("CRT-004");
        var product = _products.Get(productId) ?? throw new DomainException("CRT-002");
        if (request.Cantidad > product.Stock) throw new DomainException("CRT-003");
        var cart = Find(userId);
        var item = cart.Items.FirstOrDefault(existing => existing.ProductId == productId) ?? throw new DomainException("CRT-002");
        item.Quantity = request.Cantidad;
        cart.UpdatedAt = DateTime.UtcNow;
        return ToResponse(cart);
    }
    public void DeleteItem(Guid userId, Guid productId)
    {
        var cart = Find(userId);
        var item = cart.Items.FirstOrDefault(existing => existing.ProductId == productId) ?? throw new DomainException("CRT-002");
        cart.Items.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
    }
    public void Clear(Guid userId)
    {
        var cart = Find(userId);
        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
    }
    private CartModel CreateCart(Guid userId) { var cart = new CartModel { UserId = userId, UpdatedAt = DateTime.UtcNow }; _carts.Add(cart); return cart; }
    private CartModel Find(Guid userId) => _carts.FirstOrDefault(item => item.UserId == userId) ?? throw new DomainException("CRT-001");
    private static CartResponse ToResponse(CartModel cart) => new(cart.UserId, cart.Items.Select(item => new CartItemResponse(item.ProductId, item.Quantity)).ToList(), cart.UpdatedAt);
}
