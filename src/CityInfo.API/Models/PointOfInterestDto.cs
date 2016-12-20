using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointOfInterestDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name Required")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        //public virtual CityDto City { get; set; }

        //public int CityId { get; set; }
    }
}
