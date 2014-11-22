namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    class EntityModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int BonusPoints { get; set; }

        public int TaskId { get; set; }
        [Required]
        [ForeignKey("TaskId")]
        public virtual TaskModel Task { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public string OpenTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public string CloseTime { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
