using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GUTAPI.Database
{
    public class CustomDBContext:DbContext 
    {
        public DbSet<TelegramUser> TelegramUsers { get; set;}
        public DbSet<TwitterToken> TwitterTokens { get; set; }
        public DbSet<TwitterMessage> TwitterMessages { get; set; }
        public DbSet<TwitterContact> TwitterContacts { get; set; }
        public DbSet<RedditContact> RedditContacts { get; set; }
        public DbSet<RedditToken> RedditTokens { get; set; }
        public DbSet<ViberContact> ViberContacts { get; set; }
        public DbSet<ViberReceivedMessage> ViberReceivedMessages { get; set; }
        public DbSet<VTcontact> VTcontacts { get; set; }
        public DbSet<TVcontact> TVcontacts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlite("FileName=database.db", option =>
            {
                option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        
        public CustomDBContext(DbContextOptions<CustomDBContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }
        public CustomDBContext() : base()
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelegramUser>().ToTable("TelegramUsers");
            modelBuilder.Entity<TwitterToken>().ToTable("TwitterTokens");
            modelBuilder.Entity<TwitterMessage>().ToTable("TwitterMessages");
            modelBuilder.Entity<TwitterContact>().ToTable("TwitterContacts");
            modelBuilder.Entity<RedditContact>().ToTable("RedditContacts");
            modelBuilder.Entity<RedditToken>().ToTable("RedditTokens");
            modelBuilder.Entity<ViberContact>().ToTable("ViberContacts");
            modelBuilder.Entity<ViberReceivedMessage>().ToTable("ViberReceivedMessages");
            modelBuilder.Entity<VTcontact>().ToTable("VTcontacts");
            modelBuilder.Entity<TVcontact>().ToTable("TVcontacts");

            modelBuilder.Entity<TelegramUser>(entity => {
                entity.HasKey(k => k.id_telegram_user);
            });

            modelBuilder.Entity<TwitterToken>(entity => {
                entity.HasKey(k => k.id);
            });
            modelBuilder.Entity<TwitterMessage>(entity => {
                entity.HasKey(k => k.id_telegram_user);
            });
            modelBuilder.Entity<TwitterContact>(entity => {
                entity.HasKey(k => k.id);
            });

            modelBuilder.Entity<RedditContact>(entity => {
                entity.HasKey(k => k.id);
            });
            modelBuilder.Entity<RedditToken>(entity => {
                entity.HasKey(k => k.id_key);
            });

            modelBuilder.Entity<ViberContact>(entity => {
                entity.HasKey(k => k.id);
            });
            modelBuilder.Entity<ViberReceivedMessage>(entity => {
                entity.HasKey(k => k.Id);
            });

            modelBuilder.Entity<VTcontact>(entity => {
                entity.HasKey(k => k.viber_id);
            });
            modelBuilder.Entity<TVcontact>(entity => {
                entity.HasKey(k => k.tg_id);
            });
            ;
            base.OnModelCreating(modelBuilder);
        }
    }
}
