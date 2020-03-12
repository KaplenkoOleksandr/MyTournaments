using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyTournaments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestChartsController : ControllerBase
    {
        private readonly DBTournamentsContext _context;

        public TestChartsController(DBTournamentsContext context)
        {
            _context = context;
        }
        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var game = _context.Game.Include(g => g.Team).ToList();
            List<object> genBook = new List<object>();
            genBook.Add(new[] { "Гра", "Кількість команд" });
            foreach (var g in game)
            {
                genBook.Add(new object[] { g.Name, g.Team.Count() });
            }
            return new JsonResult(genBook);
        }

        [HttpGet("JsonData2")]
        public JsonResult JsonData2()
        {
            var game = _context.Team.Include(g => g.Player).ToList();
            List<object> genBook = new List<object>();
            genBook.Add(new[] { "Команда", "Кількість гравців" });
            foreach (var g in game)
            {
                genBook.Add(new object[] { g.Name, g.Player.Count() });
            }
            return new JsonResult(genBook);
        }
    }
}