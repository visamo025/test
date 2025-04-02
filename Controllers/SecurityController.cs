using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [SessionCheck]
    public class SecurityController : Controller
    {
        // GET: SecurityController
        public ActionResult Index()
        {
            return View();
        }
        private readonly visamodbContext _context;

        // GET: AdminController
        public SecurityController(visamodbContext context)
        {
            _context = context;
        }

        // GET: SecurityController/Details/5

        public ActionResult GatepassDetails()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Gatepass.ToList());
        }
        public ActionResult ServiceDetails()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Service.ToList());
        }

        public ActionResult visitor()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Visitor.ToList());
        }
        public ActionResult complainview()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View(_context.Complain.ToList());
        }

        // GET: SecurityController/Create
        public ActionResult Create()
        {
            {
                TempData["user"] = HttpContext.Session.GetString("name");//, t.user);
                TempData["photo"] = HttpContext.Session.GetString("photo");//, t.Photo);
                TempData["email"] = HttpContext.Session.GetString("email");//, t.email)               
            }
            return View();
        }

        // POST: SecurityController/Create
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

        // GET: SecurityController/Edit/5
        //public ActionResult VisitorEdit(int id)
        //{
        //    Visitor v = _context.Visitor.Find(id);
        //    return View(v);
        //}
        //[HttpPost]
        //public ActionResult VisitorEdit(Complain c)
        //{
        //    try
        //    {
        //        Users uu = _context.Users.Where(u => u.Uid == 29).ToList().First();
        //        c.Uid = uu.Uid;
        //        _context.Complain.Update(c);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Visitor));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: SecurityController/Delete/5

        //        public ActionResult Delete(int id)
        //        {

        //            try
        //            {
        //                Service s = _context.Service.Find(id);
        //                _context.Service.Remove(s);
        //                _context.SaveChanges();
        //                return RedirectToAction(nameof(ServiceDetails));
        //            }
        //            catch
        //            {
        //                return View();
        //            }
        //        }
        //        public ActionResult DeleteGatepass(int id)
        //        {

        //            try
        //            {
        //                Gatepass g = _context.Gatepass.Find(id);
        //                _context.Gatepass.Remove(g);
        //                _context.SaveChanges();
        //                return RedirectToAction(nameof(GatepassDetails));
        //            }
        //            catch
        //            {
        //                return View();
        //            }
        //        }
        public ActionResult DeleteVisitor(int id)
        {

            try
            {
                Visitor v = _context.Visitor.Find(id);
                _context.Visitor.Remove(v);
                _context.SaveChanges();
                return RedirectToAction(nameof(visitor));
            }
            catch
            {
                return View();
            }
        }
        //public ActionResult DeleteComplain(int id)
        //        {

        //            try
        //            {
        //                Complain c = _context.Complain.Find(id);
        //                _context.Complain.Remove(c);
        //                _context.SaveChanges();
        //                return RedirectToAction(nameof(complainview));
        //            }
        //            catch
        //            {
        //                return View();
        //            }
        //        }




    }
}
