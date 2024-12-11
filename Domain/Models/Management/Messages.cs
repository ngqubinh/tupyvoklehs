using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Auth;

namespace Domain.Models.Management
{
    public class Messages
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}