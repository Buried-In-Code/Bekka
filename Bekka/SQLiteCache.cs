using Microsoft.Data.Sqlite;

namespace Bekka
{
    public class SQLiteCache
    {
        private readonly string _databaseUrl;
        private readonly int? _expiry;

        public SQLiteCache(string path, int? expiry = null)
        {
            _databaseUrl = $"Data Source={path}";
            _expiry = expiry;
            CreateTable();
            Delete();
        }

        private void CreateTable()
        {
            using var connection = new SqliteConnection(_databaseUrl);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS queries (url TEXT, response TEXT, query_date DATE);";
            command.ExecuteNonQuery();
        }

        public string? Select(string url)
        {
            using var connection = new SqliteConnection(_databaseUrl);
            connection.Open();
            var command = connection.CreateCommand();
            if (_expiry == null)
                command.CommandText = @"SELECT * FROM queries WHERE url = @url;";
            else
            {
                command.CommandText = @"SELECT * FROM queries WHERE url = @url AND query_date > @expiryDate;";
                var expiryDate = DateTime.Now.AddDays(-(int)_expiry);
                command.Parameters.AddWithValue("@expiryDate", expiryDate);
            }
            command.Parameters.AddWithValue("@url", url);
            using var reader = command.ExecuteReader();
            return reader.Read() ? reader["response"].ToString() : null;
        }

        public void Insert(string url, string response)
        {
            if (Select(url) != null)
                return;

            using var connection = new SqliteConnection(_databaseUrl);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO queries (url, response, query_date) VALUES (@url, @response, @queryDate);";
            command.Parameters.AddWithValue("@url", url);
            command.Parameters.AddWithValue("@response", response);
            command.Parameters.AddWithValue("@queryDate", DateTime.Now);
            command.ExecuteNonQuery();
        }

        public void Delete()
        {
            if (_expiry == null)
                return;

            using var connection = new SqliteConnection(_databaseUrl);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM queries WHERE query_date < @expiryDate;";
            var expiryDate = DateTime.Now.AddDays(-(int)_expiry);
            command.Parameters.AddWithValue("@expiryDate", expiryDate);
            command.ExecuteNonQuery();
        }
    }
}