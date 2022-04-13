CREATE TABLE dbo.Client(
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	CountryId BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.Country(Id),
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	EmailAddress VARCHAR(50) NOT NULL,
    PrimaryPhoneNum VARCHAR(25) NOT NULL,
	SecondaryPhoneNum VARCHAR(25),
	AddressLine1 VARCHAR(255) NOT NULL,
	AddressLine2 VARCHAR(50),
	City VARCHAR(50) NOT NULL,
	State VARCHAR(2) NOT NULL,
	ZipCode VARCHAR(10) NOT NULL
);