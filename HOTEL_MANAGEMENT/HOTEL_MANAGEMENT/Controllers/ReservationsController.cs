using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HOTEL_MANAGEMENT.Models;

namespace HOTEL_MANAGEMENT.Controllers
{
    public class ReservationsController : Controller
    {
        private DB_HOTEL_MANAGEMENTEntities db = new DB_HOTEL_MANAGEMENTEntities();

        // GET: Reservations
        public ActionResult Index()
        {
            if (Session["Id_user"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if(Session["Id_user"] != null)
            {
                var reservations = db.Reservations.Include(r => r.Room).Include(r => r.User);
                return View(reservations.ToList());
            }
            return RedirectToAction("ErrorAuthorisation", "Home");
        }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Id_user"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (Session["Id_user"]!= null && Int32.Parse(Session["Id_user"].ToString()) == reservation.Id_user || Session["Roles"].ToString().ToLower() == "true")
            {
                if (reservation == null)
                {
                    return HttpNotFound();
                }


                return View(reservation);
            }
            return RedirectToAction("ErrorAuthorisation", "Home");
        }

        // GET: Reservations/Create
        //public ActionResult Create(int? id)
        //{
        //    Session["Id_Room"] = id;
        //    return RedirectToAction("Create");
        //}

        // GET: Reservations/Create
        public ActionResult Create(int? id)
        {
            if (Session["Id_user"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            Session["Id_Room"] = id;
            if(Session["Id_Room"] == null)
            {
                ViewBag.errorroom = "please, choose a room to book";
                return RedirectToAction("index", "rooms");
            }
            ViewBag.Id_Room = new SelectList(db.Rooms, "Id_Room", "Image_Room");
            ViewBag.Id_user = new SelectList(db.Users, "Id_user", "First_Name");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Date_Begin,Date_End")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.Id_Room = Int32.Parse(Session["Id_Room"].ToString());
                reservation.Id_user = Int32.Parse(Session["Id_user"].ToString());
                reservation.Date_Reservation = DateTime.Now;

                Room room = db.Rooms.Find(reservation.Id_Room);
                if (room == null)
                {
                    return HttpNotFound();
                }
                
                int Days = (int)(reservation.Date_End - reservation.Date_Begin).TotalDays;
                reservation.Bill = Days * room.Price;

                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_Room = new SelectList(db.Rooms, "Id_Room", "Image_Room", reservation.Id_Room);
            ViewBag.Id_user = new SelectList(db.Users, "Id_user", "First_Name", reservation.Id_user);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Id_user"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (Int32.Parse(Session["Id_user"].ToString()) == reservation.Id_user)
            { 
                if (reservation == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Id_Room = new SelectList(db.Rooms, "Id_Room", "Image_Room", reservation.Id_Room);
                ViewBag.Id_user = new SelectList(db.Users, "Id_user", "First_Name", reservation.Id_user);
                return View(reservation);
            }
            return RedirectToAction("ErrorAuthorisation", "Home");
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Reservation,Date_Begin,Date_End,Date_Reservation,Bill,Id_user,Id_Room")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Room = new SelectList(db.Rooms, "Id_Room", "Image_Room", reservation.Id_Room);
            ViewBag.Id_user = new SelectList(db.Users, "Id_user", "First_Name", reservation.Id_user);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Id_user"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (Session["Id_user"]!= null && Int32.Parse(Session["Id_user"].ToString()) == reservation.Id_user)
            {
                if (reservation == null)
                {
                    return HttpNotFound();
                }
                return View(reservation);
            }
            return RedirectToAction("ErrorAuthorisation", "Home");
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
