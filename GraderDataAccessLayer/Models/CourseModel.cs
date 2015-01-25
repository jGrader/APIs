namespace GraderDataAccessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CourseModel
    {
        //Scalar Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string CourseNumber { get; set; }

        [DataType(DataType.Text)]
        public string ShortName { get; set; }

        [Required]
        public int Semester { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public int ExtensionLimit { get; set; }

        [Required]
        public int ExcuseLimit { get; set; }

        //Navigation properties
        [Required]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual UserModel Owner { get; set; }

        public virtual ICollection<GradeComponentModel> GradeComponents { get; set; }

        public virtual ICollection<CourseUserModel> CourseUsers { get; set; }

        public virtual ICollection<TaskModel> Tasks { get; set; } 
    }
}