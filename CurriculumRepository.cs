using System;
using System.Collections.Generic;
using MySqlConnector;

public static class CurriculumRepository
{
    // =====================================================
    // 1. EXISTING CURRICULUM RETRIEVAL
    // =====================================================

    public static List<CurriculumItem> GetCurriculum()
    {
        List<CurriculumItem> curriculum = new();

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

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            curriculum.Add(new CurriculumItem(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                new List<string> { reader.GetString(6) }
            ));
        }

        return curriculum;
    }

    // =====================================================
    // 2. EXISTING ASSESSMENT INDICATOR RETRIEVAL
    // =====================================================

    public static List<AssessmentIndicatorRecord>
        GetAssessmentIndicators()
    {
        List<AssessmentIndicatorRecord> indicators = new();

        const string sql = @"
            SELECT
                lo.oa_id,
                lo.oa_code,
                g.grade_name,
                ld.detail_id,
                ld.detail_description,
                ai.indicator_id,
                ai.indicator_description
            FROM LearningObjectives AS lo
            INNER JOIN Grades AS g
                ON lo.grade_id = g.grade_id
            INNER JOIN LearningDetails AS ld
                ON ld.oa_id = lo.oa_id
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

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            indicators.Add(new AssessmentIndicatorRecord(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3),
                reader.GetString(4),
                reader.GetInt32(5),
                reader.GetString(6)
            ));
        }

        return indicators;
    }

    // =====================================================
    // 3. LOAD FRACTION GENERATION RULES FROM MYSQL
    // =====================================================

    public static List<FractionGenerationRule>
        GetFractionGenerationRules()
    {
        List<FractionGenerationRule> rules = new();

        const string sql = @"
            SELECT
                g.grade_name,
                mf.field_name,
                mc.content_name,
                lo.oa_id,
                lo.oa_code,
                lo.oa_description,
                ld.detail_id,
                ld.detail_description,
                fgr.activity_type,
                fgr.mathematical_skill,
                fgr.response_type,
                fgr.allowed_denominators
            FROM LearningObjectives AS lo
            INNER JOIN Grades AS g
                ON g.grade_id = lo.grade_id
            INNER JOIN MathematicalContents AS mc
                ON mc.content_id = lo.content_id
            INNER JOIN MathematicalFields AS mf
                ON mf.field_id = mc.field_id
            INNER JOIN LearningDetails AS ld
                ON ld.oa_id = lo.oa_id
            INNER JOIN FractionDetailGenerationRules AS fgr
                ON fgr.detail_id = ld.detail_id
            WHERE g.grade_number BETWEEN 3 AND 6
                AND mc.content_name = 'Fractions'
            ORDER BY
                g.grade_number,
                mf.field_name,
                lo.oa_code,
                ld.detail_id;";

        using var connection =
            DatabaseConnection.CreateConnection();

        connection.Open();

        using var command =
            new MySqlCommand(sql, connection);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            rules.Add(new FractionGenerationRule(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetInt32(6),
                reader.GetString(7),
                reader.GetString(8),
                reader.GetString(9),
                reader.GetString(10),
                reader.IsDBNull(11)
                    ? null
                    : reader.GetString(11)
            ));
        }

        return rules;
    }

    // =====================================================
    // 4. NEW: FIND A RULE BY LEARNING DETAIL ID
    // =====================================================

    // The teacher selects an OA Desagregado.
    // JavaScript sends its LearningDetailId to the backend.
    // This method retrieves the corresponding generation rule.

    public static FractionGenerationRule?
        GetFractionGenerationRuleByDetailId(int learningDetailId)
    {
        if (learningDetailId <= 0)
        {
            return null;
        }

        List<FractionGenerationRule> rules =
            GetFractionGenerationRules();

        return rules.Find(rule =>
            rule.LearningDetailId == learningDetailId);
    }
}

// =========================================================
// DATABASE RECORDS
// =========================================================

// Existing assessment indicator record.
public record AssessmentIndicatorRecord(
    int OAId,
    string OACode,
    string Grade,
    int LearningDetailId,
    string LearningDetail,
    int IndicatorId,
    string IndicatorDescription
);

// Represents one fraction generation rule.
public record FractionGenerationRule(
    string Grade,
    string MathematicalField,
    string Content,
    int OAId,
    string OACode,
    string OADescription,
    int LearningDetailId,
    string LearningDetail,
    string ActivityType,
    string MathematicalSkill,
    string ResponseType,
    string? AllowedDenominators
);