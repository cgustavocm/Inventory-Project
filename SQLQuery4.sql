-- 1) BORRAR tabla si existe
IF OBJECT_ID('dbo.Category', 'U') IS NOT NULL
    DROP TABLE dbo.Category;
GO

-- 2) Crear tabla Category
CREATE TABLE [dbo].[Category]
(
    [ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL
);
GO

-- 3) Insertar categorías
INSERT INTO [dbo].[Category] ([Name])
VALUES ('Electronic'), ('Non Electronic');
GO

