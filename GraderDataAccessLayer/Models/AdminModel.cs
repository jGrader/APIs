namespace GraderDataAccessLayer.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AdminModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Scalar properties
        [Required]
        public int UserId { get; set; }

        [Required]
        public bool IsSuperUser { get; set; }

        //Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
