using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace IslandEscape.Controllers
{
    public class ConfigurationsController : Controller
    {
        private IslandEscapeOfficialEntities db = new IslandEscapeOfficialEntities();

        public static string GetConfigurationURLPart(string user_id)
        {
            int? sound = null;
            int? brightness = null;

            using(var context = new IslandEscapeOfficialEntities())
            {
                Configuration config = context.Configurations.Where(c => c.UserId == user_id && c.Chosen).FirstOrDefault();

                if(config != null)
                {
                    sound = config.SoundLevel;
                    brightness = config.BrightnessLevel;
                }

            }

            return "&sound=" + (sound != null ? sound : 100) + "&brightness=" + (brightness != null ? brightness : 100);
        }

        // GET: Configurations
        public ActionResult Index(string config_user_id)
        {
            return View(db.Users.Where(user => user.Id == config_user_id).FirstOrDefault());
        }

        // GET: Configurations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        // GET: Configurations/Create
        public ActionResult Create(string config_user_id)
        {
            ViewBag.UserId = getSelectList(config_user_id);
            ViewBag.ConfigurationUserId = config_user_id;
            return View();
        }

        // POST: Configurations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BrightnessLevel,SoundLevel,UserId,Chosen")] Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                db.Configurations.Add(configuration);
                db.SaveChanges();
                updateConfigChosens(configuration, configuration.UserId);
                return RedirectToAction("Index", "Configurations", new { config_user_id = configuration.UserId });
            }

            ViewBag.UserId = getSelectList(configuration.UserId);
            ViewBag.ConfigurationId = configuration.UserId;
            return View(configuration);
        }

        // GET: Configurations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserId = getSelectList(configuration.UserId);
            return View(configuration);
        }

        // POST: Configurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BrightnessLevel,SoundLevel,UserId,Chosen")] Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(configuration).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                updateConfigChosens(configuration, configuration.UserId);
                return RedirectToAction("Index", "Configurations", new { config_user_id = configuration.UserId });
            }

            ViewBag.UserId = getSelectList(configuration.UserId); 
            return View(configuration);
        }

        // GET: Configurations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = db.Configurations.Find(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        // POST: Configurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Configuration configuration = db.Configurations.Find(id);
            string config_user_id = configuration.UserId;
            db.Configurations.Remove(configuration);
            db.SaveChanges();
            updateConfigChosens(null, config_user_id);
            return RedirectToAction("Index", "Configurations", new { config_user_id = config_user_id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private SelectList getSelectList(string config_user_id)
        {
            return new SelectList(db.Users.Where(u => u.Id == config_user_id), "Id", "UserName", config_user_id);
        }

        private void updateConfigChosens(Configuration config, string config_user_id)
        {
            IList<Configuration> chosen_configs = db.Configurations.Where(c => c.UserId == config_user_id && c.Chosen).ToList();
            IEnumerable<Configuration> configs = db.Configurations.Where(c => c.UserId == config_user_id);

            if (chosen_configs.Count == 0 && configs.FirstOrDefault() != null) {
                configs.FirstOrDefault().Chosen = true;
            }else if(chosen_configs.Count > 1)
            {
                for (int i = 1; i < chosen_configs.Count; i++)
                {
                    if(!chosen_configs[i].Equals(config))
                    {
                        chosen_configs[i].Chosen = false;
                    }else
                    {
                        chosen_configs[0].Chosen = false;
                    }
                }
            }               

            db.SaveChanges();

        }
              
    }
}
