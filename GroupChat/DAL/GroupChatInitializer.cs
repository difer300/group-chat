using GroupChat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace GroupChat.DAL
{
    public class GroupChatInitializer : DropCreateDatabaseIfModelChanges<GroupChatContext>
    {
        protected override void Seed(GroupChatContext context)
        {
            var users = new List<User>
            {
                new User() {ID= 1, Name ="Bot"},
                new User() {ID=2, Name ="Diana"},
                new User() {ID=3, Name ="Alex"},
            };

            users.ForEach(x => context.Users.Add(x));
            context.SaveChanges();

            var groups = new List<Group>
            {
                new Group() {ID = 1, Name ="Work"},
                new Group() {ID = 2, Name ="Fun"},
                new Group() {ID = 3, Name ="Party"},
            };

            groups.ForEach(x => context.Groups.Add(x));
            context.SaveChanges();

            var userGroups = new List<UserGroup>
            {
                new UserGroup() {ID = 1, GroupId = 1, UserId = 1},
                new UserGroup() {ID = 2, GroupId = 1, UserId = 2},
                new UserGroup() {ID = 3, GroupId = 1, UserId = 3},
                new UserGroup() {ID = 4, GroupId = 2, UserId = 1},
                new UserGroup() {ID = 5, GroupId = 2, UserId = 2},
                new UserGroup() {ID = 6, GroupId = 2, UserId = 3},
                new UserGroup() {ID = 7, GroupId = 3, UserId = 1},
                new UserGroup() {ID = 8, GroupId = 3, UserId = 2},
                new UserGroup() {ID = 9, GroupId = 3, UserId = 3},
            };

            userGroups.ForEach(x => context.UserGroups.Add(x));
            context.SaveChanges();

            var messages = new List<Message>
            {
                new Message() {AddedBy = users[1], CreatedDate = DateTime.Now, Group = groups[0], Text = "Hi guys!!", IsBoot = false},
                new Message() {AddedBy = users[1], CreatedDate = DateTime.Now, Group = groups[0], Text = "APPL quote is $93.42 per share", IsBoot = true}
            };

            messages.ForEach(x => context.Messages.Add(x));
            context.SaveChanges();
        }
    }
}