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
    public class PlayersController : Controller
    {
        private readonly DBTournamentsContext _context;

        public PlayersController(DBTournamentsContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index(int? id, int? gameId, string? gameName)
        {
            ViewBag.teamId = id;
            ViewBag.gameId = gameId;
            ViewBag.gameName = gameName;
            var playersByTeams = _context.Player
                .Where(p => p.TeamId == id)
                .Include(p => p.Team);

            return View(await playersByTeams.ToListAsync());
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Player
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        public IActionResult Create(int teamId, int gameId, string gameName)
        {
            if (gameName == null) return RedirectToAction("Index", "Home");
            ViewBag.teamId = teamId;
            ViewBag.gameId = gameId;
            ViewBag.gameName = gameName;
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int teamId, int gameId, string gameName, [Bind("Id,Name,Position,Info,EntranceDate")] Player player)
        {
            if (player.EntranceDate > DateTime.Now)
            {
                ModelState.AddModelError("EntranceDate", "jhbfdjnhghnj");
                return View(player);
            }
            else
            {
                if (gameName == null) return RedirectToAction("Index", "Home");
                player.TeamId = teamId;
                if (ModelState.IsValid)
                {
                    _context.Add(player);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Players", new { id = teamId, gameId, gameName });
                }
                return RedirectToAction("Index", "Players", new { id = teamId, gameId, gameName });
            }
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id, int? teamId, int gameId, string gameName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Player.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            ViewBag.teamId = teamId;
            ViewBag.gameId = gameId;
            ViewBag.gameName = gameName;
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,int teamId, int gameId, string gameName, [Bind("Id,Name,Position,Info,EntranceDate")] Player player)
        {
            player.TeamId = teamId;
            if (id != player.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Players", new { id = teamId, gameId, gameName });
            }
            return RedirectToAction("Index", "Players", new { id = teamId, gameId, gameName });
        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id, int teamId, int gameId, string gameName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Player
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }
            ViewBag.teamId = teamId;
            ViewBag.gameId = gameId;
            ViewBag.gameName = gameName;
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int teamId, int gameId, string gameName)
        {
            var player = await _context.Player.FindAsync(id);
            _context.Player.Remove(player);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Players", new { id = teamId, gameId, gameName });
        }

        private bool PlayerExists(int id)
        {
            return _context.Player.Any(e => e.Id == id);
        }
    }
}
