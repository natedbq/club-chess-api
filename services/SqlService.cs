namespace chess.api.services
{
    public class SqlService
    {
        private readonly string _connectionString;
        public SqlService(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
