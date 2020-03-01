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
    public class TournamentGamesController : Controller
    {
        private readonly DBTournamentsContext _context;

        public TournamentGamesController(DBTournamentsContext context)
        {
            _context = context;
        }

        // GET: TournamentGames
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Home", "Index");
            ViewBag.gameId = id;
            ViewBag.gameName = name;
            var tournByGame = _context.TournamentGames.Where(t => t.GameId == id)
                .Include(b => b.Tournamet)
                .Include(b => b.Game)
                .Include(t => t.Tournamet.Sponsor);
            return View(await tournByGame.ToListAsync());
        }

        // GET: TournamentGames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentGames = await _context.TournamentGames
                .Include(t => t.Game)
                .Include(t => t.Tournamet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournamentGames == null)
            {
                return NotFound();
            }

            return View(tournamentGames);
        }

        // GET: TournamentGames/Create
        public IActionResult Create(int? gameId, string? gameName)
        {
            ViewBag.gameId = gameId;
            ViewBag.gameName = gameName;
            ViewData["TournametId"] = new SelectList(_context.Tournament, "Id", "Name");
            return View();
        }

        // POST: TournamentGames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int gameId , string gameName, [Bind("Id,TournametId")] TournamentGames tournamentGames)
        {
            if (gameName == null) return RedirectToAction("Home", "Index");
            TournamentGames tg = new TournamentGames();
            tg.GameId = gameId;
            tournamentGames.GameId = gameId;
            tg.TournametId = tournamentGames.TournametId;

            if (ModelState.IsValid)
            {
                _context.Add(tg);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = gameId, name = gameName });
            }

            return RedirectToAction("Index", new { id = gameId, name = gameName });
        }

        // GET: TournamentGames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentGames = await _context.TournamentGames.FindAsync(id);
            if (tournamentGames == null)
            {
                return NotFound();
            }
            ViewData["GameId"] = new SelectList(_context.Game, "Id", "Name", tournamentGames.GameId);
            ViewData["TournametId"] = new SelectList(_context.Tournament, "Id", "Name", tournamentGames.TournametId);
            return View(tournamentGames);
        }

        // POST: TournamentGames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TournametId,GameId")] TournamentGames tournamentGames)
        {
            if (id != tournamentGames.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournamentGames);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentGamesExists(tournamentGames.Id))
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
            ViewData["GameId"] = new SelectList(_context.Game, "Id", "Name", tournamentGames.GameId);
            ViewData["TournametId"] = new SelectList(_context.Tournament, "Id", "Location", tournamentGames.TournametId);
            return View(tournamentGames);
        }

        // GET: TournamentGames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentGames = await _context.TournamentGames
                .Include(t => t.Game)
                .Include(t => t.Tournamet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournamentGames == null)
            {
                return NotFound();
            }

            return View(tournamentGames);
        }

        // POST: TournamentGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,int GameId)
        {
            var tournamentGames = await _context.TournamentGames.FindAsync(id);
            _context.TournamentGames.Remove(tournamentGames);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index",new { id= GameId });
        }

        private bool TournamentGamesExists(int id)
        {
            return _context.TournamentGames.Any(e => e.Id == id);
        }
    }
}
