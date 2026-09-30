# Module 2 — MySQL Database Design

## Project
Mathematical Question Generation Engine

## Objective
Store the mathematics curriculum in a relational MySQL
database and retrieve it from the existing C# application.

## Proposed Tables
1. MathematicalFields — mathematical fields.
2. MathematicalContents — contents associated with fields.
3. Grades — grade levels from 1st to 6th Grade.
4. LearningObjectives — OA codes and descriptions,
   associated with grades and mathematical contents.
5. LearningDetails — details associated with each OA.
6. AssessmentIndicators — indicators associated with
   learning details.

## Proposed Relationships
MathematicalFields 1:N MathematicalContents
MathematicalContents 1:N LearningObjectives
Grades 1:N LearningObjectives
LearningObjectives 1:N LearningDetails
LearningDetails 1:N AssessmentIndicators

## Design Question
Can one learning objective belong to more than one
mathematical content? If so, a junction table may be needed.

## Next Task
Define primary keys, foreign keys, column types,
and relationship constraints.
