using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GraderDataAccessLayer.Models
{
    public class SessionIdModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public Guid SessionId { get; set; }
        public DateTime ExpirationTime { get; set; }

        public SessionIdModel()
        {
            
        }

        public SessionIdModel(int userId)
        {
            SessionId = Guid.NewGuid();
            UserId = userId;
            ExpirationTime = DateTime.UtcNow.AddMinutes(15);
        }
    }
}
