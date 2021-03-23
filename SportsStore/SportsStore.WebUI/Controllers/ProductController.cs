using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {

        // GET: Product

        private IProductRepository repository;
        public int pageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            this.repository = productRepository;
        }

        //public ViewResult List()
        //{
        //    int countOfObject = repository.Products.Count();
        //    return View(repository.Products);
        //}

        public ViewResult List(string category, int page = 1)
        {
            //return View(repository.Products.OrderBy(x => x.ProductID).Skip((page - 1) * pageSize).Take(pageSize));

            // Product 인스턴스랑 model 인스턴스를 같이 모델로 보낸다.
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = category == null ? repository.Products.OrderBy(x => x.ProductID).Skip((page - 1) * pageSize).Take(pageSize)
                : repository.Products.Where(x => x.Category == category).OrderBy(x => x.ProductID).Skip((page - 1) * pageSize).Take(pageSize),
                //Products = repository.Products.Where(x => x.Category == null || x.Category == category).OrderBy(x => x.ProductID).Skip((page - 1) * pageSize).Take(pageSize),
                
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = category == null? repository.Products.Count() : repository.Products.Where(x=>x.Category == category).Count()
                },
                
                CurrentCategory = category
               
            };
            // View(object)
            return View(model);
        }
        
    }
}