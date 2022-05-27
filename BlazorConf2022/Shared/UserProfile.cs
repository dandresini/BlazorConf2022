using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorConf2022.Shared
{
    public class UserProfile : ICloneable
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nome Obbligatorio")]
        public string GivenName { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "Cognome Obbligatorio")]
        public string Surname { get; set; } = "";

        public string City { get; set; } = "";

        public string PostalCode { get; set; } = "";

        [Required(ErrorMessage = "Email obbligatoria")]
        [Email(ErrorMessage = "Indirizzo email non valido")]
        public string Email { get; set; } = "";

        public string AnimalePreferito { get; set; } = "";

        public object Clone()
        {
            return (UserProfile)MemberwiseClone();
        }
    }
}
