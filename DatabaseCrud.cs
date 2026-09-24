
using System;
using MySqlConnector;

// Demonstrates Create, Read, Update, and Delete operations
// using a temporary grade record in MySQL.
public static class DatabaseCrud
{
    // Runs all four CRUD operations in one transaction.
    // Rolls back the changes if an operation fails.
    public static void RunDemo()
    {
        using var connection =
            DatabaseConnection.CreateConnection();

        connection.Open();

        using var transaction =
            connection.BeginTransaction();

        try
        {
            const int testGradeNumber = 99;

            // CREATE: Insert a temporary grade.
            const string insertSql = @"
                INSERT INTO Grades (grade_number, grade_name)
                VALUES (@gradeNumber, @gradeName);";

            using var insertCommand =
                new MySqlCommand(insertSql, connection, transaction);

            insertCommand.Parameters.AddWithValue(
                "@gradeNumber", testGradeNumber);

            insertCommand.Parameters.AddWithValue(
                "@gradeName", "CRUD Test");

            insertCommand.ExecuteNonQuery();

            long testGradeId = insertCommand.LastInsertedId;

            Console.WriteLine(
                $"CREATE: Inserted grade ID {testGradeId}.");

            // READ: Retrieve the newly inserted grade.
            const string selectSql = @"
                SELECT grade_number, grade_name
                FROM Grades
                WHERE grade_id = @gradeId;";

            using (var selectCommand =
                new MySqlCommand(selectSql, connection, transaction))
            {
                selectCommand.Parameters.AddWithValue(
                    "@gradeId", testGradeId);

                using var reader =
                    selectCommand.ExecuteReader();

                if (!reader.Read())
                {
                    throw new InvalidOperationException(
                        "The test grade was not found.");
                }

                Console.WriteLine(
                    $"READ: Grade {reader.GetInt32(0)} — " +
                    reader.GetString(1));
            }

            // UPDATE: Change the temporary grade's name.
            const string updateSql = @"
                UPDATE Grades
                SET grade_name = @gradeName
                WHERE grade_id = @gradeId;";

            using var updateCommand =
                new MySqlCommand(updateSql, connection, transaction);

            updateCommand.Parameters.AddWithValue(
                "@gradeName", "CRUD Updated");

            updateCommand.Parameters.AddWithValue(
                "@gradeId", testGradeId);

            int updatedRows =
                updateCommand.ExecuteNonQuery();

            Console.WriteLine(
                $"UPDATE: {updatedRows} record updated.");

            // DELETE: Remove only the record created by this demo.
            const string deleteSql = @"
                DELETE FROM Grades
                WHERE grade_id = @gradeId;";

            using var deleteCommand =
                new MySqlCommand(deleteSql, connection, transaction);

            deleteCommand.Parameters.AddWithValue(
                "@gradeId", testGradeId);

            int deletedRows =
                deleteCommand.ExecuteNonQuery();

            Console.WriteLine(
                $"DELETE: {deletedRows} record deleted.");

            transaction.Commit();

            Console.WriteLine(
                "CRUD demonstration completed successfully.");
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
