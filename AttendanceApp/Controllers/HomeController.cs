using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceApp.DAL;
using AttendanceApp.Models;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data.Entity;
using static Newtonsoft.Json.JsonConvert;

namespace AttendanceApp.Controllers
{
    public class HomeController : Controller
    {
        private EmployeeDBContext db = new EmployeeDBContext();

        [Authorize]
        public ActionResult Index(AttendanceViewModel vm)
        {
            Employee userinfo = DeserializeObject<Employee>(User.Identity.Name);

            DateTime todayDate = DateTime.Now;

            //DateTime todayDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy"));
            bool AttendanceValid = db.Attendance.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userinfo.ID);

            if (AttendanceValid == true)
            {
                vm.iscoming = true;
                bool AttendanceAllValid = db.Attendance.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userinfo.ID && c.LeaveTime != null);
                if (AttendanceAllValid)
                {
                    vm.isLeave = true;
                }

            }
            else
            {
                vm.iscoming = false;
            }




            return View(vm);


        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(Attendance attendance, AttendanceViewModel vm)
        {
            Employee userinfo = DeserializeObject<Employee>(User.Identity.Name);


            DateTime myDateTime = DateTime.Now;

            //DateTime myDateTime = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

            DateTime todayDate = myDateTime.Date;

            //DateTime todayDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy"));



            /////////////////////////////implement

            bool AttendanceValid = db.Attendance.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userinfo.ID);

            if (AttendanceValid)
            {
                var attendanceRow = db.Attendance.Where(c => c.DateOfDay == todayDate && c.EmployeeID == userinfo.ID)
                    .Single();
                vm.iscoming = true;
                vm.isLeave = true;
                attendanceRow.LeaveTime = myDateTime;


                db.Entry(attendanceRow).State = EntityState.Modified;
                db.SaveChanges();

                return View(vm);

            }
            else
            {
                int userID = userinfo.ID;
                DateTime now = DateTime.Now;
                vm.isLeave = false;
                Attendance Attendance = new Attendance
                {
                    EmployeeID = userID,
                    ComingTime = myDateTime,
                    DateOfDay = todayDate,


                    //DateOfDay = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy")),

                };

                db.Attendance.Add(Attendance);
                db.SaveChanges();
                vm.iscoming = true;
                return View(vm);
            }

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
