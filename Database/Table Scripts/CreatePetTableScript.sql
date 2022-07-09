IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Pet'))
BEGIN
	DROP TABLE dbo.Pet;
END

CREATE TABLE dbo.Pet(
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	OwnerId BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.Client(Id),
	BreedId BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.Breed(Id),
	Name VARCHAR(50) NOT NULL,
	Weight DECIMAL NOT NULL,
	DOB VARCHAR(25) NOT NULL,
	BordetellaVax BIT DEFAULT 0 NOT NULL,
	DHPPVax BIT DEFAULT 0 NOT NULL, 
	RabieVax BIT DEFAULT 0 NOT NULL, 
	OtherInfo VARCHAR(2000),
	Picture VARBINARY(MAX),
	CONSTRAINT UC_PET UNIQUE (OwnerId, BreedId, Name)
);