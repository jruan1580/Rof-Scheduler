USE [RofDatamart];
GO

IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'EmployeePayroll'))
BEGIN
	DROP TABLE dbo.EmployeePayroll;
END

CREATE TABLE dbo.EmployeePayroll
(
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	EmployeeId BIGINT NOT NULL,
	FirstName VARCHAR(25) NOT NULL,
	LastName VARCHAR(25) NOT NULL,
	EmployeeTotalPay DECIMAL(10,2) NOT NULL,
	PayrollDate DATE NOT NULL,
	PayrollMonth SMALLINT NOT NULL,
	PayrollYear SMALLINT NOT NULL
);