namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Course
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

        [DataType(DataType.Date)]
        public DateTime StarTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndTime { get; set; }

        //Navigation properties
        public int OwnerId { get; set; }

        /*
        [ForeignKey("OwnerId")]
        public virtual gawga Owner { get; set; }
        */
    }
}