using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraderDataAccessLayer.Models
{
    public class SessionIdModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        private int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public DateTime ExpirationTime { get; set; }
    }
}
