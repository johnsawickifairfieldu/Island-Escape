using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Services;

namespace IslandEscape.Controllers
{
    public class SavdGameStatesController : Controller
    {
        private IslandEscapeOfficialEntities db = new IslandEscapeOfficialEntities();

        /// <summary>
        /// Request a new game at a difficulty level.
        /// </summary>
        /// <param name="intensity_level">Difficulty level of new game. 1 is easy, 2 is medium, 3 is hard.</param>
        /// <returns>The new GameID in json format.</returns>
        [WebMethod]
        public static string GetNewGame(int intensity_level)
        {
            using (var context = new IslandEscapeOfficialEntities())
            {
                var new_game = new Game() { IntensityLevel = intensity_level };

                context.Games.Add(new_game);
                context.SaveChanges();
                                               
                return "{\"gameid\":" + new_game.Id + "}";
            }
        }

        /// <summary>
        /// Save a game state.
        /// </summary>
        /// <param name="game_id">The GameID.</param>
        /// <param name="user_id">The UserID.</param>
        /// <param name="progress">The progess between 0 and 101. 100 is complete and failed. 101 is complete and passed.</param>
        /// <returns>true or false on whether the save was successful.</returns>
        [WebMethod]
        public static string SaveGame(int game_id, string user_id, int progress)
        {
            int records = 0;

            using (var context = new IslandEscapeOfficialEntities())
            {
                var new_game_state = new SavdGameState { GameId = game_id, UserId = user_id, Progress = progress, Saved = DateTime.Now };

                context.SavdGameStates.Add(new_game_state);
                records = context.SaveChanges();
            }

            return "{\"result\":" + (records == 0 ? false : true) + "}";
        }

        // GET: SavdGameStates
        public ActionResult Index()
        {
            string user_id = User.Identity.GetUserId();
            int access_level = db.Users.Where(u => u.Id == user_id).FirstOrDefault().AccessLevel;

            var savdGameStates = db.SavdGameStates.Where(s => s.UserId == user_id || access_level >= 2).Include(s => s.Game).Include(s => s.User).OrderByDescending(s => s.UserId == user_id).ThenByDescending(s => s.Progress >= 100 ? -1 : 1).ThenByDescending(s => s.Saved);

            return View(savdGameStates.ToList());
        }

        // GET: SavdGameStates/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavdGameState savdGameState = db.SavdGameStates.Find(id);
            if (savdGameState == null)
            {
                return HttpNotFound();
            }
            return View(savdGameState);
        }

        // GET: SavdGameStates/Create
        public ActionResult Create()
        {
            ViewBag.GameId = new SelectList(db.Games, "Id", "Id");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        // POST: SavdGameStates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GameId,UserId,Saved,Progress")] SavdGameState savdGameState)
        {
            if (ModelState.IsValid)
            {
                db.SavdGameStates.Add(savdGameState);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GameId = new SelectList(db.Games, "Id", "Id", savdGameState.GameId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", savdGameState.UserId);
            return View(savdGameState);
        }

        // GET: SavdGameStates/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavdGameState savdGameState = db.SavdGameStates.Find(id);
            if (savdGameState == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameId = new SelectList(db.Games, "Id", "Id", savdGameState.GameId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", savdGameState.UserId);
            return View(savdGameState);
        }

        // POST: SavdGameStates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GameId,UserId,Saved,Progress")] SavdGameState savdGameState)
        {
            if (ModelState.IsValid)
            {
                db.Entry(savdGameState).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GameId = new SelectList(db.Games, "Id", "Id", savdGameState.GameId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", savdGameState.UserId);
            return View(savdGameState);
        }

        // GET: SavdGameStates/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavdGameState savdGameState = db.SavdGameStates.Find(id);
            if (savdGameState == null)
            {
                return HttpNotFound();
            }
            return View(savdGameState);
        }

        // POST: SavdGameStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            SavdGameState savdGameState = db.SavdGameStates.Find(id);
            db.SavdGameStates.Remove(savdGameState);
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
