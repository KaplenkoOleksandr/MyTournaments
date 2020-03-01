using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MyTournaments.Controllers
{
    public class TeamsController : Controller
    {
        private readonly DBTournamentsContext _context;

        public TeamsController(DBTournamentsContext context)
        {
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Games", "Index");
            ViewBag.gameId = id;
            ViewBag.gameName = name;
            var teamsByGame = _context.Team
                .Where(b => b.GameId == id)
                .Include(b => b.Game)
                .Include(b => b.Sponsor);

            return View(await teamsByGame.ToArrayAsync());
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Team
                .Include(t => t.Game)
                .Include(t => t.Sponsor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create(int gameId)
        {
            ViewBag.GameId = gameId;
            ViewBag.GameName = _context.Game.Where(c => c.Id == gameId).FirstOrDefault().Name;
            ViewData["SponsorId"] = new SelectList(_context.Sponsor, "Id", "Name");
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int gameId, [Bind("Id,Name,SponsorId")] Team team)
        {
            team.GameId = gameId;
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Teams", new { id = gameId, name = _context.Game.Where(c => c.Id == gameId).FirstOrDefault().Name });
            }
            ViewData["SponsorId"] = new SelectList(_context.Sponsor, "Id", "Name", team.SponsorId);
            return RedirectToAction("Index", "Teams", new { id = gameId, name = _context.Game.Where(c => c.Id == gameId).FirstOrDefault().Name });
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int id, int gameId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Team.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewBag.GameId = gameId;
            ViewData["SponsorId"] = new SelectList(_context.Sponsor, "Id", "Name", team.SponsorId);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int gameId, int id, [Bind("Id,Name,SponsorId,GameId")] Team team)
        {
            team.GameId = gameId;
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Teams", new { id = gameId, name = _context.Game.Where(c => c.Id == gameId).FirstOrDefault().Name });
            }
            ViewData["SponsorId"] = new SelectList(_context.Sponsor, "Id", "Name", team.SponsorId);
            return RedirectToAction("Index", "Teams", new { id = gameId, name = _context.Game.Where(c => c.Id == gameId).FirstOrDefault().Name });
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Team
                .Include(t => t.Game)
                .Include(t => t.Sponsor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Team.FindAsync(id);

            var player = from t in _context.Player
                         where t.TeamId == id
                         select t;
            foreach (var t in player)
            {
                _context.Player.Remove(t);
            }
            await _context.SaveChangesAsync();

            _context.Team.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Teams", new { id = team.GameId, name = _context.Game.Where(c => c.Id == team.GameId).FirstOrDefault().Name });
        }

        private bool TeamExists(int id)
        {
            return _context.Team.Any(e => e.Id == id);
        }

        public async Task<IActionResult> DeleteAllTeams(int? id)
        {
            ViewBag.gameId = id;
            var teamsByGame = _context.Team
                .Where(b => b.GameId == id)
                .Include(b => b.Game)
                .Include(b => b.Sponsor);

            foreach (Team i in teamsByGame)
            {
                _context.Team.Remove(i);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
