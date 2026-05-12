-- =========================================================================================
-- Script Name: MSE Application Processing Schema
-- Description: Standard, Normalized Database Structure for Preferential Issue Applications 
--              (In-Principle Approval, Listing, Trading)
-- =========================================================================================

-- 1. Master Tables

CREATE TABLE [dbo].[Master_ApprovalTypes] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Code] NVARCHAR(10) NOT NULL, -- e.g., 'IPA', 'LST', 'TRD'
    [Name] NVARCHAR(100) NOT NULL,
    
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[Master_SecurityTypes] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Code] NVARCHAR(50) NOT NULL, 
    [Name] NVARCHAR(100) NOT NULL, -- e.g., Equity, Convertible Securities, Both, Others
    
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[Master_ApplicationStatus] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [StatusCode] NVARCHAR(50) NOT NULL,
    [StatusName] NVARCHAR(100) NOT NULL, -- e.g., 'In Draft', 'Submitted', 'Maker In Progress'
    
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[Master_DocumentChecklist] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ApprovalTypeId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Master_ApprovalTypes]([Id]),
    [SecurityTypeId] INT NULL FOREIGN KEY REFERENCES [dbo].[Master_SecurityTypes]([Id]),
    [DocumentName] NVARCHAR(500) NOT NULL,
    [IsMandatory] BIT NOT NULL DEFAULT 1,
    
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[Master_SlaConfiguration] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ConfigKey] NVARCHAR(100) NOT NULL, -- e.g., DraftExpiryDays, MakerProcessingDays
    [ConfigValue] INT NOT NULL,
    [Description] NVARCHAR(500) NULL,
    
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

-- 2. User & Company Tables

CREATE TABLE [dbo].[Companies] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyName] NVARCHAR(255) NOT NULL,
    [Symbol] NVARCHAR(100) NOT NULL,
    [ISIN] NVARCHAR(50) NOT NULL,
    [OldName] NVARCHAR(255) NULL,
    [RegisteredAddress] NVARCHAR(MAX) NOT NULL,
    [CorporateAddress] NVARCHAR(MAX) NOT NULL,
    [ContactPerson] NVARCHAR(255) NOT NULL,
    [TelephoneNos] NVARCHAR(100) NOT NULL,
    [MobileNo] NVARCHAR(50) NOT NULL,
    [EmailId] NVARCHAR(255) NOT NULL,
    [PanNo] NVARCHAR(50) NOT NULL,
    [GstNo] NVARCHAR(50) NOT NULL,
    [CsName] NVARCHAR(255) NOT NULL,
    [CsContactDetails] NVARCHAR(MAX) NOT NULL,
    [ListingStatus] NVARCHAR(100) NOT NULL,
    
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[AppUsers] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyId] INT NULL FOREIGN KEY REFERENCES [dbo].[Companies]([Id]),
    [FullName] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Role] NVARCHAR(50) NOT NULL, -- e.g., Admin, HOD, TeamLeader, Checker, Maker, Company
    [IsActive] BIT NOT NULL DEFAULT 1,
    
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

-- 3. Core Application Tables

CREATE TABLE [dbo].[Applications] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [CaseNumber] NVARCHAR(50) NOT NULL, -- Unique YYYY0001
    [CompanyId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Companies]([Id]),
    [ApplicationType] NVARCHAR(100) NOT NULL DEFAULT 'Preferential',
    [ApprovalTypeId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Master_ApprovalTypes]([Id]),
    [SecurityTypeId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Master_SecurityTypes]([Id]),
    [SecurityTypeRemarks] NVARCHAR(255) NULL,
    
    -- Self Referencing for Hierarchy (Listing -> IPA, Trading -> Listing)
    [ParentApplicationId] INT NULL FOREIGN KEY REFERENCES [dbo].[Applications]([Id]),
    
    [StatusId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Master_ApplicationStatus]([Id]),
    [AssignedMakerId] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [AssignedCheckerId] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    
    [DateOfApplication] DATETIME2 NULL,
    [ApprovalLetterPath] NVARCHAR(MAX) NULL,
    [ApprovalNotePath] NVARCHAR(MAX) NULL,
    [LastSubmissionDate] DATETIME2 NULL,
    
    [CreatedBy] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[Application_ListingDetails] (
    [ApplicationId] INT PRIMARY KEY FOREIGN KEY REFERENCES [dbo].[Applications]([Id]),
    [BoardApprovalDate] DATE NULL,
    [RelevantDate] DATE NULL,
    [LockInStartDate] DATE NULL,
    [LockInEndDate] DATE NULL,
    [PromoterCount] INT NULL,
    [PromoterAllottedShares] BIGINT NULL,
    [NonPromoterCount] INT NULL,
    [NonPromoterAllottedShares] BIGINT NULL,
    [OfferPrice] DECIMAL(18,4) NULL,
    [MinimumIssuePrice] DECIMAL(18,4) NULL,
    [FaceValue] DECIMAL(18,4) NULL,
    [NoOfSecuritiesIssued] BIGINT NULL,
    [NoOfSecuritiesAlreadyConverted] BIGINT NULL,
    [NoOfSecuritiesOutstanding] BIGINT NULL,
    [NoOfSecuritiesToBeConverted] BIGINT NULL,
    [DelayInDays] INT NULL,
    [DelayRemark] NVARCHAR(MAX) NULL,
    
    [CreatedBy] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[Application_TradingDetails] (
    [ApplicationId] INT PRIMARY KEY FOREIGN KEY REFERENCES [dbo].[Applications]([Id]),
    [NoOfSharesCredited] BIGINT NULL,
    [DateOfCredit] DATE NULL,
    [Depository] NVARCHAR(100) NULL,
    [LockInStartDate] DATE NULL,
    [LockInEndDate] DATE NULL,
    [LockInPeriodMonths] INT NULL,
    [PriceOnLockInEndDate] DECIMAL(18,4) NULL,
    [DistinctiveNos] NVARCHAR(MAX) NULL,
    
    [CreatedBy] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

-- 4. Workflows & Documents

CREATE TABLE [dbo].[ApplicationDocuments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ApplicationId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Applications]([Id]),
    [DocumentChecklistId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Master_DocumentChecklist]([Id]),
    [FilePath] NVARCHAR(MAX) NOT NULL,
    [UploadedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    [CreatedBy] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[ApplicationObservations] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ApplicationId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Applications]([Id]),
    [FieldReference] NVARCHAR(255) NULL, -- Identifies which field/document the observation refers to
    
    [RaisedById] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [SentToRoleId] NVARCHAR(50) NOT NULL, -- e.g., 'Company', 'Maker'
    [ObservationRemarks] NVARCHAR(MAX) NOT NULL,
    [ObservationAttachmentPath] NVARCHAR(MAX) NULL,
    [ObservationDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    [RepliedById] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [ReplyRemarks] NVARCHAR(MAX) NULL,
    [ReplyAttachmentPath] NVARCHAR(MAX) NULL,
    [ReplyDate] DATETIME2 NULL,
    
    [IsResolved] BIT NOT NULL DEFAULT 0,
    
    [CreatedBy] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[ApplicationWorkflowHistory] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ApplicationId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Applications]([Id]),
    [ActionById] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    
    [ActionType] NVARCHAR(100) NOT NULL, -- e.g., Submit, Assign, Forward, Approve, RaiseObservation
    [FromStatusId] INT NULL FOREIGN KEY REFERENCES [dbo].[Master_ApplicationStatus]([Id]),
    [ToStatusId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Master_ApplicationStatus]([Id]),
    
    [Remarks] NVARCHAR(MAX) NULL,
    [ActionDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    [CreatedBy] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL FOREIGN KEY REFERENCES [dbo].[AppUsers]([Id]),
    [UpdatedDate] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);
