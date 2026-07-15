CREATE DATABASE ELMS
GO
USE ELMS
GO

CREATE TABLE mst_roles(
RoleId BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
RoleType BIGINT NOT NULL,
[Description] VARCHAR(max) NULL,
CreateAt DATETIME DEFAULT GETDATE(),
UpdateAt DATETIME NULL,
IsDeleted BIT DEFAULT 0
);
GO
CREATE TABLE mst_leavestatus(
LeaveStatusId BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
LeaveType BIGINT NOT NULL,
[Description] VARCHAR(max) NULL,
CreateAt DATETIME DEFAULT GETDATE(),
UpdateAt DATETIME NULL,
IsDeleted BIT DEFAULT 0
);
GO
CREATE TABLE mst_leavetype(
LeavetypeId BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
LeaveType VARCHAR(100) NOT NULL,
[Description] VARCHAR(max) NULL,
CreateAt DATETIME DEFAULT GETDATE(),
UpdateAt DATETIME NULL,
IsDeleted BIT DEFAULT 0
);
GO
CREATE TABLE mst_users(
UserId BIGINT IDENTITY(100,1) PRIMARY KEY NOT NULL,
FirtsName VARCHAR(100) NOT NULL,
LastName VARCHAR(100) NOT NULL,
RoleId BIGINT NOT NULL,
EmailAddress VARCHAR(256) NULL,
Password VARCHAR(1000) NOT NULL,
IsActive BIT DEFAULT 0,
CreateAt DATETIME DEFAULT GETDATE(),
UpdateAt DATETIME NULL,
IsDeleted BIT DEFAULT 0
FOREIGN KEY (RoleId) REFERENCES mst_roles(RoleId)
);
GO
CREATE TABLE tbl_leaveapplication
(
LeaveId BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
UserId BIGINT NOT NULL,
LeaveStatusId BIGINT NOT NULL,
LeaveType VARCHAR(500) NOT NULL,
StartDate Datetime NOT NULL,
EndDate Datetime NOT NULL,
Reason VARCHAR(MAX) NOT NULL,
HRComment VARCHAR(MAX) NULL,
AttachedFileName VARCHAR(500) NULL,
CreateAt DATETIME DEFAULT GETDATE(),
UpdateAt DATETIME NULL,
IsDeleted BIT DEFAULT 0,
FOREIGN KEY (UserId) REFERENCES mst_users(UserId),
FOREIGN KEY (LeaveStatusId) REFERENCES mst_leavestatus(LeaveStatusId)
);
GO
--update mst_roles set Description = 'Admin' where RoleId = 1
--update mst_roles set Description = 'HR' where RoleId = 2
--update mst_roles set Description = 'Employee' where RoleId = 3
INSERT INTO mst_roles (RoleType,Description) values(1,'Admin')
INSERT INTO mst_roles (RoleType,Description) values(2,'HR')
INSERT INTO mst_roles (RoleType,Description) values(3,'Employee')
GO
--update mst_leavestatus set Description = 'Pending' where LeaveStatusId = 1
--update mst_leavestatus set Description = 'Approved' where LeaveStatusId = 2
--update mst_leavestatus set Description = 'Rejected' where LeaveStatusId = 3

INSERT INTO mst_leavestatus (LeaveType,Description) values(1,'Pending')
INSERT INTO mst_leavestatus (LeaveType,Description) values(2,'Approved')
INSERT INTO mst_leavestatus (LeaveType,Description) values(3,'Rejected')
GO
CREATE OR ALTER PROCEDURE usp_GetUserByUserName
@UserName VARCHAR(200)
AS
BEGIN
SELECT * FROM mst_users with (nolock) where [FirtsName] +'_' +  [LastName] + '_'+ cast([UserId] as varchar(100)) = @UserName
END
GO
CREATE OR ALTER PROCEDURE usp_Insert_Upadte_Users
@UserId BIGINT = NULL,
@FirtsName VARCHAR(100),
@LastName VARCHAR(100),
@RoleId BIGINT = NULL,
@EmailAddress VARCHAR(256) = NULL,
@Password VARCHAR(1000),
@CreateAt DATETIME = NULL
AS
BEGIN
DECLARE @Count INT

