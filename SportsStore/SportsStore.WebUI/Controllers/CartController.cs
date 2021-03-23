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

        public ViewResult Index(string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = GetCart(),
                ReturnUrl = returnUrl
            });
        }

        public RedirectToRouteResult AddToCart(int productId, string returnUrl)
        {
            Product product = repository.Products.FirstOrDefault(x => x.ProductID == productId);

            // return형에 따른 메소드 사용
            if(product != null)
                GetCart().AddItems(product, 1);

            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(int productId, string returnUtl)
        {
            Product product = repository.Products.FirstOrDefault(x => x.ProductID == productId);

            if (product != null)
                GetCart().RemoveLine(product);

            return RedirectToAction("Index", returnUtl);
        }

        private Cart GetCart()
        {
            // 싱글톤 
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
                cart = new Cart();
            Session["Cart"] = cart;

            return cart;
        }
    }
}