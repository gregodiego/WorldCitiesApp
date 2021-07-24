using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities.API.Models
{
    public class City
    {
        //blocos region, são diretivas de pré-processamento, define área que podem ser recolhidas e encapsuladas dentro deles
        #region Constructor 
        public City(){

        }
        #endregion

        #region Properties
            
       
        [Key]// define chave primária
        [Required] // define que o objeto é obrigatório, não nulo
        /// <summary>
        /// Unique Id and key for this city
        /// </summary>
        public int Id { get; set; }     

        /// <summary>
        /// name for city
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ASCII name of city
        /// </summary>
        public string Name_ASCII { get; set; }

        /// <summary>
        /// city latitude
        /// </summary>
        [Column(TypeName ="decimal(7,4)")]
        public decimal Lat { get; set; }

        /// <summary>
        /// city longitude
        /// </summary>
        [Column(TypeName ="decimal(7,4)")]
        public decimal Lon { get; set; }
         #endregion
         
         /// <summary>
         /// Country id (foreign key)
         /// </summary>
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; } //variáveis virtual não viram campos na tabela do banco de dados
        
    }
}