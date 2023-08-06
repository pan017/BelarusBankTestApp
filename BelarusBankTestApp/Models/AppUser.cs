using System.ComponentModel.DataAnnotations;

namespace BelarusBankTestApp.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Роль")]
        public Role Role { get; set; }

        public AppUser() { }
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
        public Role() { }

    }
}
