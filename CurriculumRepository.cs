
using System;
using System.Collections.Generic;
using MySqlConnector;

// Retrieves curriculum information from the MySQL database.
public static class CurriculumRepository
{
    // Loads curriculum records and converts each database row
    // into a CurriculumItem used by the question engine.
    public static List<CurriculumItem> GetCurriculum()
    {
        List<CurriculumItem> curriculum =
            new List<CurriculumItem>();

        const string sql = @"
            SELECT
                mf.field_name,
                mc.content_name,
                g.grade_name,
                lo.oa_code,
                lo.oa_description,
                ld.detail_description,
                ai.indicator_description
            FROM LearningObjectives AS lo
            INNER JOIN Grades AS g
                ON lo.grade_id = g.grade_id
            INNER JOIN MathematicalContents AS mc
                ON lo.content_id = mc.content_id
            INNER JOIN MathematicalFields AS mf
                ON mc.field_id = mf.field_id
            INNER JOIN LearningDetails AS ld
                ON lo.oa_id = ld.oa_id
            INNER JOIN AssessmentIndicators AS ai
                ON ai.oa_id = lo.oa_id
                AND ai.detail_id = ld.detail_id
            ORDER BY
                g.grade_number,
                lo.oa_code,
                ld.detail_id,
                ai.indicator_id;";

        using var connection =
            DatabaseConnection.CreateConnection();

        connection.Open();

        using var command =
            new MySqlCommand(sql, connection);

        using var reader =
            command.ExecuteReader();

        while (reader.Read())
        {
            CurriculumItem item =
                new CurriculumItem(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    new List<string>
                    {
                        reader.GetString(6)
                    });

            curriculum.Add(item);
        }

        return curriculum;
    }
}