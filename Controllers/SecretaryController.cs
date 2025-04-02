using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using WebApplication3.Models;
using System.Net.NetworkInformation;

namespace WebApplication3.Controllers
{
    [SessionCheck]
    public class SecretaryController : Controller
    {
        // GET: SecretaryController
        visamodbContext _context;
        public SecretaryController(visamodbContext context)
        {
            _context = context;

        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: SecretaryController/Details/5
        public ActionResult Details(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }
        public ActionResult NoticeCreate()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }

        [HttpPost]
        public ActionResult NoticeCreate(Notice n)
        {
            if (ModelState.IsValid)
            {
                Users uu = _context.Users.Where(u => u.Rid == 17).ToList().First();
                n.Uid = uu.Uid;
                _context.Add(n);
                _context.SaveChanges();
                return RedirectToAction(nameof(NoticeView));
            }
            return View(n);
        }
        public ActionResult NoticeEdit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            Notice n = _context.Notice.Find(id);
            return View(n);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NoticeEdit(Notice n)
        {
            if (ModelState.IsValid)
            {
                Users uu = _context.Users.Where(u => u.Rid == 17).ToList().First();
                n.Uid = uu.Uid;
                _context.Notice.Update(n);

                _context.SaveChanges();
                return RedirectToAction(nameof(NoticeView));
            }
            return View(n);
        }
        public ActionResult NoticeView()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            var notices = _context.Notice.OrderByDescending(n => n.Nid).ToList();
            return View(notices);
        }

        //public ActionResult Maintenance()
        //{
        //    TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
        //        TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
        //        TempData["email"] = HttpContext.Session.GetString("email");//, t.email)
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Maintenance(Income i)
        //{
        //    try
        //    {
        //        Users uu = _context.Users.FirstOrDefault(u => u.Rid == 17);
        //        i.Uid = uu.Uid;
        //        i.Date = DateTime.Now;
        //        i.Status = "Pending";
        //        _context.Income.Add(i);
        //        _context.SaveChanges();
        //        return RedirectToAction("ViewMaintenance", i);
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        public ActionResult Maintenance()
        {
            TempData["user"] = HttpContext.Session.GetString("name");
            TempData["photo"] = HttpContext.Session.GetString("photo");
            TempData["email"] = HttpContext.Session.GetString("email");

            // Get all residents (Users) from the database
            var residents = _context.Users.Where(u => u.Rid == 19).ToList();

            // Pass the list of residents to the ViewBag or ViewData if needed
            ViewBag.Residents = residents;

            return View();
        }

        [HttpPost]
        public ActionResult Maintenance(Income i)
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                // Check if maintenance already exists for this month
                var existingMaintenance = _context.Income
                    .Where(m => m.Date.Month == currentMonth && m.Date.Year == currentYear)
                    .FirstOrDefault();

                if (existingMaintenance != null)
                {
                    // If maintenance already exists for the current month, inform the secretary
                    TempData["Error"] = "Maintenance for this month has already been generated.";
                    return RedirectToAction("Maintenance");
                }

                // Get the list of residents
                var residents = _context.Users.Where(u => u.Rid == 19).ToList();

                foreach (var resident in residents)
                {
                    int maintenanceAmount = i.Amount;

                    // Check if the resident is in Wing 'A'
                    if (resident.Wingno.Trim().ToUpper() == "A")
                    {
                        // Apply 20% increase for Wing A
                        maintenanceAmount += 1200; // 6000 + 1200 = 7200
                    }

                    // Calculate maintenance for each resident
                    Income maintenance = new Income
                    {
                        Uid = resident.Uid,
                        Amount = maintenanceAmount,  // You can set this dynamically if you need different amounts
                        Description = i.Description,
                        Flatno = resident.Flatno,   // Assuming the Resident has a FlatNo field
                        Wingno = resident.Wingno,   // Assuming the Resident has a WingNo field
                        Date = DateTime.Now,
                        Status = "Pending",
                        Details = i.Details
                    };

                    // Add the maintenance record to the database
                    _context.Income.Add(maintenance);
                }

                // Save changes to the database
                _context.SaveChanges();

