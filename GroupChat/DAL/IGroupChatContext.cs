using System;
using GroupChat.Models;
using System.Data.Entity;
using System.Web;

namespace GroupChat.DAL
{
    public interface IGroupChatContext : IDisposable
    {
        DbSet<User> Users { get; set; }
        DbSet<Group> Groups { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<UserGroup> UserGroups { get; set; }

        int SaveChanges();


    }
}