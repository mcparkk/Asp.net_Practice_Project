using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Infrastructure.Binders;

namespace SportsStore.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //Global.asax 파일은 예전 ASP에서 사용되었던 global.asa 파일에서 사용되었던 내용을
        //.NET 환경에서 사용할 수 있도록 만든 파일입니다.
        //Global 이란 뜻처럼, 전역 데이터를 관리할 수 있을뿐 아니라 웹 사이트의 시작과 종료,
        //새로운 사용자의 접속 시도 및 접속 종료시 등 여러가지 프로그래밍 코드를 작성할 수 있는 곳입니다
        //출처: https://im-first-rate.tistory.com/12 

        // 애플리케이션 시작점
        // winform에 Main과 같은 역할 
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Cart 인스턴스 생성 시 CartModelBinder을 사용하도록 지시
            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());
        }
    }
}
