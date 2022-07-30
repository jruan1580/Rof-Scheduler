IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Breed'))
BEGIN
	DROP TABLE dbo.Breed;
END

CREATE TABLE dbo.Breed(
	Id SMALLINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	BreedName VARCHAR(50) NOT NULL
);