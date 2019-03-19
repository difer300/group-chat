using GroupChat.Models;
using GroupChat.Settings;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GroupChat.DAL
{
    public class GroupChatContext : DbContext, IGroupChatContext
    {
        public GroupChatContext() : base(AppSettings.ConnectionName)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}