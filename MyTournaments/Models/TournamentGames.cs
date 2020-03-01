using System;
using System.Collections.Generic;

namespace MyTournaments
{
    public partial class TournamentGames
    {
        public int Id { get; set; }
        public int TournametId { get; set; }
        public int GameId { get; set; }

        public virtual Game Game { get; set; }
        public virtual Tournament Tournamet { get; set; }
    }
}
