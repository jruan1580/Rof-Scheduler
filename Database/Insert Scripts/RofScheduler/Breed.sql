delete from dbo.Breed;

declare @dogTypeId smallint;
declare @catTypeId smallint;

set @dogTypeId = (select Id from dbo.PetType where PetTypeName = 'Dog');
set @catTypeId = (select Id from dbo.PetType where PetTypeName = 'Cat');

Insert into dbo.Breed
(
	PetTypeId,
	BreedName
)
Values
 (@dogTypeId, 'Golden Retriever'),
 (@dogTypeId, 'Golden Doodle'),
 (@dogTypeId, 'Siberian Husky'),
 (@dogTypeId, 'Labrador Retriever'),
 (@dogTypeId, 'German Shepherd'),
 (@catTypeId, 'Persian Cat'),
 (@catTypeId, 'Abyssinian Cat'),
 (@catTypeId, 'American Bobtail Cat'),
 (@catTypeId, 'American Curl Cat');
