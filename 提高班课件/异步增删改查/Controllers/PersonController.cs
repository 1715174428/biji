using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestIService;
using TestService;
using 异步增删改查.Models;

namespace 异步增删改查.Controllers
{
    public class PersonController : Controller
    {
        // GET: Person
        public async Task<ActionResult> Index()
        {
            IPersonService ps = new PersonService();
            var persons = await ps.GetAllAsync();
            return View(persons);
        }

        [HttpGet]
        public async Task<ActionResult> AddNew()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddNew(PersonAddNewModel model)
        {
            IPersonService ps = new PersonService();
            await ps.AddAsync(model.Name, model.Age);
            return Redirect("/Person/Index");
        }

        [HttpGet]
        public async Task<ActionResult> Export()
        {
            IPersonService ps = new PersonService();
            var persons = await ps.GetAllAsync();
            this.Response.AddHeader("Content-Disposition",
                "attachment; filename=1.txt");
            //using (MemoryStream ms = new MemoryStream())
            MemoryStream ms = new MemoryStream();
            //using (StreamWriter writer = new StreamWriter(ms))
            StreamWriter writer = new StreamWriter(ms);
            {
                foreach (var p in persons)
                {
                    await writer.WriteLineAsync(p.Name + "=" + p.Age);
                }
               // writer.Flush();                
            }
            ms.Position = 0;
            return File(ms, "text/plain");
        }
    }
}