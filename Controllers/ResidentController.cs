using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using WebApplication3.Models;
using QRCoder.Core;
using System.Drawing;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static WebApplication3.Controllers.EmailController;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using Razorpay.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing.Imaging;

namespace WebApplication3.Controllers
{
    [SessionCheck]
    public class ResidentController : Controller
    {
        
        private readonly visamodbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ResidentController(IWebHostEnvironment webHostEnvironment, visamodbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }        

        // GET: ResidentController
        public ActionResult Index()
        {
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

        // GET: ResidentController            
        public ActionResult GatepassCreate()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email);
                ViewBag.WingNo = HttpContext.Session.GetString("wingno");
                ViewBag.FlatNo = HttpContext.Session.GetString("flatno");
            }
            return View();
        }
        [HttpPost]
        public ActionResult GatepassCreate(Gatepass g)
        {
            try
            {
                string email = HttpContext.Session.GetString("email");
                Users loggedInUser = _context.Users.FirstOrDefault(u => u.Email == email);

                // Assign logged-in user's details
                g.Uid = loggedInUser.Uid;
                g.Wingno = loggedInUser.Wingno;
                g.Flatno = loggedInUser.Flatno;
                g.Date = DateTime.Today;                
                _context.Gatepass.Add(g);
                _context.SaveChanges();
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "qrcode.jpg"); // Corrected path
                GenerateQrCode(g.Visitorname + " " + g.Npeople + " " + g.Date + " "+g.Type, imagePath);
                string msg = sendmail(g.Email, "Gatepass Generated", "Thank You!" , imagePath);
                ViewBag.msg = msg;                
                return RedirectToAction(nameof(ViewGatepass));                
            }
            catch
            {
                return View();
            }
        }
        public ActionResult GatepassEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Gatepass g = _context.Gatepass.Find(id); 
            return View(g);
        }
        [HttpPost]
        public ActionResult GatepassEdit(Gatepass g, Users u)
        {
            try
            {
                Users uu = _context.Users.Where(u => u.Rid == 19).ToList().First();
                g.Uid = uu.Uid;
                _context.Gatepass.Update(g);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewGatepass));
            }
            catch
            {
                return View();
            }
        }       
        public ActionResult ViewGatepass()
        {
            
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            string email = HttpContext.Session.GetString("email");
            Users loggedInUser = _context.Users.FirstOrDefault(u => u.Email == email);


            var Gatepass = _context.Gatepass
                .Where(g => g.Uid == loggedInUser.Uid)
                .ToList();
            return View(Gatepass);
        }
        public ActionResult DeleteGatepass(int id)
        {
            try
            {
                Gatepass g = _context.Gatepass.Find(id);
                _context.Gatepass.Remove(g);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewGatepass));
            }
            catch
            {
                return View();
            }

        }
        public IActionResult GenerateQrCode()
        {
            return View();
        }
            void GenerateQrCode(string msg,string imgPath)
        {
            //if (string.IsNullOrEmpty(g.Visitorname))
            //{
            //    ModelState.AddModelError("", "Message cannot be empty.");
            //  //  return View();
            //}
            //var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            //var uniqueFileName = "qrcode.jpg";
            //var filePath = Path.Combine(uploadsFolder, uniqueFileName);


            //       var path = Server.MapPath("Upload");
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(msg, QRCodeGenerator.ECCLevel.Q);
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
                            bitMap.Save(imgPath);
                        }
                    }
                }
            }

            //   return View();
        }

        public ActionResult qrDetails(int id)
        {
            Gatepass g = _context.Gatepass.Find(id);

            TempData["user"] = HttpContext.Session.GetString("name");
            TempData["photo"] = HttpContext.Session.GetString("photo");
            TempData["email"] = HttpContext.Session.GetString("email");

            // QR Code Data (e.g., ID ya koi aur unique identifier)
            string qrData = $"  Gatepass Name: {g.Visitorname} ,Flatno :{g.Flatno},Wing no:{g.Wingno}, Type: {g.Type}, EntryTime: {g.Date}";

            // QR Code Generate Karna
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q))
                {
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        using (Bitmap qrBitmap = qrCode.GetGraphic(10)) // 10 = pixel size
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                qrBitmap.Save(ms, ImageFormat.Png);
                                string qrCodeBase64 = Convert.ToBase64String(ms.ToArray());
                                ViewBag.QRCodeImage = "data:image/png;base64," + qrCodeBase64;
                            }
                        }
                    }
                }
            }

            return View(g);
        }

        // GET: ResidentController
        //public ActionResult Maintenance()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Maintenance(Income i)
        //{
        //    try
        //    {
        //        Users uu = _context.Users.FirstOrDefault(u => u.Rid == 19);
        //        i.Uid = uu.Uid;
        //        i.Date = DateTime.Now;
        //        i.Status = "Done";
        //        _context.Income.Add(i);
        //        _context.SaveChanges();
        //        return RedirectToAction("ViewMaintenance", i);
        //    }
        //    catch            {
        //        return View();
        //    }           
        //}
        public ActionResult PayView(int Id)
        {
            Income i = _context.Income.Find(Id);
           
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount",i.Amount*100); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", Id.ToString());

            string key = "rzp_test_cZd0r0o9mHqKDn";
            string secret = "kMaOmbFyEvYX2h4c7s9WzHbp";

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.orderId = order["id"].ToString();
            ViewBag.incomeId = Id; // Pass Income ID to the View
            return View("PayView", i);
        }
        [HttpPost]
        public ActionResult PayView(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {            
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", razorpay_payment_id); // this amount should be same as transaction amount
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);
            try
            {
                Utils.verifyPaymentSignature(attributes);

                // Extract the Income ID from the Razorpay Order object
                RazorpayClient client = new RazorpayClient("rzp_test_cZd0r0o9mHqKDn", "kMaOmbFyEvYX2h4c7s9WzHbp");
                Razorpay.Api.Order order = client.Order.Fetch(razorpay_order_id);
                int incomeId = int.Parse(order["receipt"].ToString());  // Extract Income ID

                // Find the payment record and update the status
                Income i = _context.Income.Find(incomeId);
                if (i != null)
                {
                    i.Status = "Paid"; // Update status
                    _context.SaveChanges();
                }

                return View("success");
            }
            catch (Exception ex)
            {
                return View("failure");

            }

            //return View();
        }
        public ActionResult Success(Income i)
        {
            //i.Status = "Done";
            return View();
        }

        public ActionResult Failure(Income i)
        {
            i.Status = "Fail";
            return View();
        }

        public ActionResult MaintenanceEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Income i = _context.Income.Find(id);
            return View(i);
        }
        [HttpPost]
        public ActionResult MaintenanceEdit(Income i, Users u)
        {
            try
            {
                Users uu = _context.Users.Where(u => u.Rid == 19).ToList().First();
                i.Uid = uu.Uid;
                i.Status = "Done";
                _context.Income.Update(i);
                _context.SaveChanges();
                return RedirectToAction("PayView", i);
            }
            catch
            {
                return View();
            }
        }
        public ActionResult ViewMaintenance()
        {
            // Store session data in TempData (for passing to View)
            TempData["user"] = HttpContext.Session.GetString("name");
            TempData["photo"] = HttpContext.Session.GetString("photo");
            TempData["email"] = HttpContext.Session.GetString("email");
            TempData["wingno"] = HttpContext.Session.GetString("wingno");
            TempData["flatno"] = HttpContext.Session.GetString("flatno");
             
            using (var db = new visamodbContext()) // Ensure this is your actual DbContext
            {
                string flatno = HttpContext.Session.GetString("flatno");
                string wingno = HttpContext.Session.GetString("wingno");
                //string flatno = HttpContext.Session.GetString("flatno");
                Users loggedInUsers = _context.Users.FirstOrDefault(u => u.Wingno == wingno && u.Flatno == flatno);
                // Fetch maintenance data specific to the logged-in resident
                var maintenanceData = db.Income 
                                        .Where(i => i.Wingno == wingno && i.Flatno == flatno ) // Corrected filter
                                        .Where(i => i.Description.Contains("maintenance"))
                                        .ToList();

                return View(maintenanceData);
            }
        }
        //Card No
        //5267 3181 8797  5449        

        public ActionResult Service()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)
                TempData["wingno"] = HttpContext.Session.GetString("wingno");
                TempData["flatno"] = HttpContext.Session.GetString("flatno");
                ViewBag.WingNo = HttpContext.Session.GetString("WingNo");
                ViewBag.FlatNo = HttpContext.Session.GetString("FlatNo");

            }
            return View();
        }
        [HttpPost]
        public ActionResult Service(Service s)
        {
            try
            {
                string email = HttpContext.Session.GetString("email");
                Users loggedInUser = _context.Users.FirstOrDefault(u => u.Email == email);

                // Assign logged-in user's details
                s.Uid = loggedInUser.Uid;
                s.Wingno = loggedInUser.Wingno;
                s.Flatno = loggedInUser.Flatno;
                s.Completedatetime = DateTime.Now;
                s.Requestdatetime = DateTime.Now;
                s.Nameofworker = "Raju";
                _context.Service.Add(s);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewService));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult ServiceEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Service s = _context.Service.Find(id);
            return View(s);
        }
        [HttpPost]
        public ActionResult ServiceEdit(Service s)
        {
            try
            {
                Users uu = _context.Users.Where(u => u.Uid == 19).ToList().First();
                s.Uid = uu.Uid;
                _context.Service.Update(s);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewService));
            }
            catch
            {
                return View();
            }
        }

      
        public ActionResult DeleteService(int id)
        {
            try
            {
                Service s = _context.Service.Find(id);
                _context.Service.Remove(s);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewService));
            }
            catch
            {
                return View();
            }

        }
        public ActionResult ViewService()
        {
            
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
             string email = HttpContext.Session.GetString("email");
            Users loggedInUser = _context.Users.FirstOrDefault(u => u.Email == email);


            var service = _context.Service
                .Where(s => s.Uid == loggedInUser.Uid)
               .OrderBy(s => s.Requestdatetime == null)  // Unresolved services first
    .ThenByDescending(s => s.Completedatetime) // Then sort by complaint date
                .ToList();
            return View(service);
           
        }

        public ActionResult Viewnotice()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            var notices = _context.Notice.OrderByDescending(n => n.Nid).ToList();
            return View(notices);
        }
        public ActionResult Eventname()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)

                Event model = new Event
                {
                    Amount = 6000,  // Set the fixed amount
                    Bookdate = DateTime.Today
                };
            }
            return View();
        }
        [HttpPost]

            public ActionResult Eventname(Event e)
        {
            try
            {
                string email = HttpContext.Session.GetString("email");
                Users loggedInUser = _context.Users.FirstOrDefault(u => u.Email == email);

                if (loggedInUser == null)
                {
                    return RedirectToAction("Login"); // Redirect if user is not logged in
                }

                // Check if an event is already booked on the selected date
                bool isEventBooked = _context.Event.Any(ev => ev.Bookdate == e.Bookdate);

                if (isEventBooked)
                {
                    TempData["ErrorMessage"] = "An event is already booked on this date. Please choose another date.";
                    return View(e);
                }

                // Assign logged-in user's details
                e.Uid = loggedInUser.Uid;
                e.Reqdate = DateTime.Today;
                e.Amount = 6000;

                // Ensure Ndays is valid (should be at least 1)
                if (e.Ndays < 1)
                {
                    TempData["ErrorMessage"] = "Number of days must be at least 1.";
                    return View(e);
                }

                // Calculate Amount based on Ndays
                int fixedAmountPerDay = 6000; // Example fixed price per day
                e.Amount = fixedAmountPerDay * e.Ndays;

                _context.Event.Add(e);
                _context.SaveChanges();

                return RedirectToAction("Payevent", e);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult PayEvent(Event e)
        {
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", e.Amount * 100); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", "12121");

            string key = "rzp_test_cZd0r0o9mHqKDn";
            string secret = "kMaOmbFyEvYX2h4c7s9WzHbp";

            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.orderId = order["id"].ToString();
            return View("PayEvent", e);
        }
        [HttpPost]
        public ActionResult PayEvent(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)

        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", razorpay_payment_id); // this amount should be same as transaction amount
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);
            try
            {
                Utils.verifyPaymentSignature(attributes);
                return View("success");
            }
            catch (Exception ex)
            {
                return View("failure");

            }

            //return View();
        }
        public ActionResult success(Event e)
        {           
            return View();
        }

        public ActionResult failure(Event e)
        {           
            return View();
        }

        public ActionResult EventEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Event e = _context.Event.Find(id);
            return View(e);
        }
        [HttpPost]
        public ActionResult EventEdit(Event e)
        {
            try
            {
                Users uu = _context.Users.Where(u => u.Uid == 19).ToList().First();
                e.Uid = uu.Uid;               
                _context.Event.Update(e);
                _context.SaveChanges();
                return RedirectToAction("PayEvent", e);
            }
            catch
            {
                return View();
            }
        }
        public ActionResult DeleteEvent(int id)
        {
            try
            {
                Event e = _context.Event.Find(id);
                _context.Event.Remove(e);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewEvent));
            }
            catch
            {
                return View();
            }

        }
        public ActionResult ViewEvent()
        {
            
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)
                  string email = HttpContext.Session.GetString("email");
            Users loggedInUser = _context.Users.FirstOrDefault(u => u.Email == email);


            var Event = _context.Event
                .Where(e => e.Uid == loggedInUser.Uid)
                .ToList();
            return View(Event);
           
        }
      
        public ActionResult Complain()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }
        [HttpPost]
        public ActionResult Complain(Complain c)
        {
            try
            {
                string email = HttpContext.Session.GetString("email");
                Users loggedInUser = _context.Users.FirstOrDefault(u => u.Email == email);
                // Assign logged-in user's details
                c.Uid = loggedInUser.Uid;
                c.Status = "Pending";
                c.Cdate = DateTime.Now;
                c.Rdate = DateTime.Now;
                _context.Complain.Add(c);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewComplain));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult ComplainEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Complain c = _context.Complain.Find(id);
            return View(c);
        }
        [HttpPost]
        public ActionResult ComplainEdit(Complain c)
        {
            try
            {
                Users uu = _context.Users.Where(u => u.Uid == 29).ToList().First();
                c.Uid = uu.Uid;
                _context.Complain.Update(c);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewComplain));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult DeleteComplain(int id)
        {
            try
            {
                Complain c = _context.Complain.Find(id);
                _context.Complain.Remove(c);
                _context.SaveChanges();
                return RedirectToAction(nameof(ViewComplain));
            }
            catch
            {
                return View();
            }

        }
        public ActionResult ViewComplain()
        {
            
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            string email = HttpContext.Session.GetString("email");
            Users loggedInUser = _context.Users.FirstOrDefault(u => u.Email == email);


            var Complain = _context.Complain
                .Where(C => C.Uid == loggedInUser.Uid)
                .OrderByDescending(c => c.Cdate)
                .ToList();
            return View(Complain);

        }
       

        // GET: ResidentController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: residentController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: residentController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: residentController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: residentController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: residentController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: residentController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
