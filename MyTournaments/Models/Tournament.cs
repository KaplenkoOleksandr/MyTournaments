using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace MyTournaments
{
    public partial class Tournament
    {
        public Tournament()
        {
            TournamentGames = new HashSet<TournamentGames>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "!@#$%^&")]
        public string Name { get; set; }
        public int? SponsorId { get; set; }
        [Required(ErrorMessage = "!@#$%^&")]
        public string Location { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public decimal PrizeFund { get; set; }

        public virtual Sponsor Sponsor { get; set; }
        public virtual ICollection<TournamentGames> TournamentGames { get; set; }
    }
}
