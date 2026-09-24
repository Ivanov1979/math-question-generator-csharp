
# Math Question & Activity Engine

## Overview

This software is a console-based mathematical question generator developed in C# for my CSE 310 Applied Programming course.

The program allows a teacher to select a mathematical field, content, grade, learning objective (OA), learning detail, assessment indicator, mathematical skill, and difficulty level. It then generates a mathematics question and a suggested answer.

The current question generator supports numerical patterns for OA09. It can generate numerical-sequence questions and input-output-table questions. For input-output tables, the program provides different question formats for four mathematical skills: Represent, Model, Solve Problems, and Argue & Communicate.

Curriculum information is stored in a MySQL relational database rather than being limited to hard-coded curriculum records. The database currently contains curriculum records for OA05 (division), OA08 (fractions), and OA09 (numerical patterns). Question generation for OA05 and OA08 is future work.

My purpose for writing this software was to develop my C# and SQL skills while creating a prototype that could later become part of a larger mathematics learning platform.

[Software Demo Video](VIDEO_LINK_HERE)

## Development Environment

I developed this software using:

* Visual Studio Code
* C#
* .NET 10
* MySQL
* MySQL Workbench
* MySqlConnector
* Git and GitHub
* macOS

The application uses C# to connect to MySQL, execute SQL queries, retrieve curriculum information, and generate mathematics questions.

## Database Structure

The MySQL database is named `math_question_generator`. It contains six related curriculum tables:

* `MathematicalFields` — mathematical fields, such as Numbers and Patterns and Algebra.
* `MathematicalContents` — mathematical contents associated with fields.
* `Grades` — grade levels.
* `LearningObjectives` — learning objectives associated with grades and mathematical contents.
* `LearningDetails` — learning details associated with learning objectives.
* `AssessmentIndicators` — assessment indicators associated with learning objectives and learning details.

The application uses SQL `JOIN` operations to retrieve related curriculum information from these tables.

## Software Features

### Database-Backed Curriculum Selection

The teacher selects curriculum information through console menus. The application retrieves curriculum records from MySQL and uses the selected information to create a question-generation request.

### Mathematical Question Generation

The implemented OA09 generator creates randomized numerical-pattern questions at Easy, Medium, and Hard difficulty levels.

For the input-output-table learning detail, the generator supports four mathematical skills:

* Represent: Complete a missing value in a table.
* Model: Write an equation relating the input and output.
* Solve Problems: Apply the table's rule to a new input.
* Argue & Communicate: Explain the rule and justify it using values from the table.

The program displays the generated question and a suggested answer.

### SQL CRUD Demonstration

The application includes a separate demonstration of the four basic database operations:

* **Create:** Insert a temporary grade record.
* **Read:** Retrieve that record.
* **Update:** Modify the record.
* **Delete:** Remove the temporary record.

### SQL JOIN Demonstration

The application includes a separate database test that connects to MySQL and retrieves learning objectives, learning details, and assessment indicators using SQL `JOIN` operations.

## Running the Software

The application requires .NET, a running MySQL server, and the `math_question_generator` database with the curriculum tables and records described above.

The MySQL password is read from the `MYSQL_PASSWORD` environment variable. The password should not be stored in the source code or committed to GitHub.

From the project directory, run the question generator:

```bash
dotnet run
```

Run the CRUD demonstration:

```bash
dotnet run -- --test-crud
```

Run the SQL JOIN demonstration:

```bash
dotnet run -- --test-db
```

## Useful Websites

* [Microsoft C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)
* [Microsoft .NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
* [MySQL Documentation](https://dev.mysql.com/doc/)
* [MySqlConnector Documentation](https://mysqlconnector.net/)

## Future Work

* Implement question generators for OA05 (division) and OA08 (fractions).
* Expand the curriculum records and supported grade levels.
* Develop additional question formats for each learning detail and assessment indicator.
* Add automated tests for database retrieval and question generation.
* Improve error handling and setup instructions for the database connection.
* Integrate the question engine into the larger mathematics learning platform.