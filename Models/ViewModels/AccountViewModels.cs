using System.ComponentModel.DataAnnotations;

namespace RomBooking.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-post er påkrevd")]
    [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Passord er påkrevd")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    [Display(Name = "Husk meg")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Fullt navn er påkrevd")]
    [StringLength(100, ErrorMessage = "Navnet kan ikke være lengre enn 100 tegn")]
    [Display(Name = "Fullt navn")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "E-post er påkrevd")]
    [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
    [Display(Name = "E-post")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Passord er påkrevd")]
    [StringLength(100, ErrorMessage = "Passordet må være minst {2} og maksimalt {1} tegn langt.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Passord")]
    public string Password { get; set; } = string.Empty;
    
    [DataType(DataType.Password)]
    [Display(Name = "Bekreft passord")]
    [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
