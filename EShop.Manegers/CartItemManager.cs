using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF_Core;
using EF_Core.Models;
namespace EShop.Manegers
{
    public class CartItemManager :BaseManager<CartItem>
    {
       public CartItemManager( EShopContext context ) : base( context ) 
        {

        }
    }
}