                // Redirect to a different view, for example, a maintenance overview
                return RedirectToAction("ViewMaintenance");
            }
            catch
            {
                // Handle any errors that occur
                return View();
            }
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
                Users uu = _context.Users.Where(u => u.Rid == 17).ToList().First();
                i.Uid = uu.Uid;
                i.Date = DateTime.Now;
                i.Status = "Pending";
                // i.Status = "Done";
                _context.Income.Update(i);
                _context.SaveChanges();
                return RedirectToAction("ViewMaintenance", i);
            }
            catch
            {
                return View();
            }
        }
        //[HttpPost]
        //public IActionResult QuickUpdateStatus(int Id, string Status)
        //{
        //    try
        //    {
        //        var record = _context.Income.Find(Id);
        //        if (record != null)
        //        {
        //            record.Status = Status;
        //            _context.SaveChanges();
        //        }
        //        return RedirectToAction("ViewMaintenance");
        //    }
        //    catch (Exception)
        //    {
        //        return RedirectToAction("ViewMaintenance"); // Error handling can be improved
        //    }
        //}

        public ActionResult ViewMaintenance(string? status, string? wing)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            using (var db = new visamodbContext()) // Replace with your actual DbContext
            {
                var maintenanceData = db.Income
                .Where(i => i.Description.Contains("maintenance"))
                .ToList();

                if (wing == "Wing A")
                {
                    maintenanceData = maintenanceData
                        .Where(i => i.Wingno == "A") // Assuming Wing A is identified by 'A'
                        .ToList();
                }
                else if (wing == "Other Wings")
                {
                    maintenanceData = maintenanceData
                        .Where(i => i.Wingno != "A") // Filter out Wing A to get other wings
                        .ToList();
                }

                // Segregate the data based on status
                if (status == "Paid")
                {
                    maintenanceData = maintenanceData
                        .Where(i => i.Status == "Paid")
                        .ToList();
                }
                else if (status == "Pending")
                {
                    maintenanceData = maintenanceData
                        .Where(i => i.Status == "Pending")
                        .ToList();
                }
                else if (status == "Overdue")
                {
                    //DateTime testDate = new DateTime(2025, 4, 7); // Simulating April 7, 2025

                    maintenanceData = maintenanceData
                        .Where(i => i.Status == "Pending" && i.Date.Day >= 6)
                        .ToList();

                    Console.WriteLine("Overdue Count: " + maintenanceData.Count);
                    foreach (var record in maintenanceData)
                    {
                        Console.WriteLine("FlatNo: " + record.Flatno + " | Date: " + record.Date + " | Status: " + record.Status);
                    }
                }
                ViewBag.SelectedStatus = status; // Store status for highlighting buttons if needed
                ViewBag.SelectedWing = wing; // Store wing for UI updates

                return View(maintenanceData);
                // return RedirectToAction("PayView", i);
            }                 
        }
        //Card No
        //5267 3181 8797  5449        

        public ActionResult ExpenseCreate()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }

        [HttpPost]
        public ActionResult ExpenseCreate(Expense e)
        {
            if (ModelState.IsValid)
            {
                Users uu = _context.Users.Where(u => u.Rid == 17).ToList().First();
                e.Uid = uu.Uid;
                _context.Add(e);
                _context.SaveChanges();
                return RedirectToAction(nameof(ExpenseView));
            }
            return View(e);
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
        public ActionResult ExpenseEdit(int id)
        {
            TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
            TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
            TempData["email"] = HttpContext.Session.GetString("email");//, t.email)    
            Expense e = _context.Expense.Find(id);
            return View(e);
        }
        [HttpPost]
        public ActionResult ExpenseEdit(Expense e)
        {
            if (ModelState.IsValid)
            {
                Users uu = _context.Users.Where(u => u.Rid == 17).ToList().First();
                e.Uid = uu.Uid;
                _context.Expense.Update(e);
                _context.SaveChanges();
                return RedirectToAction(nameof(ExpenseView));
            }
            return View(e);
        }
        public ActionResult ExpenseDelete(int id)
        {
            try
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.Email);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");
                Expense e = _context.Expense.Find(id);
                _context.Expense.Remove(e);
                _context.SaveChanges();
                return RedirectToAction(nameof(ExpenseView));

            }
            catch
            {
                return View();
            }
        }
        public ActionResult IncomeCreate()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }

        [HttpPost]
        public ActionResult IncomeCreate(Income i)
        {
            if (ModelState.IsValid)
            {
                Users uu = _context.Users.Where(u => u.Rid == 17).ToList().First();
                i.Uid = uu.Uid;
                i.Date = DateTime.Now;
                i.Status = "Done";
                _context.Income.Add(i);
                _context.SaveChanges();
                return RedirectToAction("PayView", i);
            }
            return View(i);
        }
        //public ActionResult PayView(int Id)
        //{
        //    Income i = _context.Income.Find(Id);

        //    Dictionary<string, object> input = new Dictionary<string, object>();
        //    input.Add("amount", i.Amount * 100); // this amount should be same as transaction amount
        //    input.Add("currency", "INR");
        //    input.Add("receipt", Id.ToString());

        //    string key = "rzp_test_cZd0r0o9mHqKDn";
        //    string secret = "kMaOmbFyEvYX2h4c7s9WzHbp";

        //    RazorpayClient client = new RazorpayClient(key, secret);

        //    Razorpay.Api.Order order = client.Order.Create(input);
        //    ViewBag.orderId = order["id"].ToString();
        //    ViewBag.incomeId = Id; // Pass Income ID to the View
        //    return View("PayView", i);
        //}
        //[HttpPost]
        //public ActionResult PayView(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        //{
        //    Dictionary<string, string> attributes = new Dictionary<string, string>();
        //    attributes.Add("razorpay_payment_id", razorpay_payment_id); // this amount should be same as transaction amount
        //    attributes.Add("razorpay_order_id", razorpay_order_id);
        //    attributes.Add("razorpay_signature", razorpay_signature);
        //    try
        //    {
        //        Utils.verifyPaymentSignature(attributes);
        //        // Extract the Income ID from the Razorpay Order object
        //        RazorpayClient client = new RazorpayClient("rzp_test_cZd0r0o9mHqKDn", "kMaOmbFyEvYX2h4c7s9WzHbp");
        //        Razorpay.Api.Order order = client.Order.Fetch(razorpay_order_id);
        //        int incomeId = int.Parse(order["receipt"].ToString());  // Extract Income ID

        //        // Find the payment record and update the status
        //        Income i = _context.Income.Find(incomeId);
        //        if (i != null)
        //        {
        //            i.Status = "Paid"; // Update status
        //            _context.SaveChanges();
        //        }
        //        return View("success");
        //    }

        //    catch (Exception ex)
        //    {
        //        return View("failure");

        //    }
        //}
        // //return View();        
        //public ActionResult Success(Income i)
        //{
        //    i.Status = "Done";
        //    return View();
        //}

        //public ActionResult Failure(Income i)
        //{
        //    i.Status = "Fail";
        //    return View();
        //}
        public ActionResult IncomeEdit(int id)
        {
            TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
            TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
            TempData["email"] = HttpContext.Session.GetString("email");//, t.email)    
            Income i = _context.Income.Find(id);
            return View(i);
            
        }
        [HttpPost]
        public ActionResult IncomeEdit(Income i)
        {
            if (ModelState.IsValid)
            {
                Users uu = _context.Users.Where(u => u.Rid == 17).ToList().First();
                i.Uid = uu.Uid;
                i.Date = DateTime.Now;
                i.Status = "Done";
                _context.Income.Update(i);
                _context.SaveChanges();
                return RedirectToAction(nameof(IncomeView));
            }
            return View(i);
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
        
        public ActionResult IncomeDelete(int id)
        {
            try
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.Email);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");
                Income i = _context.Income.Find(id);
                _context.Income.Remove(i);
                _context.SaveChanges();
                return RedirectToAction(nameof(NoticeView));

            }
            catch
            {
                return View();
            }
        }
        public ActionResult ManageEvent()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Event.ToList());
        }
        public IActionResult Resolve(int id)
        {
            using (var db = new visamodbContext())
            {
                var complaint = db.Complain.FirstOrDefault(c => c.Cid == id);
                if (complaint != null)
                {
                    complaint.Status = "Resolved"; // Update status
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ViewComplain");
        }
        public ActionResult ViewComplain()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)                                                                                      
            }
            return View(_context.Complain.ToList());
        }
        // GET: SecretaryController/Create
        public ActionResult Create()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }

        // POST: SecretaryController/Create
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

        // GET: SecretaryController/Edit/5
        public ActionResult Edit(int id)
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
        public ActionResult NoticeDelete(int id)
        {
            try
            {
                Notice nn = _context.Notice.Find(id);
                _context.Notice.Remove(nn);
                _context.SaveChanges();
                return RedirectToAction(nameof(NoticeView));

            }
            catch
            {
                return View();
            }
        }
        public ActionResult ComplainDelete(int id)
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


    }
}
