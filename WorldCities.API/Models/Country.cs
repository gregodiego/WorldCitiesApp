using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorldCities.API.Models
{
    public class Country
    {
        #region Constructor
        public Country()
        {
            
        }
        #endregion

        #region Name
        [Key]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISO2 { get; set; }
        public string ISO3 { get; set; }
        #endregion
        public virtual List<City> Cities { get; set; }


    }
}