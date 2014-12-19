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

        //Navigation property
        public int CourseId { get; set; }

        [Required]
        [ForeignKey("CourseId")]
        public virtual CourseModel Course { get; set; }

        public int UserId { get; set; }
        //Navigation property
        [Required]
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        [Required]
        public int Permissions { get; set; }

        [Required]
        public int ExtensionLimit { get; set; }

        [Required]
        public int ExcuseLimit { get; set; }
    }
}
