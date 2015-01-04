namespace GraderDataAccessLayer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        //Navigation properties
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int GradeComponentId { get; set; }

        [ForeignKey("CourseId")]
        public virtual CourseModel Course { get; set; }

        [ForeignKey("GradeComponentId")]
        public virtual GradeComponentModel GradeComponent { get; set; }

        public virtual ICollection<EntityModel> Entities { get; set; }
    }
}
