using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prjShoppingCar.Models;

namespace prjShoppingCar.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        dbShoppingCarEntities db = new dbShoppingCarEntities();
        // GET: Member
        public ActionResult Index()
        {
            var products = db.tProduct.OrderByDescending(r => r.fId).ToList();

            return View("../Home/Index","_LayoutMember",products);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","Home");
        }

        public ActionResult ShoppingCar()
        {
            var UserId = User.Identity.Name;
            var orderDetailDatas = db.tOrderDetail.Where(r => r.fUserId == UserId && r.fIsApproved == "否")
                .Select(r => new tOrderDetailData()
                {
                    orderDetail = r,
                    product = db.tProduct.Where(p => p.fPId == r.fPId).FirstOrDefault()
                }).ToList();

            return View(orderDetailDatas);
        }

        [HttpPost]
        public ActionResult ShoppingCar(string fReceiver,string fEmail,string fAddress)
        { 
            string fUserId = User.Identity.Name;
            string guid= Guid.NewGuid().ToString();

            tOrder order = new tOrder();
            order.fOrderGuid = guid;
            order.fUserId = fUserId;
            order.fReceiver = fReceiver;
            order.fEmail = fEmail;
            order.fAddress = fAddress;
            order.fDate = DateTime.Now;
            db.tOrder.Add(order);

            var carList = db.tOrderDetail.Where(r => r.fUserId == fUserId && r.fIsApproved == "否").ToList();
            foreach(var  item in carList) 
            {
                item.fOrderGuid = guid;
                item.fIsApproved = "是";
            }

            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        public ActionResult OrderList()
        {
            string fUserId = User.Identity.Name;
            var orders = db.tOrder.Where(r => r.fUserId == fUserId).OrderByDescending(r => r.fDate).ToList();

            return View(orders);
        }

        public ActionResult OrderDetail(string fOrderGuid)
        {
            var orderDetailDatas = db.tOrderDetail.Where(r => r.fOrderGuid == fOrderGuid)
                .Select(r=> new tOrderDetailData() {orderDetail = r ,product = db.tProduct.Where(p=>p.fPId == r.fPId).FirstOrDefault() }).ToList();
            return View(orderDetailDatas);
        }

        public ActionResult AddCar(string fPid)
        {
            var UserId = User.Identity.Name;
            var currentCar = db.tOrderDetail.Where(r => r.fUserId == UserId && r.fPId == fPid && r.fIsApproved == "否").FirstOrDefault();

            if (currentCar == null)
            {
                var product = db.tProduct.Where(r => r.fPId == fPid).FirstOrDefault();
                tOrderDetail orderDetail = new tOrderDetail();
                orderDetail.fUserId = UserId;
                orderDetail.fPId = fPid;
                orderDetail.fName = product.fName;
                orderDetail.fPrice = product.fPrice;
                orderDetail.fQty = 1;
                orderDetail.fIsApproved = "否";
                db.tOrderDetail.Add(orderDetail);
            }
            else
            {
                currentCar.fQty += 1;
            }

            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
        }

        public ActionResult DeleteCar(int fId)
        {
            var orderDetail = db.tOrderDetail.Where(r=>r.fId == fId).FirstOrDefault();
            db.tOrderDetail.Remove(orderDetail); 
            db.SaveChanges();

            return RedirectToAction("ShoppingCar");
        }
    }
}