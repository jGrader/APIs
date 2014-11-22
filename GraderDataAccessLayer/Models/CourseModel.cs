namespace GraderDataAccessLayer.Models
{
    using System;
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

        public int Semester { get; set; }

        public int Year { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndTime { get; set; }

        //Navigation properties
        public int OwnerId { get; set; }
/*
        [ForeignKey("OwnerId")]
        public virtual UserModel Owner { get; set; }
  */      
    }
}