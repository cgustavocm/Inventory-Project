-- 1) Crear tabla Category
CREATE TABLE [dbo].[Category](
    [ID]   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(50)      NOT NULL
);

-- 2) Insertar las dos categorías base
INSERT INTO [dbo].[Category] ([Name]) 
VALUES ('Electronic'), ('Non Electronic');
