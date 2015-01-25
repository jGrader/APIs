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
        public string FilePath { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int FileId { get; set; }

        //Navigation property
        
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
        
        [ForeignKey("FileId")]
        public virtual FileModel File { get; set; }
    }
}
