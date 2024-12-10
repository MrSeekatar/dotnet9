IF OBJECT_ID('BoxServer.Boxs', 'U') IS NULL
    CREATE TABLE BoxServer.Boxs
    (
        BoxId UNIQUEIDENTIFIER,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(250),
        Active INT NOT NULL DEFAULT 0,
        CreatedOn DATETIME DEFAULT GETDATE(),
        CONSTRAINT PK_BOXS PRIMARY KEY (BoxId),
        CONSTRAINT AK_BOXS_NAME UNIQUE (Name),
    );
ELSE
    print 'BoxServer.Boxs table exists'
GO
