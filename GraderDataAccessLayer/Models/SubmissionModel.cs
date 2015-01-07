namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class SubmissionModel
    {
        //Scalar properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string FileName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string FilePath { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }

        //Navigation property
        [Required]
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        //Navigation property
        [Required]
        public int EntityId { get; set; }
        
        [ForeignKey("EntityId")]
        public virtual EntityModel Entity { get; set; }
    }
}
