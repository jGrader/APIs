namespace GraderDataAccessLayer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class SshKeyModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Key { get; set; }

        //Navigation property
        public int UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
