namespace GraderDataAccessLayer.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ExcuseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        //Navigation properties
        [Required]
        public int UserId { get; set; }

        [Required]
        public int EntityId { get; set; }
        

        [ForeignKey("EntityId")]
        public virtual EntityModel Entity { get; set; }
        
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
