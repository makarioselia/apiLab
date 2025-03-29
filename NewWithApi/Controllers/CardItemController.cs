using EF_Core.Models;
using EShop.Manegers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace EShop.Presentation.Controllers
{
    public class CartItemDto
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SupPrice { get; set; }
    }
    [ApiController]
    [Route("api/{Controller}")]
    public class CartItemController : Controller
    {
        private CartItemManager CardManager;
        public CartItemController(CartItemManager _cardManager)
        {
            CardManager = _cardManager;
        }

        [HttpGet("Index")]
        //[Authorize(Roles = "client")]
        public IActionResult Index()
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var list = CardManager.GetList(c => c.ClientId == userId)
             .Select(c => new CartItemDto
             {
                 ProductName = c.Product.Name,
                 Price = c.Product.Price,
                 Quantity = c.Quantity
             }).ToList();

            return Ok(new { data = list });
        }

        [HttpPost("Add/{ProductId}")]
        [Authorize(Roles = "client")]
        public IActionResult Add([FromRoute] int ProductId)
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var cartItem = new CartItem
            {
                ProductID = ProductId,
                Quantity = 1,
                ClientId = userId,
                SupPrice = 1100
            };

            CardManager.Add(cartItem);
            return RedirectToAction("Index", "CardItem");
        }

        [HttpPost("delet/{ProductId}")]
        [Authorize(Roles = "client")]
        public IActionResult Delet(int id)
        {
            var item = CardManager.GetList(a => a.Id == id).ToList()[0];
            CardManager.Delete(item);
            return Ok(new { message = "Item deleted successfully" });

        }
    }
}
