namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ExcuseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Navigation properties
        public int EntityId { get; set; }
        [Required]
        [ForeignKey("EntityId")]
        public virtual EntityModel Entity { get; set; }

        //Navigation properties
        public int UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
