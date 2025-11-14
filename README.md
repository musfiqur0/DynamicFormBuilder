Dynamic Form Builder

A simple ASP.NET Core MVC application that lets users create custom
forms with dynamic dropdown fields, save them to a SQL Server database,
and preview the completed forms.

Features

-   Create forms with dynamic dropdowns
-   Add/remove fields using jQuery
-   Server-side pagination using DataTables
-   Preview saved forms
-   ADO.NET with stored procedures
-   Validation using DataAnnotations

Requirements

-   .NET 8 SDK
-   SQL Server (local or remote)

Setup

1.  Update appsettings.json with your SQL connection string.

2.  Run the SQL script (script.sql) to create tables and stored
    procedures.

3.  Build and run the project:

        dotnet run

Project Structure

-   Controllers
-   Models
-   Views
-   Data (Repository + ADO.NET)

How It Works

Users create a form → select dropdown options → save → view in grid →
preview single form.
