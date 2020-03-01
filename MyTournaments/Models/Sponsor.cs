using System;
using System.Collections.Generic;

namespace MyTournaments
{
    public partial class Sponsor
    {
        public Sponsor()
        {
            Team = new HashSet<Team>();
            Tournament = new HashSet<Tournament>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }

        public virtual ICollection<Team> Team { get; set; }
        public virtual ICollection<Tournament> Tournament { get; set; }
    }
}