SELECT @Count = COUNT(*) FROM mst_users where UserId = @UserId

IF @Count > 0
 BEGIN
  UPDATE mst_users SET FirtsName = @FirtsName, LastName =@LastName,EmailAddress =@EmailAddress, Password = @Password,UpdateAt= GETDATE() 
    WHERE UserId = @UserId
 END
ELSE
 BEGIN
 INSERT INTO mst_users 
	(FirtsName,LastName,RoleId,EmailAddress,Password,CreateAt) 
VALUES
	(@FirtsName,@LastName,@RoleId,@EmailAddress,@Password,ISNULL(@CreateAt,GETDATE()))
 END
END
GO
CREATE OR ALTER PROCEDURE usp_Insert_Update_Leaveapplication
@LeaveId BIGINT = NULL,
@UserId BIGINT,
@LeaveStatusId BIGINT,
@LeaveType VARCHAR(500),
@StartDate Datetime,
@EndDate Datetime,
@Reason VARCHAR(MAX),
@AttachedFileName VARCHAR(500) = NULL
AS
BEGIN
IF EXISTS
(
    SELECT 1
    FROM tbl_leaveapplication
    WHERE LeaveId = @LeaveId
)

  BEGIN
   UPDATE tbl_leaveapplication SET StartDate = @StartDate, EndDate = @EndDate, Reason =@Reason,AttachedFileName =  ISNULL(@AttachedFileName, AttachedFileName),
		UpdateAt =GETDATE() WHERE LeaveId =@LeaveId
  END
ELSE
 BEGIN
 INSERT INTO 
	tbl_leaveapplication (UserId,LeaveStatusId,LeaveType,StartDate,EndDate,Reason,AttachedFileName)	
 VALUES 
	(@UserId,@LeaveStatusId,@LeaveType,@StartDate,@EndDate,@Reason, @AttachedFileName)
 END
END
GO
CREATE OR ALTER PROCEDURE usp_GetLeaveSummarybyUserId
(
    @UserId BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*) AS TotalLeave,
        SUM(CASE WHEN LeaveStatusId = 1 THEN 1 ELSE 0 END) AS PendingLeave,
        SUM(CASE WHEN LeaveStatusId = 2 THEN 1 ELSE 0 END) AS ApprovedLeave,
        SUM(CASE WHEN LeaveStatusId = 3 THEN 1 ELSE 0 END) AS RejectedLeave
    FROM tbl_leaveapplication
    WHERE UserId = @UserId
    AND IsDeleted = 0;
END
GO
CREATE OR ALTER PROCEDURE usp_GetLeaveSummary
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*) AS TotalLeave,
        SUM(CASE WHEN LeaveStatusId = 1 THEN 1 ELSE 0 END) AS PendingLeave,
        SUM(CASE WHEN LeaveStatusId = 2 THEN 1 ELSE 0 END) AS ApprovedLeave,
        SUM(CASE WHEN LeaveStatusId = 3 THEN 1 ELSE 0 END) AS RejectedLeave
    FROM tbl_leaveapplication
    WHERE IsDeleted = 0;
END

GO
CREATE OR ALTER PROCEDURE usp_GetLeaveList
@UserId BIGINT,
@PageNumber INT = 1,
@PageSize INT = 10,
@Search VARCHAR(100) = NULL
AS
BEGIN

DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

SELECT
        L.LeaveId,
        L.LeaveType,
        L.StartDate,
        L.LeaveStatusId,
        L.EndDate,
        DATEDIFF(DAY, L.StartDate, L.EndDate) + 1 AS TotalDays,
        L.Reason,
        L.HRComment,
        L.AttachedFileName,
        L.CreateAt
    FROM tbl_leaveapplication L
    INNER JOIN mst_leavestatus LS
        ON L.LeaveStatusId = LS.LeaveStatusId
    INNER JOIN mst_leavetype LT on LT.LeavetypeId = L.LeaveType
    WHERE
        L.UserId = @UserId
        AND L.IsDeleted = 0
        AND
        (
            ISNULL(@Search,'')=''
            OR LS.Description LIKE '%' + @Search + '%'
            OR LT.LeaveType LIKE '%' + @Search + '%'
        )

    ORDER BY L.CreateAt DESC
    OFFSET @Offset ROWS FETCH NEXT  @PageSize ROWS ONLY;

     SELECT COUNT(*) AS TotalRecords
    FROM tbl_leaveapplication L
    INNER JOIN mst_leavestatus LS
        ON L.LeaveStatusId = LS.LeaveStatusId
    INNER JOIN mst_leavetype LT on LT.LeavetypeId = L.LeaveType
    WHERE
        L.UserId = @UserId
        AND L.IsDeleted = 0
        AND
        (
            ISNULL(@Search,'')=''
            OR LS.Description LIKE '%' + @Search + '%'
            OR LT.LeaveType LIKE '%' + @Search + '%'
        );
