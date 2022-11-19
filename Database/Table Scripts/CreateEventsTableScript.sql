IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'JobEvent'))
BEGIN
	DROP TABLE dbo.JobEvent;
END

CREATE TABLE dbo.JobEvent(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	EmployeeId BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.Employee(Id),
	PetId BIGINT NOT NULL FOREIGN KEY REFERENCES dbo.Pet(Id),
	PetServiceId SMALLINT NOT NULL FOREIGN KEY REFERENCES dbo.PetServices(Id),
	EventStartTime DATETIME NOT NULL,
	EventEndTime DATETIME NOT NULL,
	Completed BIT NOT NULL,
	Canceled BIT NOT NULL,
	CONSTRAINT UC_EVENT UNIQUE (EmployeeId, PetId, EventStartTime, EventEndTime)
);