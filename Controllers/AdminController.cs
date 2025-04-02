using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;


//using WebApplication1.Models;
using WebApplication3.Models;
using System.Text;

namespace WebApplication3.Controllers
{
    [SessionCheck]
    public class AdminController : Controller
    {
        private readonly visamodbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;



        // GET: AdminController
        public AdminController(IWebHostEnvironment webHostEnvironment, visamodbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        private static readonly string key = "zxmkwuapcaimwfau"; // Keep it secret

        public static string Encrypt(string text)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 16));
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                {
                    byte[] textBytes = Encoding.UTF8.GetBytes(text);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);
                    byte[] combined = new byte[iv.Length + encryptedBytes.Length];
                    Array.Copy(iv, 0, combined, 0, iv.Length);
                    Array.Copy(encryptedBytes, 0, combined, iv.Length, encryptedBytes.Length);
                    return Convert.ToBase64String(combined);
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] combined = Convert.FromBase64String(encryptedText);
            byte[] iv = new byte[16];
            byte[] encryptedBytes = new byte[combined.Length - 16];

            Array.Copy(combined, 0, iv, 0, iv.Length);
            Array.Copy(combined, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 16));
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult Login()
        { 
            return View();
        }
        [HttpPost]
        public ActionResult Login(Users u)
        {
            Users t = _context.Users
                              .Where(U1 => U1.Email == u.Email && U1.Password == u.Password)
                              .FirstOrDefault();

            if (t != null)
            {
                HttpContext.Session.SetString("name", t.Name ?? "Unknown");
                HttpContext.Session.SetString("photo", t.Photo ?? "default.png");
                HttpContext.Session.SetString("email", t.Email ?? "");
                HttpContext.Session.SetString("wingno", t.Wingno ?? "N/A");
                HttpContext.Session.SetString("flatno", t.Flatno ?? "N/A");

                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                Response.Cookies.Append("name", t.Name, cookieOptions);
                Response.Cookies.Append("photo", t.Photo, cookieOptions);
                Response.Cookies.Append("email", t.Email, cookieOptions);

                // Role-based redirection
                return t.Rid switch
                {
                    16 => RedirectToAction("AdminDashboard"),
                    19 => RedirectToAction("ResidentDashboard"),
                    17 => RedirectToAction("SecretaryDashboard"),
                    _ => RedirectToAction("SecurityDashboard"),
                };
            }
            else
            {
                ViewBag.Message = "Invalid username or password";
                return View("Login");
            }
        }

        public ActionResult Logout()
            {

                HttpContext.Session.Clear();
                HttpContext.Session.SetString("name", "");
                HttpContext.Session.SetString("photo", "");//, t.Photo);
                HttpContext.Session.SetString("email", "");

                Response.Cookies.Delete("name");
                Response.Cookies.Delete("photo");
                Response.Cookies.Delete("email");

                return RedirectToAction("Login");
            }

        public ActionResult forgotpassword()
        { 
         return View(); 
        }
        [HttpPost]
        public ActionResult forgotpassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "Please enter a valid email address.";
                return View();
            }

            using (var db = new visamodbContext())
            {
                // Find the user by email
                var user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    ViewBag.Message = "No account found with this email.";
                    return View();
                }

                // Generate encrypted token using the email
                string encryptedToken = AdminController.Encrypt(email);

                // Generate reset link with encrypted token
                string resetLink = Url.Action("resetpassword", "Admin",
                              new { token = encryptedToken },
                              _httpContextAccessor.HttpContext.Request.Scheme);

                // Send reset link via email
                string subject = "Password Reset Request";
                string message = $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>";

                string response = sendmail(email, subject, message, null);

                ViewBag.Message = response.Contains("Sent") ?
                                  "Password reset link has been sent to your email." :
                                  "Error sending email.";
            }

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
        public ActionResult resetpassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            // Pass the token to the view so the form can use it
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public ActionResult resetpassword(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                ViewBag.Message = "Invalid token or empty password.";
                return View();
            }

            using (var db = new visamodbContext())  // Replace 'YourDbContext' with your actual DB context
            {
                // 🔓 Step 1: Decrypt the token to get the user's email
                string email = Decrypt(token);  // Use your decryption function

                // 🔍 Step 2: Find user by email
                var user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    ViewBag.Message = "Invalid or expired token.";
                    return View();
                }

                // 🔑 Step 3: Update the password
                user.Password = newPassword; // Ideally, hash the password before saving
                db.SaveChanges();

                ViewBag.Message = "Your password has been reset successfully!";
            }

            return RedirectToAction("Login");  // Redirect to login after password reset
        }


        public ActionResult AdminDashboard()
        {                       
                TempData["user"] = HttpContext.Session.GetString("name");//, t.Email);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");

            // Fetch counts for different user roles
            var resident = _context.Users.Where(u => u.Rid == 19).ToList().Count();
            ViewData["resident"] = resident;
            var Secretary = _context.Users.Where(u => u.Rid == 17).ToList().Count();
            ViewData["Secretary"] = Secretary;
            var Security = _context.Users.Where(u => u.Rid == 18).ToList().Count();
            ViewData["Security"] = Security;
            var complain = _context.Complain.ToList().Count();
            ViewData["complain"] = complain;
            var Event = _context.Event.ToList().Count();
            ViewData["Event"] = Event;
            var Notice = _context.Notice.ToList().Count();
            ViewData["Notice"] = Notice;
            var latestNotices = _context.Notice.OrderByDescending(n => n.Details).Take(3).ToList();
            var latestEvents = _context.Event.OrderByDescending(e => e.Eventname).Take(3).ToList();
            var latestvisitor = _context.Visitor.OrderByDescending(v => v.Datetime).Take(3).ToList();

            ViewData["LatestNotices"] = latestNotices;
            ViewData["LatestEvents"] = latestEvents;
            ViewData["LiveVisitorLogs"] = latestvisitor;

            return View();            
        }
        public ActionResult ResidentDashboard()
        {
            TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
            TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
            TempData["email"] = HttpContext.Session.GetString("email");//, t.email)

            var userEmail = HttpContext.Session.GetString("email");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login"); // Redirect to login if no session exists
            }

            // Get UserId from the database using the email
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login"); // Redirect if user not found
            }
             int userId = user.Uid; // Assuming 'UserId' is the primary key

            // Fetch counts based on logged-in user's data
            ViewData["complain"] = _context.Complain.Where(c => c.Uid == userId).Count();
            ViewData["Event"] = _context.Event.Where(e => e.Uid == userId).Count();
            ViewData["Gatepass"] = _context.Gatepass.Where(g => g.Uid == userId).Count();
            ViewData["maintance"] = _context.Income.Where(m => m.Uid == userId).Count();
            ViewData["notice"] = _context.Notice.Where(n => n.Uid == userId).Count();
            ViewData["service"] = _context.Service.Where(s => s.Uid == userId).Count();
            ViewData["latestcomplain"] = _context.Complain.Where(c => c.Uid == userId).OrderByDescending(c => c.Details).Take(3).ToList();
            var latestgatepass = _context.Gatepass.Where(g => g.Uid == userId).OrderByDescending(g => g.Date).Take(3).ToList();
            ViewData["latestgatepass"] = latestgatepass;
            var latestNotices = _context.Notice.OrderByDescending(n => n.Details).Take(3).ToList();
            ViewData["LatestNotices"] = latestNotices;
            var latestEvents = _context.Event.OrderByDescending(e => e.Eventname).Take(3).ToList();
            ViewData["LatestEvents"] = latestEvents;

            return View();
        }
        public ActionResult SecretaryDashboard()
        {
            TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
            TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
            TempData["email"] = HttpContext.Session.GetString("email");//, t.email)

            var Event = _context.Event.ToList().Count();
            ViewData["Event"] = Event;
            var notice = _context.Notice.ToList().Count();
            ViewData["notice"] = notice;
            var complain = _context.Complain.ToList().Count();
            ViewData["complain"] = complain;
            var LatestIncome = _context.Income.OrderByDescending(i => i.Date).Take(3).ToList();
            ViewData["LatestIncome"] = LatestIncome;
         
            return View();
        }
        public ActionResult SecurityDashboard()
        {
            TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
            TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
            TempData["email"] = HttpContext.Session.GetString("email");//, t.email)
                                                                       // Get the logged-in user's email from the session
            var userEmail = HttpContext.Session.GetString("email");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login"); // Redirect to login if no session exists
            }

            // Get Uid from the database using the email
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login"); // Redirect if user not found
            }
            int userId = user.Uid; // Assuming 'Uid' is the primary key

            // Update only the requested fields to be user-specific
            var complain = _context.Complain.Where(c => c.Uid == userId).Count();
            ViewData["complain"] = complain;

            var Gatepass = _context.Gatepass.Where(g => g.Uid == userId).Count();
            ViewData["Gatepass"] = Gatepass;

            var service = _context.Service.Where(s => s.Uid == userId).Count();
            ViewData["service"] = service;

            var visitor = _context.Visitor.Where(v => v.Uid == userId).Count();
            ViewData["visitor"] = visitor;
            var latestNotices = _context.Notice.OrderByDescending(n => n.Details).Take(3).ToList();
            ViewData["LatestNotices"] = latestNotices;
            var latestvisitor = _context.Visitor.OrderByDescending(v => v.Vname).Take(3).ToList();
            ViewData["latestvisitor"] = latestvisitor;



            return View();
        }
        public ActionResult ResidentCreate()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)
                return View();
            }
        }
        [HttpPost]
        // GET: AdminController/Create
        public ActionResult ResidentCreate(Users u, IFormFile file)

        {
            if (ModelState.IsValid)
            {
                if (file.Name != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var uniqueFileName = file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    u.Photo = uniqueFileName;

                }
                Roles rr = _context.Roles.Where(r => r.Roles1 == "Resident").ToList().First();
                u.Rid = rr.Rid;
                _context.Add(u);
                _context.SaveChanges();
                return RedirectToAction(nameof(ResidentView));
            }
            return View(u);
        }


        public ActionResult ResidentView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Users.Where(u => u.Rid == 19).ToList());
        }

        public ActionResult SecretaryCreate()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }

        [HttpPost]
        // GET: AdminController/Create
        public ActionResult SecretaryCreate(Users u, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file.Name != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var uniqueFileName = file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    u.Photo = uniqueFileName;

                }
                Roles rr = _context.Roles.Where(r => r.Roles1 == "Secretary").ToList().First();
                u.Rid = rr.Rid;
                _context.Add(u);
                _context.SaveChanges();
                return RedirectToAction(nameof(SecretaryView));
            }
            return View(u);
        }
        public ActionResult SecretaryView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Users.Where(u => u.Rid == 17).ToList());
        }



        public ActionResult SecurityCreate()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }
        [HttpPost]
        public ActionResult SecurityCreate(Users u, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file.Name != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var uniqueFileName = file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    u.Photo = uniqueFileName;

                }
                Roles rr = _context.Roles.Where(r => r.Roles1 == "Security").ToList().First();
                u.Rid = rr.Rid;
                _context.Add(u);
                _context.SaveChanges();
                return RedirectToAction(nameof(SecurityView));
            }
            return View(u);
        }
        public ActionResult SecurityView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }

            return View(_context.Users.Where(u=>u.Rid==18).ToList());
        }
        public ActionResult VisitorView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Visitor.ToList());
        }
        public ActionResult Event()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Event.ToList());
        }
        public ActionResult NoticeView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Notice.ToList());
        }

        public ActionResult IncomeView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Income.ToList());
        }

        public ActionResult ExpenseView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Expense.ToList());
        }

        public ActionResult ComplainView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Complain.ToList());
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult ResidentEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Users u = _context.Users.Find(id);
            return View(u);
        }
        // post :admincontroller/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
            public ActionResult ResidentEdit(Users u, IFormFile file)
        {
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var uniqueFileName = file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    u.Photo = uniqueFileName;

                }
                Roles rr = _context.Roles.Where(r => r.Roles1 == "Resident").ToList().First();
                u.Rid = rr.Rid;
                _context.Update (u);
                
                _context.SaveChanges();
                return RedirectToAction(nameof(ResidentView));
            }
            return View(u);
        }
        // GET: AdminController/Edit/5
        public ActionResult SecretaryEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Users u = _context.Users.Find(id);
            return View(u);
        }
        // post :admincontroller/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecretaryEdit(Users u, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var uniqueFileName = file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    u.Photo = uniqueFileName;

                }
                Roles rr = _context.Roles.Where(r => r.Roles1 == "Secretary").ToList().First();
                u.Rid = rr.Rid;
                _context.Update(u);
                _context.SaveChanges();
                return RedirectToAction(nameof(SecretaryView));
            }
            return View(u);
        }
        // GET: AdminController/Edit/5
        public ActionResult SecurityEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Users u = _context.Users.Find(id);
            return View(u);
        }
        // post :admincontroller/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecurityEdit(Users u, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var uniqueFileName = file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    u.Photo = uniqueFileName;

                }
                Roles rr = _context.Roles.Where(r => r.Roles1 == "Security").ToList().First();
                u.Rid = rr.Rid;
                _context.Update(u);
                _context.SaveChanges();
                return RedirectToAction(nameof(SecurityView));
            }
            return View(u);
        }
        // GET: AdminController/Delete/5
        public ActionResult DeletesResident(int id)
        {             
            try
            {
                Users uu = _context.Users.Find(id);
                _context.Users.Remove(uu);
                _context.SaveChanges();
                return RedirectToAction(nameof(ResidentView));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Deletesecretary(int id)
        {

            try
            {
                Users uu = _context.Users.Find(id);
                _context.Users.Remove(uu);
                _context.SaveChanges();
                TempData["status"] = "Secretary Deleted Successfully";

                return RedirectToAction(nameof(SecretaryView));
            }
            catch
            {
                TempData["status"] = "Child Data Exists for this Secretary";
                return RedirectToAction(nameof(SecretaryView));
                //return View("SecretaryView",);
            }
        }
        public ActionResult Deletesecurity(int id)
        {

            try
            {
                Users uu = _context.Users.Find(id);
                _context.Users.Remove(uu);
                _context.SaveChanges();
                return RedirectToAction(nameof(SecurityView));
            }
            catch
            {
                return View();
            }
        }


    }
}




        