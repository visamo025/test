using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System;
using QRCoder.Core;
using System.Drawing;
using System.IO;
using static WebApplication3.Controllers.ResidentController;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;

namespace WebApplication3.Controllers
{

    public class EmailController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult MYIndex()
        {
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "qrcode.jpg"); // Corrected path
            string msg = sendmail("shrenisaraiya04@gmail.com", "Gatepass Generated", "Thank You!", imagePath);
            ViewBag.msg = msg;
            return View();


        }

        string sendmail(string rec, string sub1, string msg, string imagePath)
        {
            try
            {

                string receiver = rec; // "shrenisaraiya04@gmail.com";
                string subject = sub1;//  "Order";
                string message = msg; // "Thank you for Visiting";
                var senderEmail = new MailAddress("visamo025@gmail.com", "Visamo");
                var receiverEmail = new MailAddress(receiver, "Receiver");
                var password = "zxmkwuapcaimwfau";
                var sub = subject;
                var body = message;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                    {
                        Attachment attachment = new Attachment(imagePath);
                        mess.Attachments.Add(attachment);                       
                    }
                        smtp.Send(mess);
                    
                    return "Sent ";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public class QRModel
        {
            public string Message { get; set; }
        }
        public IActionResult GenerateQrCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateQrCode(QRModel qr)
        {
            if (string.IsNullOrEmpty(qr.Message))
            {
                ModelState.AddModelError("", "Message cannot be empty.");
                return View();
            }
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var uniqueFileName = "qrcode.jpg";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);


            //       var path = Server.MapPath("Upload");
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qr.Message, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    int qrSize = 10; // Reduced size (Default was 20)
                    using (Bitmap bitMap = qrCode.GetGraphic(qrSize))

                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                            bitMap.Save(filePath);
                        }
                    }
                }
            }

            return View();
        }
    }
}



    