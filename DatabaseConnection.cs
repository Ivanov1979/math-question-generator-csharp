using MySqlConnector;

public static class DatabaseConnection
{
    public static MySqlConnection CreateConnection()
    {
        string? password =
            Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

        if (string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException(
                "MYSQL_PASSWORD is not set.");
        }

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            Port = 3306,
            UserID = "root",
            Password = password,
            Database = "math_question_generator"
        };

        return new MySqlConnection(builder.ConnectionString);
    }
}