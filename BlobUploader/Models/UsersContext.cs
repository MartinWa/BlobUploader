﻿using System.Data.Entity;

namespace BlobUploader.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext() : base("DatabaseConnection") { }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Membership>()
                .HasMany<Role>(r => r.Roles)
                .WithMany(u => u.Members)
                .Map(m =>
                {
                    m.ToTable("webpages_UsersInRoles");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                });
        }
    }
}