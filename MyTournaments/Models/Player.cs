using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace MyTournaments
{
    public partial class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Info { get; set; }
        public int TeamId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'.'MM'.'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EntranceDate { get; set; }

        public virtual Team Team { get; set; }
    }
}
