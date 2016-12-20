using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class CityDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name Required")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
        public int NumberOfPointsOfInterest {
            get {
                return PointsOfInterest.Count;
            }
        }

        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = 
            new List<PointOfInterestDto>();
    }
}
