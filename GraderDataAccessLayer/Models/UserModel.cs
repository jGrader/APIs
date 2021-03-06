﻿namespace GraderDataAccessLayer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Remoting.Contexts;

    public class UserModel
    {
        public UserModel()
        {
            Teams = new HashSet<TeamModel>();    
        }

        // Scalar Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [DataType(DataType.Text)]
        public string PasswordHash { get; set; }

        [Required]
        public string GraduationYear { get; set; }

        //Navigation properties
        public virtual ICollection<TeamModel> Teams { get; set; }

        public virtual ICollection<SubmissionModel> Submissions { get; set; }

        public virtual ICollection<CourseModel> OwnedCourses { get; set; }

        public virtual ICollection<CourseUserModel> Courses { get; set; }

        public virtual ICollection<ExcuseModel> Excuses { get; set; }
        
        public virtual ICollection<ExtensionModel> Extensions { get; set; }

        public virtual ICollection<GradeModel> Grades { get; set; }

        public virtual ICollection<GradeModel> GradesAsGrader { get; set; } 
    }
}
