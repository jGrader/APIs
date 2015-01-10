namespace GraderDataAccessLayer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TeamModel
    {
        public TeamModel()
        {
            TeamMembers = new HashSet<UserModel>();
        }

        //Scalar properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Navigation property
        [Required]
        public int EntityId { get; set; }
        
        [ForeignKey("EntityId")]
        public virtual EntityModel Entity { get; set; }

        public virtual ICollection<UserModel> TeamMembers { get; set; }
    }
}
