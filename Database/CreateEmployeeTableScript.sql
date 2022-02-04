CREATE TABLE dbo.Employee(
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	FirstName VARCHAR(25) NOT NULL,
	LastName VARCHAR(25) NOT NULL,
	SSN VARCHAR(25) NOT NULL,
    Username VARCHAR(32) NOT NULL,
	Password VARBINARY(max) NOT NULL,
	Role VARCHAR(25) NOT NULL,
	IsLocked BIT DEFAULT 0 NOT NULL,
	FailedLoginAttempts SMALLINT DEFAULT 0 NOT NULL,
	FirstLoggedIn BIT DEFAULT 0 NOT NULL,
	Active BIT DEFAULT 1 NOT NULL
);