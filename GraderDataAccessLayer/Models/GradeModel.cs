﻿namespace GraderDataAccessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class GradeModel
    {
        //Scalar Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Grade { get; set; }

        [Required]
        public int BonusGrade { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime TimeStamp { get; set; }

        //Navigation properties
        [Required]
        public int UserId { get; set; }

        [Required]
        public int GraderId { get; set; }

        [Required]
        public int EntityId { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Grades")]
        public virtual UserModel User { get; set; }

        [ForeignKey("GraderId")]
        [InverseProperty("GradesAsGrader")]
        public virtual UserModel Grader { get; set; }

        [ForeignKey("EntityId")]
        public virtual EntityModel Entity { get; set; }

        public virtual ICollection<MessageModel> Messages { get; set; }
    }
}
