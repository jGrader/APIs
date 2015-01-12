namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class GradeModel
    {
        //Scalar Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Navigation property
        public int UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        //Navigation property
        public int GraderId { get; set; }
        [Required]
        [ForeignKey("GraderId")]
        public virtual UserModel Grader { get; set; }

        //Navigation property
        public int EntityId { get; set; }
        [Required]
        [ForeignKey("EntityId")]
        public virtual EntityModel Entity { get; set; }

        [Required]
        public int Grade { get; set; }

        [Required]
        public int BonusGrade { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Comment { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }
    }
}
