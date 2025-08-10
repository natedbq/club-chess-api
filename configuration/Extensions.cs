namespace chess.api.configuration
{
    public static class Extensions
    {
        public static string SqlOrNull(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "null";
            }

            return $"'{str.Replace("'", "''")}'";
        }
        public static string SqlOrNull(this bool b)
        {

            return b ? "1" : "0";
        }
        public static string SqlOrNull(this Guid? guid)
        {
            if (!guid.HasValue)
            {
                return "null";
            }

            return $"'{guid}'";
        }
        public static string SqlOrNull(this Guid guid)
        {
            if (guid == default(Guid))
            {
                return "null";
            }

            return $"'{guid}'";
        }
        public static string SqlOrNull(this DateTime date)
        {
            if (date == default(DateTime))
            {
                return "null";
            }

            return $"'{date.ToString()}'";
        }
    }
}
