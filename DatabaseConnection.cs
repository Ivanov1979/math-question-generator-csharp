
using System;
using MySqlConnector;

public static class DatabaseConnection
{
    public static MySqlConnection CreateConnection()
    {
        string? password =
            Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "MYSQL_PASSWORD is not set.");
        }

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            Port = 3306,
            UserID = "math_app",
            Password = password,
            Database = "math_question_generator"
        };

        return new MySqlConnection(builder.ConnectionString);
    }
}
