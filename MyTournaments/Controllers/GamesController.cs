using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyTournaments;

namespace MyTournaments.Controllers
{
    public class GamesController : Controller
    {
        private readonly DBTournamentsContext _context;

        public GamesController(DBTournamentsContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            return View(await _context.Game.ToListAsync());
        }

        public async Task<IActionResult> ViewExport()
        {
            ViewData["GamesModel"] = _context.Game.ToList();
            ViewData["TeamsModel"] = _context.Team.Include(t => t.Sponsor).ToList();
            ViewData["PlayersModel"] = _context.Player.ToList();
            ViewData["SponsorsModel"] = _context.Sponsor.ToList();
            ViewData["TournamentsModel"] = _context.Tournament.ToList();
            return View();
        }
        
        public async Task<IActionResult> GamesByTournament(int id)
        {
            var g = from l in _context.TournamentGames
                    where l.TournametId == id
                    select l.Game;
            return View(await g.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            //return View(game);
            return RedirectToAction("Index", "Teams", new { id = game.Id, name = game.Name });
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Info")] Game game)
        {
            int counter = 0;
            foreach (var g in _context.Game)
            {
                if (g.Name == game.Name) { counter++; break; }
            }
            if (counter != 0)
            {
                ModelState.AddModelError("Name", "Таке ім'я вже існує");
                return View(game);
            }
            else {
                if (ModelState.IsValid)
                {
                    _context.Add(game);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(game);
            }
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Info")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.FindAsync(id);

            var teams = from t in _context.Team
                        where t.GameId == id
                        select t;
            foreach(var t in teams)
            {
                var players = from p in _context.Player
                              where p.TeamId == t.Id
                              select p;
                foreach (var p in players)
                {
                    _context.Player.Remove(p);
                }
                
                _context.Team.Remove(t);
            }
            await _context.SaveChangesAsync();

            var tournByGame = from tbg in _context.TournamentGames
                        where tbg.GameId == id
                        select tbg;
            foreach (var tbg in tournByGame)
            {
                _context.TournamentGames.Remove(tbg);
            }
            await _context.SaveChangesAsync();

            _context.Game.Remove(game);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.Id == id);
        }
    }

}
