using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GraderDataAccessLayer.Models
{
    public class SessionIdModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        private int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        private Guid SessionId { get; set; }
        public DateTime ExpirationTime { get; set; }

        public SessionIdModel()
        {
            SessionId = Guid.NewGuid();
            ExpirationTime = DateTime.UtcNow.AddMinutes(15);
        }
    }
}
