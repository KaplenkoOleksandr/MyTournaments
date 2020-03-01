using System;
using System.Collections.Generic;

namespace MyTournaments
{
    public partial class Game
    {
        public Game()
        {
            Team = new HashSet<Team>();
            TournamentGames = new HashSet<TournamentGames>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }

        public virtual ICollection<Team> Team { get; set; }
        public virtual ICollection<TournamentGames> TournamentGames { get; set; }
    }
}
