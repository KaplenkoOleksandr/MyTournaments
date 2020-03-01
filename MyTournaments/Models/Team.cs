using System;
using System.Collections.Generic;

namespace MyTournaments
{
    public partial class Team
    {
        public Team()
        {
            Player = new HashSet<Player>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? SponsorId { get; set; }
        public int GameId { get; set; }

        public virtual Game Game { get; set; }
        public virtual Sponsor Sponsor { get; set; }
        public virtual ICollection<Player> Player { get; set; }
    }
}
