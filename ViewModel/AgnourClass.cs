using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.ViewModel
{
    public class AgnourClass
    {
        [NotMapped]
        public IFormFile ClientFile { get; set; }

    }
}