END
GO
CREATE OR ALTER PROC usp_GetLeaveById
@LeaveId BIGINT
AS
BEGIN

SELECT
LeaveId,
LeaveType,
StartDate,
EndDate,
Reason,
AttachedFileName

FROM tbl_leaveapplication

WHERE LeaveId=@LeaveId

END
GO

CREATE OR ALTER PROCEDURE usp_GetEmployeeLeaveList
@UserId BIGINT= NULL,
@PageNumber INT = 1,
@PageSize INT = 10,
@Search VARCHAR(100) = NULL
AS
BEGIN

DECLARE @Offset INT = (@PageNumber - 1) * @PageSize

SELECT 
    (MU.FirtsName +' ' +  MU.LastName ) AS UserName,
    LA.LeaveId,
    LA.LeaveStatusId,
    La.LeaveType,
    LA.StartDate,
    LA.LeaveStatusId,
        LA.EndDate,
        DATEDIFF(DAY, LA.StartDate, LA.EndDate) + 1 AS TotalDays,
        LA.Reason,
        LA.HRComment,
        LA.AttachedFileName,
        LA.CreateAt    
FROM tbl_leaveapplication LA
INNER JOIN mst_leavestatus LS on LA.LeaveStatusId = LS.LeaveStatusId 
INNER JOIN mst_leavetype LT on LT.LeavetypeId= LA.LeaveStatusId
INNER JOIN mst_users MU on MU.UserId = LA.UserId
WHERE LA.IsDeleted =0
AND
(
    ISNULL(@Search , '') = ''
    OR MU.FirtsName LIKE  '%' + @Search + '%'
    OR MU.LastName LIKE  '%' + @Search + '%'
    OR LT.LeaveType LIKE '%' + @Search + '%'
    OR LS.Description LIKE '%' + @Search + '%'
)
ORDER BY LA.CreateAt DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;

SELECT 
    COUNT(*) AS TotalRecords  
FROM tbl_leaveapplication LA
INNER JOIN mst_leavestatus LS on LA.LeaveStatusId = LS.LeaveStatusId 
INNER JOIN mst_leavetype LT on LT.LeavetypeId= LA.LeaveStatusId
INNER JOIN mst_users MU on MU.UserId = LA.UserId
WHERE LA.IsDeleted =0
AND
(
    ISNULL(@Search , '') = ''
    OR MU.FirtsName LIKE  '%' + @Search + '%'
    OR MU.LastName LIKE  '%' + @Search + '%'
    OR LT.LeaveType LIKE '%' + @Search + '%'
    OR LS.Description LIKE '%' + @Search + '%'
)
END
GO
CREATE OR ALTER PROCEDURE usp_UpdateLeaveStatus
@LeaveId BIGINT,
@ActionType VARCHAR(50),
@HRComment VARCHAR(MAX) = NULL 
AS
BEGIN
IF @ActionType = 'Approved'
 BEGIN
  UPDATE tbl_leaveapplication SET LeaveStatusId = 2 , UpdateAt = GETDATE() WHERE LeaveId = @LeaveId
 END 
ELSE IF @ActionType = 'Rejected'
UPDATE tbl_leaveapplication SET LeaveStatusId = 3 ,HRComment = @HRComment, UpdateAt = GETDATE() WHERE LeaveId = @LeaveId
END
select * from tbl_leaveapplication
    --- select * from mst_users