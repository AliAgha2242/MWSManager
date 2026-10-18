using Microsoft.EntityFrameworkCore;
using Model.ApplicationModels;
using Model.PublicModels.LogModels;
using MWSMan.Core.Model.PublicModels.LogModels;

namespace Persistence
{
    public class MinooDbContext : DbContext
    {
        public DbSet<ProgramLogModel> RabbitConsumerLogDetail { get; set; }
        public DbSet<RabbitLogModel> RabbitConsumerLog { get; set; }
        public DbSet<RabbitMessage> RabbitMessages { get; set; }
        public DbSet<RabbitMessageType> RabbitMessageTypes { get; set; }
        public DbSet<RabbitTasks> RabbitTasks { get; set; }
        public DbSet<RabbitTaskServers> RabbitTaskServers { get; set; }

        public DbSet<RabbitConsumerRequestHub> RabbitConsumerRequestHub { get; set; }
        public MinooDbContext(DbContextOptions<MinooDbContext> options) :base(options)
        {   
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            modelBuilder.Entity<ProgramLogModel>().ToTable(t=>t.ExcludeFromMigrations());
            modelBuilder.Entity<RabbitLogModel>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<RabbitMessage>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<RabbitMessageType>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<RabbitConsumerRequestHub>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<RabbitTasks>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<RabbitTaskServers>().ToTable(t => t.ExcludeFromMigrations());





            //modelBuilder.Entity<ProgramLogModel>().ToTable("RabbitConsumerLogDetail");
            //modelBuilder.Entity<RabbitLogModel>().ToTable("RabbitConsumerLog");
            //modelBuilder.Entity<RabbitMessage>().ToTable("RabbitMessage");
            //modelBuilder.Entity<RabbitMessageType>().ToTable("RabbitMessageType");
            //modelBuilder.Entity<RabbitConsumerRequestHub>().ToTable("RabbitConsumerRequestHub");
            //modelBuilder.Entity<RabbitTasks>().Property(p => p.IntervalTotalMinute).HasComputedColumnSql("[IntervalMinute] + (ISNULL([IntervalHour],0) * 60)");

            modelBuilder.Entity<RabbitTasks>().HasMany(p => p.RabbitTaskServerss).WithOne(s => s.RabbitTasks).HasForeignKey(p => p.RabbitTaskId);
            modelBuilder.Entity<RabbitTasks>().HasOne(p => p.rabbitConsumerRequestHub).WithMany(s => s.RabbitTasks).HasForeignKey(p => p.RabbitConsumerRequestHubId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
