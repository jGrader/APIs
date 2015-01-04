namespace GraderDataAccessLayer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class GradeComponentModel
    {
        //Scalar Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        public int Percentage { get; set; }

        //Navigation properties
        [Required]
        public int CourseId { get; set; }
        
        [ForeignKey("CourseId")]
        public virtual CourseModel Course { get; set; }

        public virtual ICollection<TaskModel> Tasks { get; set; }
    }
}
