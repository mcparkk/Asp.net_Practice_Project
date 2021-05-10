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
        // 맴버변수는 언더바로 syntax 지키자
        private IProductRepository repository;
        private IOrderProcessor orderProcessor;

        // 생성자를 여러개로 다형성을 지키는것은 안될까? 
        public CartController(IProductRepository repo, IOrderProcessor proc)
        {
            repository = repo;
            orderProcessor = proc;
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

        // [Complete order] 버튼 클릭 시 POST요청을 처리
        // shippingDetails 매개변수는 Http 폼 데이터를 통해서 자동으로 생성 
        // Cart 매개변수는 사용자 지정 모델 바인더를 통해 생성된다. 
        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            // cart 검증 
            if(cart.Lines.Count() == 0)
            {
                // cart가 비어있으면 폼데이터에 에러 추가 => 결론적으로 아래 검증에서 걸러진다.
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            
            // ShippingDetails 검증
            //ModelState.IsValid
            // 요약:
            //     모델 상태 사전의 이 인스턴스가 유효한지를 나타내는 값을 가져옵니다.
            if (ModelState.IsValid)
            {
                orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
        }

    }
}