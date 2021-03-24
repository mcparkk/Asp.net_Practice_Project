using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace SportsStore.WebUI.Infrastructure.Binders
{
    // IModelBinding 인터페이스!!
    
    // => system.Web.mvc에 있는 IModelBinder와 
    //반환 값:
    //바인딩된 값입니다.

    // => System.Web.ModelBinding에 있는 IModelBinder와 혼용 금지!! 
    //반환 값:
    //true 모델 바인딩에 성공 하면 그렇지 않으면 false합니다.

    public class CartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";

        public object BindModel(ControllerContext controllercontext, ModelBindingContext bindingcontext)
        {
            // 세션에서 Cart 개체 가져오기 
            Cart cart = null;
            if(controllercontext.HttpContext.Session != null)
                cart = (Cart)controllercontext.HttpContext.Session[sessionKey];
            
            // 세션 데이터에 Cart 개체가 없다면 새로 생성한다.
            if(cart == null)
            {
                cart = new Cart();
                if(controllercontext.HttpContext.Session != null)
                    controllercontext.HttpContext.Session[sessionKey] = cart;
            }

            return cart;
        }
    }
}