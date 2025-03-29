using EF_Core.Models;
using EShop.Manegers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Presentation.Controllers
{
    //[Authorize (Roles ="Client")]
    public class CartItemController : Controller
    {
        private CartItemManager CardManager;

        public CartItemController (CartItemManager _cardManager)
        {
            CardManager = _cardManager;
        }

        public IActionResult Index()
        {
            var list = CardManager.Get().ToList();
            return View(list);
        }
       

        [HttpPost]
        public IActionResult Add(int ProductId)
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var cartItem = new CartItem
            {
                ProductID = ProductId,
                Quantity = 1,
                ClientId= userId
            };

                CardManager.Add(cartItem);
            return RedirectToAction("Index", "Product");
        }
    }
}
