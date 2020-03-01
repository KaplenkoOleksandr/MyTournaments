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
    public class TournamentsController : Controller
    {
        private readonly DBTournamentsContext _context;

        public TournamentsController(DBTournamentsContext context)
        {
            _context = context;
        }

        // GET: Tournaments
        public async Task<IActionResult> Index()
        {
            var dBTournamentsContext = _context.Tournament.Include(t => t.Sponsor);
            return View(await dBTournamentsContext.ToListAsync());
        }
        // GET: Tournaments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tournament = await _context.Tournament
                .Include(t => t.Sponsor)
                .FirstOrDefaultAsync(m => m.Id == id);

            var tornbygame = from tbg in _context.TournamentGames
                             where tbg.TournametId == tournament.Id
                             select tbg;
            List<string> namesgames = new List<string>();
            foreach(var tbg in tornbygame)
            {
                var gamename = from gn in _context.Game
                               where gn.Id == tbg.GameId
                               select gn;
                foreach(var gn in gamename)
                {
                    namesgames.Add(gn.Name); 
                }
            }

            ViewData["GameName"] = namesgames;

            if (tournament == null)
            {
                return NotFound();
            }
            return View(tournament);
        }

        // GET: Tournaments/Create
        public IActionResult Create()
        {
            ViewData["SponsorId"] = new SelectList(_context.Sponsor, "Id", "Name");
            return View();
        }

        // POST: Tournaments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SponsorId,Location,PrizeFund")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SponsorId"] = new SelectList(_context.Sponsor, "Id", "Name", tournament.SponsorId);
            return View(tournament);
        }

        // GET: Tournaments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournament.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }
            ViewData["SponsorId"] = new SelectList(_context.Sponsor, "Id", "Name", tournament.SponsorId);
            return View(tournament);
        }

        // POST: Tournaments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SponsorId,Location,PrizeFund")] Tournament tournament)
        {
            if (id != tournament.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournament.Id))
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
            ViewData["SponsorId"] = new SelectList(_context.Sponsor, "Id", "Name", tournament.SponsorId);
            return View(tournament);
        }

        // GET: Tournaments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournament
                .Include(t => t.Sponsor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // POST: Tournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournament.FindAsync(id);

            var tournByGame = from tbg in _context.TournamentGames
                              where tbg.TournametId == id
                              select tbg;
            foreach (var tbg in tournByGame)
            {
                _context.TournamentGames.Remove(tbg);
            }
            await _context.SaveChangesAsync();

            _context.Tournament.Remove(tournament);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournament.Any(e => e.Id == id);
        }
    }
}
