delete from dbo.Employee;

declare @usCountryId bigint;
set @usCountryId = (select Id from dbo.Country where [Name] = 'United States of America');

INSERT [dbo].[Employee] 
(
	[CountryId], 
	[FirstName], 
	[LastName], 
	[SSN], 
	[Username], 
	[Password], 
	[Role], 
	[IsLocked], 
	[FailedLoginAttempts], 
	[TempPasswordChanged], 
	[Status], 
	[Active], 
	[AddressLine1], 
	[AddressLine2], 
	[City], 
	[State], 
	[ZipCode]
) 
VALUES 
(
	@usCountryId, 
	'Jason', 
	'Ruan', 
	'111-11-1111', 
	'jruan1580', 
	0x75AF3662EA5F09BBFB3C66CC2322448E49BE5FC3E331111CA87ED127B6B09B36BF1A66EEDE35D6C789BCD2B255CFC3C0A907A06172A7C623395F0674E2BFB368, 
	'Administrator', 
	0, 
	0, 
	1, 
	0, 
	1, 
	'123 Main St.', 
	NULL, 
	'San Diego', 
	'CA', 
	'92123'
);
