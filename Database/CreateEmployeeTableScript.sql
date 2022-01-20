CREATE TABLE dbo.Employee(
	Id bigint PRIMARY KEY Identity(1,1) NOT NULL,
    Username varchar(32) not null,
	Password varbinary(max) not null,
	Role varchar(25) not null,
	FirstLoggedIn bit default 0 not null,
	Active bit default 1 not null
);