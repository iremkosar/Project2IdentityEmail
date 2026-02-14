namespace Project2IdentityEmail.Dtos
{
    public class UserEditDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string About { get; set; }
        public string ImageUrl { get; set; }
        public string ConfirmPassword { get; set; }
        public IFormFile Image {  get; set; }

    }
}
