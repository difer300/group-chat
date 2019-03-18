using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupChat.Models
{
    public class Message
    {
        public int ID { get; set; }

        [ForeignKey("AddedBy_ID")]
        public User AddedBy { get; set; }

        [Required]
        public int AddedBy_ID { get; set; }

        public string Text { get; set; }

        [ForeignKey("Group_ID")]
        public Group Group { get; set; }

        [Required]
        public int Group_ID { get; set; }

        public bool IsBoot { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}