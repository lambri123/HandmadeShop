using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string UserName { get; set; }

   
    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Пълното име трябва да е между 2 и 100 символа.")]
    public string FullName { get; set; }
}
