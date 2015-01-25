namespace GraderDataAccessLayer.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class FileModel
    {
        // Scalar properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string Extension { get; set; }

        // Navigation properties
        [Required]
        public int EntityId { get; set; }
        
        [ForeignKey("EntityId")]
        public virtual EntityModel Entity { get; set; }

        public virtual ICollection<SubmissionModel> Submissions { get; set; }
    }
}
