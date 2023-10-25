using prjShoppingCar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace prjShoppingCar.Controllers
{
    public class HomeController : Controller
    {
        dbShoppingCarEntities db = new dbShoppingCarEntities();
        public ActionResult Index()
        {
            var products = db.tProduct.OrderByDescending(r=>r.fId).ToList();
            return View(products);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd)
        {
            var member = db.tMember.Where(r=>r.fUserId == fUserId && r.fPwd == fPwd).FirstOrDefault();
            if(member == null) 
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }

            Session["Welcome"] = member.fName + "歡迎光臨";

            FormsAuthentication.RedirectFromLoginPage(fUserId, true);
            return RedirectToAction("Index", "Member");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(tMember newMember)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            var member = db.tMember.Where(r => r.fUserId == newMember.fUserId).FirstOrDefault();
            if (member == null)
            {
                db.tMember.Add(newMember);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            ViewBag.Message = "此帳號已經有人使用";
            return View();
        }
    }
}