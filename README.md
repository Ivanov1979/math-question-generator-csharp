# Math Question & Activity Engine

## Overview

This software is a console-based Math Question & Activity Engine developed in C# for my CSE 310 Applied Programming course.

The purpose of the program is to organize mathematics activities according to curriculum information and allow a user to select activities based on several criteria.

The current program follows this structure:

Course → Learning Objective (OA) → Assessment Indicator → Competency → Difficulty → Activity

The program reads mathematics activities from a text file, displays the available options dynamically, checks student answers, keeps track of the score during a session, and saves student results to an output file.

My purpose for writing this software was to improve my understanding of C# while developing a prototype that could later become part of a larger mathematics learning platform.

[Software Demo Video](VIDEO_LINK_HERE)

## Development Environment

I developed this software using:

* Visual Studio Code
* C#
* .NET
* Git
* GitHub
* macOS

The program uses C# and .NET libraries including:

* `System`
* `System.Collections.Generic`
* `System.IO`

## Useful Websites

* [Microsoft C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)
* [Microsoft .NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
* [BYU-Idaho CSE 310](https://byui-cse.github.io/cse310-ww-course/)

## Future Work

* Add additional grade levels and Learning Objectives.
* Add more assessment indicators and mathematics activities.
* Improve answer validation for written responses.
* Randomly select activities when several activities match the same criteria.
* Store student names and session information.
* Improve result reporting and statistics.
* Connect the activity engine to the larger mathematics learning platform.