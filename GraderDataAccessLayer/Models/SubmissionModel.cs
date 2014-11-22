namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    class SubmissionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Navigation property
        public int UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual UserModel Id { get; set; }

        //Navigation property
        public int EntityId { get; set; }
        [Required]
        [ForeignKey("EntityId")]
        public virtual EntityModel Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string FileName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string FilePath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string TimeStamp { get; set; }
    }
}
