namespace chess.api.Dto
{

    public class UsernameAndPasswordDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class NewUserDto
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }
}
