namespace GraderDataAccessLayer.Models
{
    using System;
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

        public virtual ICollection<FileModel> Files { get; set; }

        public virtual ICollection<TeamModel> Teams { get; set; }

        public virtual ICollection<ExcuseModel> Excuses { get; set; }

        public virtual ICollection<ExtensionModel> Extensions { get; set; }

        public virtual ICollection<GradeModel> Grades { get; set; } 
    }
}
