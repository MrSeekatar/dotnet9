IF NOT EXISTS (SELECT 1 FROM sys.schemas where name = 'BoxServer')
BEGIN
    EXECUTE sp_executesql N'CREATE SCHEMA BoxServer'
    print 'Added BoxServer schema'
END
ELSE
    print 'BoxServer schema exists'
GO

