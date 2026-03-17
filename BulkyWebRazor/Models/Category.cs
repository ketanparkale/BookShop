using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor.Models
{
    public class Category
    {
        [Key]  //data anotation alike constraints in sql. This one denotes PrimaryKey
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Required]
        [Range(1, 100, ErrorMessage = "Enter value between 1 to 100.")]
        public int DisplayOrder { get; set; }

    }
}
