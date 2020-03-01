using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyTournaments
{
    public partial class DBTournamentsContext : DbContext
    {
        public DBTournamentsContext()
        {
        }

        public DBTournamentsContext(DbContextOptions<DBTournamentsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<Player> Player { get; set; }
        public virtual DbSet<Sponsor> Sponsor { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<Tournament> Tournament { get; set; }
        public virtual DbSet<TournamentGames> TournamentGames { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-H13FP8L\\SQLEXPRESS; Database=DBTournaments; Trusted_Connection=True; ");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Info)
                    .HasColumnName("info")
                    .HasColumnType("ntext");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("ntext");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EntranceDate)
                    .HasColumnName("entranceDate")
                    .HasColumnType("date");

                entity.Property(e => e.Info)
                    .HasColumnName("info")
                    .HasColumnType("ntext");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("ntext");

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasColumnName("position")
                    .HasColumnType("ntext");

                entity.Property(e => e.TeamId).HasColumnName("teamId");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Player)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Player_Team");
            });

            modelBuilder.Entity<Sponsor>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Info)
                    .HasColumnName("info")
                    .HasColumnType("ntext");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("ntext");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GameId).HasColumnName("gameId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("ntext");

                entity.Property(e => e.SponsorId).HasColumnName("sponsorId");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Team_Game");

                entity.HasOne(d => d.Sponsor)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.SponsorId)
                    .HasConstraintName("FK_Team_Sponsor");
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnName("location")
                    .HasColumnType("ntext");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("ntext");

                entity.Property(e => e.PrizeFund)
                    .HasColumnName("prizeFund")
                    .HasColumnType("money");

                entity.Property(e => e.SponsorId).HasColumnName("sponsorId");

                entity.HasOne(d => d.Sponsor)
                    .WithMany(p => p.Tournament)
                    .HasForeignKey(d => d.SponsorId)
                    .HasConstraintName("FK_Tournament_Sponsor");
            });

            modelBuilder.Entity<TournamentGames>(entity =>
            {
             
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TournametId).HasColumnName("tournametId");

                entity.Property(e => e.GameId).HasColumnName("gameId");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.TournamentGames)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TournamentGames_Game");

                entity.HasOne(d => d.Tournamet)
                    .WithMany(p => p.TournamentGames)
                    .HasForeignKey(d => d.TournametId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TournamentGames_Tournament");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
