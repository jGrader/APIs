namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        //Navigation property
        public int GradeComponentId { get; set; }
        [Required]
        [ForeignKey("GradeComponentId")]
        public virtual GradeComponentModel GradeComponent { get; set; }

        //Navigation property
        public int CourseId { get; set; }
        [Required]
        [ForeignKey("CourseId")]
        public virtual CourseModel Course { get; set; }
    }
}
