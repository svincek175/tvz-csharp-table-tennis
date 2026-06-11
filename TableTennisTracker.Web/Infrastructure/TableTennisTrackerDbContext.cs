using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure.Identity;

namespace TableTennisTracker.Web.Infrastructure;

public class TableTennisTrackerDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public TableTennisTrackerDbContext(DbContextOptions<TableTennisTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();
    public DbSet<MatchSetResult> MatchSetResults => Set<MatchSetResult>();
    public DbSet<QuizFile> QuizFiles => Set<QuizFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tournament>()
            .HasOne(t => t.Venue)
            .WithMany(v => v.Tournaments)
            .HasForeignKey(t => t.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Player)
            .WithMany(p => p.Registrations)
            .HasForeignKey(r => r.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Tournament)
            .WithMany(t => t.Registrations)
            .HasForeignKey(r => r.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Registration>()
            .HasIndex(r => new { r.PlayerId, r.TournamentId })
            .IsUnique();

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Tournament)
            .WithMany(t => t.Matches)
            .HasForeignKey(m => m.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.WinnerPlayer)
            .WithMany()
            .HasForeignKey(m => m.WinnerPlayerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<MatchParticipant>()
            .HasOne(mp => mp.Match)
            .WithMany(m => m.Participants)
            .HasForeignKey(mp => mp.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchParticipant>()
            .HasOne(mp => mp.Player)
            .WithMany(p => p.MatchParticipations)
            .HasForeignKey(mp => mp.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchParticipant>()
            .HasIndex(mp => new { mp.MatchId, mp.PlayerId })
            .IsUnique();

        modelBuilder.Entity<MatchParticipant>()
            .HasIndex(mp => new { mp.MatchId, mp.Slot })
            .IsUnique();

        modelBuilder.Entity<MatchSetResult>()
            .HasOne(sr => sr.Match)
            .WithMany(m => m.SetResults)
            .HasForeignKey(sr => sr.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchSetResult>()
            .HasIndex(sr => new { sr.MatchId, sr.SetNumber })
            .IsUnique();

        modelBuilder.Entity<QuizFile>()
            .HasIndex(q => q.UploadedUtc);
    }
}
