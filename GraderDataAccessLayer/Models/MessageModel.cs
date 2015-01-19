namespace GraderDataAccessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MessageModel
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int GradeId { get; set; }

        // Note the lack of [Required]
        public int MessageId { get; set; }
        
        [Required]
        public string Contents { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        // Navigation properties
        [ForeignKey("GradeId")]
        public virtual GradeModel Grade { get; set; }

        [ForeignKey("MessageId")]
        public virtual MessageModel ParentMessage { get; set; }

        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
