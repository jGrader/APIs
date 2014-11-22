namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public enum PermissionOptions
    {
        canGrade = 1,
        canCreateEntities = 2
    }

    class CourseUserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Navigation property
        public int CourseId { get; set; }
        [Required]
        [ForeignKey("CourseId")]
        public virtual CourseModel Course { get; set; }


        //Navigation property
        public int UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        [Required]
        public int Permissions { get; set; }
    }
}
