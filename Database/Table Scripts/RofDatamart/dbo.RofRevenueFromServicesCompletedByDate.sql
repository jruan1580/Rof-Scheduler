USE [RofDatamart];
GO

IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'RofRevenueFromServicesCompletedByDate'))
BEGIN
	DROP TABLE dbo.RofRevenueFromServicesCompletedByDate;
END

CREATE TABLE dbo.RofRevenueFromServicesCompletedByDate
(	
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	EmployeeId BIGINT NOT NULL,
	EmployeeFirstName VARCHAR(25) NOT NULL,
	EmployeeLastName VARCHAR(25) NOT NULL,
	EmployeePay DECIMAL(5,2) NOT NULL,
	PetServiceId SMALLINT NOT NULL,
	PetServiceName VARCHAR(255) NOT NULL,
	PetServiceRate DECIMAL(5, 2) NOT NULL,
	IsHolidayRate BIT NOT NULL,
	NetRevenuePostEmployeeCut DECIMAL(5,2) NOT NULL,
	RevenueDate DATE NOT NULL
);