USE [RofDatamart];
GO

IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'EmployeePayrollDetail'))
BEGIN
	DROP TABLE dbo.EmployeePayrollDetail;
END

CREATE TABLE dbo.EmployeePayrollDetail
(
	EmployeeId BIGINT NOT NULL,
	FirstName VARCHAR(25) NOT NULL,
	LastName VARCHAR(25) NOT NULL,
	PetServiceId BIGINT NOT NULL,
	PetServiceName VARCHAR(255) NOT NULL,
	EmployeePayForService DECIMAL(5,2) NOT NULL,
	IsHolidayPay BIT NOT NULL,
	ServiceDuration INT NOT NULL, 
	ServiceDurationTimeUnit VARCHAR(10) NOT NULL,
	JobEventId BIGINT NOT NULL,
	ServiceStartDateTime DATETIME NOT NULL,
	ServiceEndDateTime DATETIME NOT NULL
);