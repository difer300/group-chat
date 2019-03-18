using GroupChat.Models;
using GroupChat.Settings;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GroupChat.DAL
{
    public class GroupChatContext : DbContext
    {
        public GroupChatContext() : base(AppSettings.ConnectionName)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}