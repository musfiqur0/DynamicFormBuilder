-- Create the Test database
CREATE DATABASE Test;
GO
USE Test;
GO

-- Table to hold the forms
CREATE TABLE Forms (
    FormId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Table to hold dropdown fields for each form
CREATE TABLE FormFields (
    FieldId INT IDENTITY(1,1) PRIMARY KEY,
    FormId INT NOT NULL FOREIGN KEY REFERENCES Forms(FormId),
    Label NVARCHAR(255) NOT NULL,
    IsRequired BIT NOT NULL,
    SelectedOption NVARCHAR(50) NOT NULL
);
GO

-- User?defined table type used for bulk inserting form fields
CREATE TYPE dbo.FormFieldTableType AS TABLE
(
    Label NVARCHAR(255),
    IsRequired BIT,
    SelectedOption NVARCHAR(50)
);
GO

-- Stored procedure to save a form and its fields
CREATE PROCEDURE sp_SaveForm
    @Title NVARCHAR(255),
    @Fields dbo.FormFieldTableType READONLY,
    @NewFormId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Forms (Title, CreatedDate) VALUES (@Title, GETDATE());
    SET @NewFormId = SCOPE_IDENTITY();
    INSERT INTO FormFields (FormId, Label, IsRequired, SelectedOption)
    SELECT @NewFormId, Label, IsRequired, SelectedOption FROM @Fields;
END;
GO

-- Stored procedure to return a list of all forms
CREATE PROCEDURE sp_GetFormsPaged
    @PageNumber INT,
    @PageSize   INT
AS
BEGIN
    SET NOCOUNT ON;

    -- total count (for DataTables recordsTotal / recordsFiltered)
    SELECT COUNT(*) AS TotalCount
    FROM Forms;

    -- current page
    SELECT FormId,
           Title,
           CreatedDate
    FROM Forms
    ORDER BY FormId DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- Stored procedure to return a specific form and its fields
CREATE PROCEDURE sp_GetFormById
    @FormId INT
AS
BEGIN
    SET NOCOUNT ON;
    -- First result set: form details
    SELECT FormId, Title, CreatedDate FROM Forms WHERE FormId = @FormId;
    -- Second result set: associated fields
    SELECT FieldId, FormId, Label, IsRequired, SelectedOption
    FROM FormFields
    WHERE FormId = @FormId;
END;
GO
