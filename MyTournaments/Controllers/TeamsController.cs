using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Xceed.Document.NET;
using Xceed.Words.NET;

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
            if (id == null) return View(await _context.Team.ToListAsync());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null && fileExcel.FileName.EndsWith(".xlsx"))
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                if (worksheet.Name == "Games")
                                {
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            if(row.Cell(1).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            foreach (var g in _context.Game)
                                            {
                                                if (g.Name == row.Cell(1).Value.ToString()) { throw new Exception(); }
                                            }
                                            Game game = new Game();
                                            game.Name = row.Cell(1).Value.ToString();
                                            game.Info = row.Cell(2).Value.ToString();
                                            _context.Game.Add(game);
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                                if (worksheet.Name == "Sponsors")
                                {
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            if (row.Cell(1).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            foreach (var s in _context.Sponsor)
                                            {
                                                if (s.Name == row.Cell(1).Value.ToString()) { throw new Exception(); }
                                            }
                                            Sponsor sponsor = new Sponsor();
                                            sponsor.Name = row.Cell(1).Value.ToString();
                                            sponsor.Info = row.Cell(2).Value.ToString();
                                            _context.Sponsor.Add(sponsor);
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                                if (worksheet.Name == "Teams")
                                {
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            if (row.Cell(1).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            if (row.Cell(2).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }

                                            foreach (var t in _context.Team)
                                            {
                                                if (t.Name == row.Cell(1).Value.ToString()) { throw new Exception(); }
                                            }
                                            int tempGameId = -1;
                                            foreach (var g in _context.Game)
                                            {
                                                if (g.Name == row.Cell(2).Value.ToString()) { tempGameId = g.Id; break; }
                                            }
                                            if (tempGameId == -1)
                                            {
                                                Game game = new Game();
                                                game.Name = row.Cell(2).Value.ToString();
                                                game.Info = "from Excel TEAM";
                                                _context.Game.Add(game);
                                                await _context.SaveChangesAsync();
                                                foreach (var g in _context.Game)
                                                {
                                                    if (g.Name == game.Name) { tempGameId = g.Id; break; }
                                                }
                                            }

                                            Team team = new Team();

                                            if (row.Cell(3).Value.ToString().Replace(" ", "") != "")
                                            {
                                                foreach (var s in _context.Sponsor)
                                                {
                                                    if (s.Name == row.Cell(3).Value.ToString()) { team.SponsorId = s.Id; break; }
                                                }
                                                if (team.SponsorId == null)
                                                {
                                                    Sponsor sponsor = new Sponsor();
                                                    sponsor.Name = row.Cell(3).Value.ToString();
                                                    sponsor.Info = "from Excel";
                                                    _context.Sponsor.Add(sponsor);
                                                    await _context.SaveChangesAsync();
                                                    foreach (var g in _context.Sponsor)
                                                    {
                                                        if (g.Name == sponsor.Name) { team.SponsorId = g.Id; break; }
                                                    }
                                                }
                                            }
                                            team.Name = row.Cell(1).Value.ToString();
                                            team.GameId = tempGameId;
                                            _context.Team.Add(team);
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                                if (worksheet.Name == "Players")
                                {
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            if (row.Cell(1).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            if (row.Cell(2).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            if (row.Cell(3).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            if (row.Cell(4).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            if (row.Cell(5).Value.ToString().Replace(" ", "") == "" || !DateTime.TryParse(row.Cell(5).Value.ToString(), out DateTime dateValue)) { throw new Exception(); }

                                            foreach (var p in _context.Player)
                                            {
                                                if (p.Name == row.Cell(1).Value.ToString()) { throw new Exception(); }
                                            }
                                            int tempGameId = -1;
                                            foreach (var g in _context.Game)
                                            {
                                                if (g.Name == row.Cell(2).Value.ToString()) { tempGameId = g.Id; break; }
                                            }
                                            if (tempGameId == -1)
                                            {
                                                Game game = new Game();
                                                game.Name = row.Cell(2).Value.ToString();
                                                game.Info = "from Excel PLAYER";
                                                _context.Game.Add(game);
                                                await _context.SaveChangesAsync();
                                                foreach (var g in _context.Game)
                                                {
                                                    if (g.Name == game.Name) { tempGameId = g.Id; break; }
                                                }
                                            }
                                            int tempTeamId = -1;
                                            foreach (var t in _context.Team)
                                            {
                                                if (t.Name == row.Cell(3).Value.ToString()) { tempTeamId = t.Id; break; }
                                            }
                                            if (tempTeamId == -1)
                                            {
                                                Team team = new Team();
                                                team.Name = row.Cell(3).Value.ToString();
                                                team.GameId = tempGameId;
                                                _context.Team.Add(team);
                                                await _context.SaveChangesAsync();
                                                foreach (var g in _context.Team)
                                                {
                                                    if (g.Name == team.Name) { tempTeamId = g.Id; break; }
                                                }
                                            }
                                            Player player = new Player();
                                            player.Name = row.Cell(1).Value.ToString();
                                            player.Info = row.Cell(6).Value.ToString();
                                            player.Position = row.Cell(4).Value.ToString();
                                            player.EntranceDate = dateValue;
                                            player.TeamId = tempTeamId;
                                            _context.Player.Add(player);
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                                if (worksheet.Name == "Tournaments")
                                {
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            if (row.Cell(1).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            if (row.Cell(2).Value.ToString().Replace(" ", "") == "") { throw new Exception(); }
                                            if (row.Cell(3).Value.ToString().Replace(" ", "") == "" || !int.TryParse(row.Cell(3).Value.ToString(), out int prize)) { throw new Exception(); }

                                            foreach (var t in _context.Tournament)
                                            {
                                                if (t.Name == row.Cell(1).Value.ToString())
                                                {
                                                    throw new Exception();
                                                }
                                            }
                                            Tournament tournament = new Tournament();
                                            if (row.Cell(4).Value.ToString().Replace(" ", "") != "")
                                            {
                                                foreach (var s in _context.Sponsor)
                                                {
                                                    if (s.Name == row.Cell(4).Value.ToString()) { tournament.SponsorId = s.Id; break; }
                                                }
                                                if (tournament.SponsorId == null)
                                                {
                                                    Sponsor sponsor = new Sponsor();
                                                    sponsor.Name = row.Cell(4).Value.ToString();
                                                    sponsor.Info = "from Excel";
                                                    _context.Sponsor.Add(sponsor);
                                                    await _context.SaveChangesAsync();
                                                    foreach (var g in _context.Sponsor)
                                                    {
                                                        if (g.Name == sponsor.Name) { tournament.SponsorId = g.Id; break; }
                                                    }
                                                }
                                            }
                                            tournament.Name = row.Cell(1).Value.ToString();
                                            tournament.Location = row.Cell(2).Value.ToString();
                                            tournament.PrizeFund = prize;
                                            _context.Tournament.Add(tournament);
                                            await _context.SaveChangesAsync();

                                            int tempTournamentId = -1;
                                            foreach (var t in _context.Tournament)
                                            {
                                                if (t.Name == tournament.Name) { tempTournamentId = t.Id; break; }
                                            }
                                            string[] tournGames = row.Cell(5).Value.ToString().Replace(" ", "").Split(',');
                                            foreach(var tg in tournGames)
                                            {
                                                int gameId = -1;
                                                foreach (var g in _context.Game)
                                                {
                                                    if (g.Name == tg) { gameId = g.Id; break; }
                                                }
                                                if(gameId == -1)
                                                {
                                                    if(tg == "") { throw new Exception(); }
                                                    Game game = new Game();
                                                    game.Name = tg;
                                                    game.Info = "from Excel";
                                                    _context.Game.Add(game);
                                                    await _context.SaveChangesAsync();
                                                    foreach (var g in _context.Game)
                                                    {
                                                        if (g.Name == game.Name) { gameId = g.Id; break; }
                                                    }
                                                }
                                                TournamentGames tournamentGames = new TournamentGames();
                                                tournamentGames.GameId = gameId;
                                                tournamentGames.TournametId = tempTournamentId;
                                                _context.TournamentGames.Add(tournamentGames);
                                            }
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                                if (worksheet.Name == "Tournaments")
                                {
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            int tempTournamentId = -1;
                                            foreach (var t in _context.Tournament)
                                            {
                                                if (t.Name == row.Cell(1).Value.ToString()) { tempTournamentId = t.Id; break; }
                                            }
                                            string[] tournGames = row.Cell(5).Value.ToString().Replace(" ", "").Split(',');
                                            foreach (var tg in tournGames)
                                            {
                                                int gameId = -1;
                                                foreach (var g in _context.Game)
                                                {
                                                    if (g.Name == tg) { gameId = g.Id; break; }
                                                }
                                                if (gameId == -1)
                                                {
                                                    if (tg == "") { throw new Exception(); }
                                                    Game game = new Game();
                                                    game.Name = tg;
                                                    game.Info = "from Excel";
                                                    _context.Game.Add(game);
                                                    await _context.SaveChangesAsync();
                                                    foreach (var g in _context.Game)
                                                    {
                                                        if (g.Name == game.Name) { gameId = g.Id; break; }
                                                    }
                                                }
                                                TournamentGames tournamentGames = new TournamentGames();
                                                tournamentGames.GameId = gameId;
                                                tournamentGames.TournametId = tempTournamentId;
                                                foreach(var tog in _context.TournamentGames)
                                                {
                                                    if(tog.GameId == gameId && tog.TournametId == tempTournamentId)
                                                    {
                                                        throw new Exception();
                                                    }
                                                }
                                                _context.TournamentGames.Add(tournamentGames);
                                            }
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Games");
        }

        public ActionResult Export(List<int> selectedTeams, List<int> selectedPlayers, List<int> selectedGames, List<int> selectedSponsors, List<int> selectedTournaments)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var worksheet1 = workbook.Worksheets.Add("Games");
                int gameCounter = 2;
                foreach (var g in _context.Game)
                {
                    worksheet1.Cell("A1").Value = "Name";
                    worksheet1.Cell("B1").Value = "Info";
                    for (int i = 0; i < _context.Game.Count(); i++)
                    {
                        try
                        {
                            if (selectedGames.Contains(g.Id))
                            {
                                worksheet1.Cell(gameCounter, 1).Value = g.Name;
                                worksheet1.Cell(gameCounter, 2).Value = g.Info;

                                gameCounter++;
                                selectedGames.Remove(g.Id);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                var worksheet2 = workbook.Worksheets.Add("Sponsors");
                int sponsorCounter = 2;
                foreach (var s in _context.Sponsor)
                {
                    worksheet2.Cell("A1").Value = "Name";
                    worksheet2.Cell("B1").Value = "Info";
                    for (int i = 0; i < _context.Sponsor.Count(); i++)
                    {
                        try
                        {
                            if (selectedSponsors.Contains(s.Id))
                            {
                                worksheet2.Cell(sponsorCounter, 1).Value = s.Name;
                                worksheet2.Cell(sponsorCounter, 2).Value = s.Info;

                                sponsorCounter++;
                                selectedSponsors.Remove(s.Id);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                var worksheet3 = workbook.Worksheets.Add("Teams");
                int teamCounter = 2;
                foreach(var t in _context.Team)
                {
                    worksheet3.Cell("A1").Value = "Name";
                    worksheet3.Cell("B1").Value = "Game";
                    worksheet3.Cell("C1").Value = "Sponsor";
                    for (int i = 0; i < _context.Team.Count(); i++)
                    {
                        try
                        {
                            if(selectedTeams.Contains(t.Id))
                            {
                                string GameName = _context.Game.Where(c => c.Id == t.GameId).FirstOrDefault().Name;
                                if(GameName == null) { throw new Exception(); }
                                List<Sponsor> sponsor = _context.Sponsor.Where(c => c.Id == t.SponsorId).ToList();
                                string sponsorName;
                                if (sponsor.Count() == 0) { sponsorName = ""; }
                                else { sponsorName = sponsor[0].Name; }
                                worksheet3.Cell(teamCounter, 1).Value = t.Name;
                                worksheet3.Cell(teamCounter, 2).Value = GameName;
                                worksheet3.Cell(teamCounter, 3).Value = sponsorName;

                                teamCounter++;
                                selectedTeams.Remove(t.Id);
                            }
                        }
                        catch(Exception e)
                        {

                        }
                    }
                }
                var worksheet4 = workbook.Worksheets.Add("Players");
                int playerCounter = 2;
                foreach (var p in _context.Player)
                {
                    worksheet4.Cell("A1").Value = "Name";
                    worksheet4.Cell("B1").Value = "Game";
                    worksheet4.Cell("C1").Value = "Team";
                    worksheet4.Cell("D1").Value = "Position";
                    worksheet4.Cell("E1").Value = "EntranceDate";
                    worksheet4.Cell("F1").Value = "Info";
                    for (int i = 0; i < _context.Player.Count(); i++)
                    {
                        try
                        {
                            if (selectedPlayers.Contains(p.Id))
                            {
                                string TeamName = _context.Team.Where(c => c.Id == p.TeamId).FirstOrDefault().Name;
                                if (TeamName == null) { throw new Exception(); }
                                Team gameId = _context.Team.Where(c => c.Id == p.TeamId).FirstOrDefault();
                                string GameName = _context.Game.Where(c => c.Id == gameId.GameId).FirstOrDefault().Name;
                                if (GameName == null) { throw new Exception(); }

                                worksheet4.Cell(playerCounter, 1).Value = p.Name;
                                worksheet4.Cell(playerCounter, 2).Value = GameName; 
                                worksheet4.Cell(playerCounter, 3).Value = TeamName;
                                worksheet4.Cell(playerCounter, 4).Value = p.Position;
                                worksheet4.Cell(playerCounter, 5).Value = p.EntranceDate;
                                worksheet4.Cell(playerCounter, 6).Value = p.Info;

                                playerCounter++;
                                selectedPlayers.Remove(p.Id);
                            }
                        }
                        catch (Exception e)
                        {
                            worksheet4.Cell("A1").Value = "!!!!!!!!!!";
                        }
                    }
                }
                var worksheet5 = workbook.Worksheets.Add("Tournaments");
                int tournamentCounter = 2;
                foreach (var t in _context.Tournament)
                {
                    worksheet5.Cell("A1").Value = "Name";
                    worksheet5.Cell("B1").Value = "Location";
                    worksheet5.Cell("C1").Value = "Prize";
                    worksheet5.Cell("D1").Value = "Sponsor";
                    worksheet5.Cell("E1").Value = "Games";
                    for (int i = 0; i < _context.Tournament.Count(); i++)
                    {
                        try
                        {
                            if (selectedTournaments.Contains(t.Id))
                            {
                                Sponsor SponsorName = _context.Sponsor.Where(c => c.Id == t.SponsorId).FirstOrDefault();

                                worksheet5.Cell(tournamentCounter, 1).Value = t.Name;
                                worksheet5.Cell(tournamentCounter, 2).Value = t.Location;
                                worksheet5.Cell(tournamentCounter, 3).Value = t.PrizeFund;
                                worksheet5.Cell(tournamentCounter, 4).Value = SponsorName.Name;
                                string gameString = "";
                                foreach(var tg in _context.TournamentGames)
                                {
                                    if(tg.TournametId == t.Id)
                                    {
                                        gameString += _context.Game.Where(c => c.Id == tg.GameId).FirstOrDefault().Name + ",";
                                    }
                                }
                                worksheet5.Cell(tournamentCounter, 5).Value = gameString;

                                tournamentCounter++;
                                selectedTournaments.Remove(t.Id);
                            }
                        }
                        catch (Exception e)
                        {
                            
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportDocx(List<int> selectedTeams, List<int> selectedPlayers, List<int> selectedGames, List<int> selectedSponsors, List<int> selectedTournaments)
        {
            // Create a document.
            using (DocX document = DocX.Create(@"Test.docx"))
            {
                // Create a Table game.
                Table game = document.AddTable(selectedGames.Count()+1, 2);
                game.Design = TableDesign.ColorfulGrid;
                game.Alignment = Alignment.center;
                int gameCounter = 1;

                game.Rows[0].Cells[0].Paragraphs.First().Append("Game");
                game.Rows[0].Cells[1].Paragraphs.First().Append("Info");

                foreach (var g in _context.Game)
                {
                    for (int i = 0; i < _context.Game.Count(); i++)
                    {
                        try
                        {
                            if (selectedGames.Contains(g.Id))
                            {
                                game.Rows[gameCounter].Cells[0].Paragraphs.First().Append(g.Name);
                                game.Rows[gameCounter].Cells[1].Paragraphs.First().Append(g.Info);

                                gameCounter++;
                                selectedGames.Remove(g.Id);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                

                // Create a Table teams.
                Table teams = document.AddTable(selectedTeams.Count() + 1, 3);
                teams.Design = TableDesign.ColorfulList;
                teams.Alignment = Alignment.center;
                int teamsCounter = 1;

                teams.Rows[0].Cells[0].Paragraphs.First().Append("Name");
                teams.Rows[0].Cells[1].Paragraphs.First().Append("Game");
                teams.Rows[0].Cells[2].Paragraphs.First().Append("Sponsor");

                foreach (var t in _context.Team)
                {
                    for (int i = 0; i < _context.Team.Count(); i++)
                    {
                        try
                        {
                            if (selectedTeams.Contains(t.Id))
                            {
                                string GameName = _context.Game.Where(c => c.Id == t.GameId).FirstOrDefault().Name;
                                if (GameName == null) { throw new Exception(); }
                                List<Sponsor> sponsor = _context.Sponsor.Where(c => c.Id == t.SponsorId).ToList();
                                string sponsorName;
                                if (sponsor.Count() == 0) { sponsorName = ""; }
                                else { sponsorName = sponsor[0].Name; }

                                teams.Rows[teamsCounter].Cells[0].Paragraphs.First().Append(t.Name);
                                teams.Rows[teamsCounter].Cells[1].Paragraphs.First().Append(GameName);
                                teams.Rows[teamsCounter].Cells[2].Paragraphs.First().Append(sponsorName);

                                teamsCounter++;
                                selectedTeams.Remove(t.Id);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
               

                // Create a Table players.
                Table players = document.AddTable(selectedPlayers.Count() + 1, 6);
                players.Design = TableDesign.ColorfulShading;
                players.Alignment = Alignment.center;
                int playersCounter = 1;

                players.Rows[0].Cells[0].Paragraphs.First().Append("Name");
                players.Rows[0].Cells[1].Paragraphs.First().Append("Position");
                players.Rows[0].Cells[2].Paragraphs.First().Append("Info");
                players.Rows[0].Cells[3].Paragraphs.First().Append("EntranceDate");
                players.Rows[0].Cells[4].Paragraphs.First().Append("Team");
                players.Rows[0].Cells[5].Paragraphs.First().Append("Game");

                foreach (var p in _context.Player)
                {
                    for (int i = 0; i < _context.Player.Count(); i++)
                    {
                        try
                        {
                            if (selectedPlayers.Contains(p.Id))
                            {
                                string TeamName = _context.Team.Where(c => c.Id == p.TeamId).FirstOrDefault().Name;
                                if (TeamName == null) { throw new Exception(); }
                                Team gameId = _context.Team.Where(c => c.Id == p.TeamId).FirstOrDefault();
                                string GameName = _context.Game.Where(c => c.Id == gameId.GameId).FirstOrDefault().Name;
                                if (GameName == null) { throw new Exception(); }

                                players.Rows[playersCounter].Cells[0].Paragraphs.First().Append(p.Name);
                                players.Rows[playersCounter].Cells[1].Paragraphs.First().Append(p.Position);
                                players.Rows[playersCounter].Cells[2].Paragraphs.First().Append(p.Info);
                                players.Rows[playersCounter].Cells[3].Paragraphs.First().Append(p.EntranceDate.ToString());
                                players.Rows[playersCounter].Cells[4].Paragraphs.First().Append(TeamName);
                                players.Rows[playersCounter].Cells[5].Paragraphs.First().Append(GameName);

                                playersCounter++;
                                selectedPlayers.Remove(p.Id);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                

                // Create a Table sponsors.
                Table sponsors = document.AddTable(selectedSponsors.Count() + 1, 2);
                sponsors.Design = TableDesign.DarkList;
                sponsors.Alignment = Alignment.center;
                int sponsorsCounter = 1;

                sponsors.Rows[0].Cells[0].Paragraphs.First().Append("Name");
                sponsors.Rows[0].Cells[1].Paragraphs.First().Append("Info");

                foreach (var s in _context.Sponsor)
                {
                    for (int i = 0; i < _context.Sponsor.Count(); i++)
                    {
                        try
                        {
                            if (selectedSponsors.Contains(s.Id))
                            {
                                sponsors.Rows[sponsorsCounter].Cells[0].Paragraphs.First().Append(s.Name);
                                sponsors.Rows[sponsorsCounter].Cells[1].Paragraphs.First().Append(s.Info);

                                sponsorsCounter++;
                                selectedSponsors.Remove(s.Id);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                

                // Create a Table tournaments.
                Table tournaments = document.AddTable(selectedTournaments.Count() + 1, 5);
                tournaments.Design = TableDesign.DarkListAccent5;
                tournaments.Alignment = Alignment.center;
                int tournamentsCounter = 1;

                tournaments.Rows[0].Cells[0].Paragraphs.First().Append("Name");
                tournaments.Rows[0].Cells[1].Paragraphs.First().Append("Location");
                tournaments.Rows[0].Cells[2].Paragraphs.First().Append("PrizeFund");
                tournaments.Rows[0].Cells[3].Paragraphs.First().Append("Sponsor");
                tournaments.Rows[0].Cells[4].Paragraphs.First().Append("Game");

                foreach (var t in _context.Tournament)
                {
                    for (int i = 0; i < _context.Tournament.Count(); i++)
                    {
                        try
                        {
                            if (selectedTournaments.Contains(t.Id))
                            {
                                Sponsor SponsorName = _context.Sponsor.Where(c => c.Id == t.SponsorId).FirstOrDefault();

                                tournaments.Rows[tournamentsCounter].Cells[0].Paragraphs.First().Append(t.Name);
                                tournaments.Rows[tournamentsCounter].Cells[1].Paragraphs.First().Append(t.Location);
                                tournaments.Rows[tournamentsCounter].Cells[2].Paragraphs.First().Append(t.PrizeFund.ToString());
                                tournaments.Rows[tournamentsCounter].Cells[3].Paragraphs.First().Append(SponsorName.Name);

                                string gameString = "";
                                foreach (var tg in _context.TournamentGames)
                                {
                                    if (tg.TournametId == t.Id)
                                    {
                                        gameString += _context.Game.Where(c => c.Id == tg.GameId).FirstOrDefault().Name + ",";
                                    }
                                }
                                tournaments.Rows[tournamentsCounter].Cells[4].Paragraphs.First().Append(gameString);

                                tournamentsCounter++;
                                selectedTournaments.Remove(t.Id);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                document.InsertTable(game);
                document.InsertTable(players);
                document.InsertTable(teams);
                document.InsertTable(tournaments);
                document.InsertTable(sponsors);

                using (var stream = new MemoryStream())
                {
                    document.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.docx"
                    };
                }
            }
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