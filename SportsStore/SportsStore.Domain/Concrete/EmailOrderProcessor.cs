using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    { 
        public string MailToAddress = "orders@example.com";
        public string MailFromAddress = "sportsstore@example.com";
        public bool UseSsl = true;
        public string UserName = "MysmtpUserName";
        public string Password = "password";
        public string ServerName = "smtp.example.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"C:\Users\pmc02\Desktop\Asp.net_Practice_Project\OrderInfoEmails";
    }
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        // email 셋팅 설정
        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using(var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password);

                // smtp 서버 없을 경우 디렉토리에 파일로 생성                 
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder()
                    .AppendLine("A new order has been subbitted")
                    .AppendLine("---")
                    .AppendLine("Items");

                foreach(var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    // {2:c} 문화권에 맞는 통화로 변경
                    body.AppendFormat("{0} X {1} (subtotal : {2:c})", line.Quantity, line.Product.Name, subtotal);
                }

                // AppendFormat과 AppendLine의 차이 
                // https://docs.microsoft.com/ko-kr/dotnet/api/system.text.stringbuilder.appendformat?view=net-5.0
                // 문자열 보간 사용 시 AppendFormat 사용  
                body.AppendFormat("Total order value: {0:c}", cart.ComputeTatolValue())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1)
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.State ?? "")
                    .AppendLine(shippingDetails.Country)
                    .AppendLine(shippingDetails.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");


                MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress, emailSettings.MailToAddress);  // 보내는 메일 주소 // 받는 메일 주소 설정 생성
                mailMessage.Subject ="New order submmited";

                // 파일형식으로 저장 시 body UTF-8로 인코딩 
                //var utfbody = Encoding.UTF8.GetBytes(body.ToString());
                // mailMessage.Body = emailSettings.WriteAsFile ? utfbody.ToString() : body.ToString();
                mailMessage.Body = body.ToString();

                // 교재 코드 => 실행되지 않아 수정
                //MailMessage mailMessage = new MailMessage(
                //    emailSettings.MailFromAddress                                           // 보내는 메일 주소 
                //    , emailSettings.MailToAddress                                           // 받는 메일 주소 설정 생성
                //    , "New order submitted!"                                                // 제목
                //    , emailSettings.WriteAsFile ? utfbody.ToString() : body.ToString());    // 본문 (파일 형식으로 저장할시 UTF-8로 변환된 body를 추가함 

                //https://www.codeproject.com/Articles/32434/Adding-Save-functionality-to-System-Net-Mail-MailM  파일 이름 설정할 수 있도록 확장 
                // 파일로 작성시
                if (emailSettings.WriteAsFile)
                {
                    // 요약:
                    //     메시지 본문을 인코딩하는 데 사용 되는 인코딩을 가져오거나 설정 합니다.
                    // 반환 값:
                    //     System.Text.Encoding 의 내용에 적용 되는 System.Net.Mail.MailMessage.Body합니다.
                    // 바디의 내용을 UTF-8형식으로 인코딩
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }
        }
    }
}
