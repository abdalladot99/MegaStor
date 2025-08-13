using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }




        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }


        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public ApplicationUser? Customer { get; set; }


    }
}
