using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorConf2022.Shared
{

    public enum TipoAnimale
    {
        [Display(Name = "Iguana")]
        Iguana = 0,
        [Display(Name = "Mio Marito")]
        MioMarito = 1,
        [Display(Name = "Cane")]
        Cane = 2,
        [Display(Name = "Gatto")]
        Gatto = 3,
        [Display(Name = "Nessun Animale")]
        NessunAnimale = 4
    }

    public class UserProfile : ICloneable
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nome Obbligatorio")]
        public string GivenName { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "Cognome Obbligatorio")]
        public string Surname { get; set; } = "";

        public string City { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public string Email { get; set; } = "";

        public TipoAnimale? AnimalePreferito { get; set; }

        public object Clone()
        {
            return (UserProfile)MemberwiseClone();
        }
    }

}
