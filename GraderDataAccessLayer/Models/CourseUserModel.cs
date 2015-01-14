namespace GraderDataAccessLayer.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CourseUserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Scalar properties
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int Permissions { get; set; }

        [Required]
        public int ExtensionNumber { get; set; }

        [Required]
        public int ExcuseNumber { get; set; }


        //Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        [ForeignKey("CourseId")]
        public virtual CourseModel Course { get; set; } 
    }
}
