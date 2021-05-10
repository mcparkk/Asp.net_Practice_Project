using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart

        private IProductRepository repository;

        public CartController(IProductRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                //Cart = GetCart(),             
                //=> session에 있는 개체가 아니라 IModelBinder을 사용하여 만들어진 인스턴스를 사용 
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {
            Product product = repository.Products.FirstOrDefault(x => x.ProductID == productId);

            // return형에 따른 메소드 사용
            if(product != null)
                cart.AddItems(product, 1);

            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            Product product = repository.Products.FirstOrDefault(x => x.ProductID == productId);

            if (product != null)
                cart.RemoveLine(product);

            return RedirectToAction("Index", new { returnUrl });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        private Cart GetCart()
        {
            // singleton 
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
                cart = new Cart();
            Session["Cart"] = cart;

            return cart;
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }


    }
}