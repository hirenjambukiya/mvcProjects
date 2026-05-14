IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ApiTester')
BEGIN
    CREATE DATABASE ApiTester;
END
GO

USE ApiTester;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ApiRequestHistory' and xtype='U')
BEGIN
    CREATE TABLE ApiRequestHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ApiUrl NVARCHAR(MAX) NOT NULL,
        HttpMethod NVARCHAR(10) NOT NULL,
        RequestHeaders NVARCHAR(MAX) NULL,
        RequestJson NVARCHAR(MAX) NULL,
        ResponseJson NVARCHAR(MAX) NULL,
        StatusCode INT NOT NULL,
        ResponseTime BIGINT NOT NULL,
        IsSuccess BIT NOT NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO
