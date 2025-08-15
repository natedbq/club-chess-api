namespace chess.api.Exceptions
{

    public class UserException: Exception
    {
        public UserException(string message)
        : base(message)
        {

        }
    }
    public class UsernameTakenException: UserException
    {
        public UsernameTakenException(string username)
        : base($"Username '{username}' is already taken.")
        {
            
        }
    }

    public class InvalidUserIdException : UserException
    {
        public InvalidUserIdException(Guid id)
        : base($"No user with id '{id}'")
        {

        }
    }

    public class FailedToAuthenticateUserException : UserException
    {
        public FailedToAuthenticateUserException()
        : base($"Username or password is incorrect.")
        {

        }
    }
}
