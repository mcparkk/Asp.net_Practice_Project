using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using System.Web.Mvc;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Concrete;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBinding();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBinding()
        {

            kernel.Bind<IProductRepository>().To<EFProductRepository>();
            
            
            // 바인딩 정보
            //Mock<IProductRepository> mock = new Mock<IProductRepository>();
            //mock.Setup(x => x.Products).Returns(new List<Product>
            //{
            //    new Product {Name = "Football", Price = 25000}
            //   ,new Product {Name = "Surf board", Price = 179000}
            //   ,new Product {Name = "Running shoes", Price = 95000}
            //});

            // mock object를 싱글톤으로 사용 -> 단일 인스턴스 
            //kernel.Bind<IProductRepository>().ToConstant(mock.Object);
        }
    }
}