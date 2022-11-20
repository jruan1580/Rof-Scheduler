IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'PetServices'))
BEGIN
	DROP TABLE dbo.PetServices;
END

CREATE TABLE dbo.PetServices(
	Id SMALLINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ServiceName VARCHAR(255) NOT NULL,
	Price DECIMAL(10, 2) NOT NULL,
	EmployeeRate DECIMAL(5,2) NOT NULL, -- percentage
	Duration SMALLINT NOT NULL, -- specified in hours, minutes, or seconds
	TimeUnit VARCHAR(10) NOT NULL, -- hours, minutes, and seconds
	[Description] VARCHAR(2000) NULL,
	CONSTRAINT UC_ServiceName UNIQUE(ServiceName)
);