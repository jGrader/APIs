namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using GraderDataAccessLayer.Models;

    class GradeComponentModel
    {
        //Scalar Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Navigation properties
        public int CourseId { get; set; }

        [Required]
        [ForeignKey("CourseId")]
        public virtual Course Id { get; set; }
        
    }
}
