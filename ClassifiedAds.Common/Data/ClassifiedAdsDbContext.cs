using ClassifiedAds.Common.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClassifiedAds.Common.Data;

public class ClassifiedAdsDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    public DbSet<Ad> Ads { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<FlaggedAd> FlaggedAds { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DevicePublicKey>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.MemberId, x.DeviceId })
                  .IsUnique(); // ensures 1 key per device

            entity.Property(x => x.DeviceId).IsRequired();
            entity.Property(x => x.PublicKeyBase64).IsRequired();
        });
        modelBuilder.Entity<Photo>().HasQueryFilter(x => x.IsApproved);

        modelBuilder.Entity<IdentityRole>()
        .HasData(
            new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "1ef432b8-2148-4d6a-9240-9a257206c87c" },
            new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = "d5a6b89f-94d6-40d7-8e0e-7f0e9f3b4d1e" },
            new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "a3b7c24f-7e12-4b3a-9f5d-6e8f2a1b3c4d" }
        );

        modelBuilder.Entity<Message>()
            .HasOne(x => x.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ad>()
          .HasOne(a => a.Member)       // An Ad has one Member
          .WithMany()                  // A Member has many Ads (but no property in Member class)
          .HasForeignKey(a => a.MemberId) // Use the new String Foreign Key
          .OnDelete(DeleteBehavior.Cascade); // If Member is deleted, delete their Ads

        modelBuilder.Entity<Ad>(b =>
        {
            b.Property(a => a.CreatedDate).HasColumnType("timestamp with time zone");
            b.Property(a => a.ModifiedDate).HasColumnType("timestamp with time zone");


        });

        // Photo -> Ad
        modelBuilder.Entity<Photo>()
            .HasOne(p => p.Ad)
            .WithMany(a => a.Photos)
            .HasForeignKey(p => p.AdId)
            .OnDelete(DeleteBehavior.Cascade);

        // Photo -> Member (profile photos)
        modelBuilder.Entity<Photo>()
            .HasOne(p => p.Member)
            .WithMany(m => m.Photos)
            .HasForeignKey(p => p.MemberId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .Property(x => x.Content)
            .HasConversion(new EncryptedConverter());

        modelBuilder.Entity<MemberLike>()
            .HasKey(x => new { x.SourceMemberId, x.TargetMemberId });

        modelBuilder.Entity<MemberLike>()
            .HasOne(s => s.SourceMember)
            .WithMany(t => t.LikedMembers)
            .HasForeignKey(s => s.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberLike>()
            .HasOne(s => s.TargetMember)
            .WithMany(t => t.LikedByMembers)
            .HasForeignKey(s => s.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}
