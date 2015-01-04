using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GraderDataAccessLayer.Models
{
    public class SessionIdModel
    {
        public SessionIdModel()
        {
        }

        public SessionIdModel(int userId)
        {
            SessionId = Guid.NewGuid();
            UserId = userId;
            ExpirationTime = DateTime.UtcNow.AddMinutes(15);
        }

        //Scalar Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid SessionId { get; set; }
        public DateTime ExpirationTime { get; set; }

        //Navigation Properties
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
