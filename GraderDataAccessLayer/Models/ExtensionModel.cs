namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ExtensionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Navigation property
        public int UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        //Navigation property
        public int EntityId { get; set; }
        [Required]
        [ForeignKey("EntityId")]
        public virtual EntityModel Entity { get; set; }
    }
}
