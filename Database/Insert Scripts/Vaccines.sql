delete from dbo.Vaccines;

declare @dogTypeId smallint;
declare @catTypeId smallint;

set @dogTypeId = (select Id from dbo.PetType where PetTypeName = 'Dog');
set @catTypeId = (select Id from dbo.PetType where PetTypeName = 'Cat');

Insert into dbo.Vaccines
(
	PetTypeId,
	VaxName
)
Values
 (@dogTypeId, 'Bordetella'),
 (@dogTypeId, 'Rabies'),
 (@dogTypeId, 'Leptospirosis'), 
 (@dogTypeId, 'DHPP/DA2PP/DHLPP'),
 (@dogTypeId, 'Kennel Cough'),
 (@catTypeId, 'Rabies'),
 (@catTypeId, 'FVRCP'),
 (@catTypeId, 'Feline Leukemia');
