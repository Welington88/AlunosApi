using System;
using System.ComponentModel.DataAnnotations;

namespace AlunosApi.ViewModel
{
    public class RegisterModel
    { 
        [Required]
        [EmailAddress]
        public String? Email{get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirma senha")]
        [Compare("Password", ErrorMessage = "Senhas não conferem")]
        public String? ConfirmPassword { get; set; }
    }
}

