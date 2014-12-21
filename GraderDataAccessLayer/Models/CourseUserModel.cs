namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public enum PermissionOptions
    {
        CanGrade  = 1,
        CanCreateEntities = 2
    }

    public class CourseUserModel
    {
        public CourseUserModel()
        {
            
        }

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
        public int ExtensionLimit { get; set; }

        [Required]
        public int ExcuseLimit { get; set; }


        //Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        [ForeignKey("CourseId")]
        public virtual CourseModel Course { get; set; } 
    }
}
