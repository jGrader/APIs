using System;

namespace GraderDataAccessLayer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EntityModel
    {
        //Scalar properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int BonusPoints { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime OpenTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CloseTime { get; set; }

        //Navigation properties
        [Required]
        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public virtual TaskModel Task { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual CourseModel Course { get; set; }

        public virtual ICollection<SubmissionModel> Submissions { get; set; }
    }
}
