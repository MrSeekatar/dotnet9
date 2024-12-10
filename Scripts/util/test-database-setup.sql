-- this will create the databases and schemas required for running locally
-- that mirrors the development environment
-- after doing this, you can run Perses via ./run.ps1 updatedb,bootstrapdb
IF (NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TransparentlyMasterDev'))
    create database TransparentlyMasterDev;
GO

use TransparentlyMasterDev
GO

create schema Perses
go


IF OBJECT_ID('dbo.Clients', 'U') IS NULL
BEGIN
    create table Clients
    (
        ClientID         bigint        not null primary key,
        Name             nvarchar(250) not null,
        Active           bit           not null,
        ExternalClientID bigint        not null,
        DatabaseName     varchar(250)  not null,
        APIKey           varchar(36)   not null,
        CUID             varchar(36)   not null,
        Enable2Fa        bit           not null
    )

    insert into dbo.Clients
    (ClientID, Name,      Active, ExternalClientID, DatabaseName,          APIKey, CUID,                                  Enable2Fa)
    values
    (2,       'Scranton', 1,      0,                'TransparentlyClient2',newId(),'b63bab43-ef1f-4f41-8689-633c5e88d21d',1)
    -- ,(3,       'Allentown',1,      0,                'TransparentlyClient3',newId(),'2aa184bf-4d62-4784-a88f-9a040ff7a2a5',1)
    ;

END
go

IF (NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TransparentlyClient2'))
    create database TransparentlyClient2
go

use TransparentlyClient2
go

create schema Perses
go
