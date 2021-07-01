using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCodeFirstApproch.Models;
using System.Data.Entity;

namespace WebCodeFirstApproch.Controllers
{
    public class HomeController : Controller
    {
        StudentContext db = new StudentContext();
        public ActionResult Index()
        {
            var students = GetStudents();
            var employees = GetEmployees();

            MultiModelData data = new MultiModelData();

            data.Students = students;
            data.Employees = employees;

            return View(data);
        }
        public List<Student> GetStudents()
        {
            return db.Students.ToList();
        }
        public List<Employee> GetEmployees()
        {
            return db.Employees.ToList();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Student student)
        {
            if(ModelState.IsValid == true)
            {
                db.Students.Add(student);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    TempData["InsertMessage"] = "<script>alert('Data Inserted')</script>";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.InsertMessage = "<script>alert('Error In Insertion')</script>";
                }
            }
            else
            {
                ViewBag.InsertMessage = "<script>alert('Data is Not Valid')</script>";
            }
            return View(student);
        }
        public ActionResult Edit(int id)
        {
            var row = db.Students.Where(model => model.StdId == id).FirstOrDefault();
            return View(row);
        }
        [HttpPost]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid == true)
            {
                db.Entry(student).State = EntityState.Modified;
                int count = db.SaveChanges();
                if (count > 0)
                {
                    TempData["InsertMessage"] = "<script>alert('Data Updated')</script>";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.UpdateMessage = "<script>alert('Error In Updation')</script>";
                }
            }
            else
            {
                ViewBag.UpdateMessage = "<script>alert('Data is Not Valid')</script>";
            }
            return View();
        }
        public ActionResult Delete(int id)
        {
            if(id > 0)
            {
                var StudentRow = db.Students.Where(model => model.StdId == id).FirstOrDefault();
                if (StudentRow != null)
                {
                    db.Entry(StudentRow).State = EntityState.Deleted;
                    int count = db.SaveChanges();
                    if (count > 0)
                    {
                        TempData["InsertMessage"] = "<script>alert('Data Deleted')</script>";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["InsertMessage"] = "<script>alert('Data Donot Deleted')</script>";
                    }
                }
            }
            else
            {
                TempData["InsertMessage"] = "<script>alert('Select Row!')</script>";
            }
            return RedirectToAction("Index");
        }
        public ActionResult Detials(int id)
        {
            var StudentDeitals = db.Students.Where(model => model.StdId == id).FirstOrDefault();
            return View(StudentDeitals);
        }
    }
}