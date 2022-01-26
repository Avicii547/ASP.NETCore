
CREATE TABLE [dbo].[Skill](
	PKey [uniqueidentifier] NOT NULL DEFAULT (newid()) primary key nonclustered,
	UniqueKey SMALLINT IDENTITY(-32767,1) NOT NULL UNIQUE CLUSTERED,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	DisplayOrder smallint NOT NULL,
	[Active] [bit] NOT NULL DEFAULT (1),
	[LastUpdatedBy] [nvarchar](255) NOT NULL DEFAULT (suser_sname()),
	[LastUpdatedOn] [datetime2] NOT NULL DEFAULT (getutcdate())
)
GO
Insert into Skill(Name,Description,DisplayOrder,Active)Values('ASP.NETCore','ASP.NET',1,1)
Insert into Skill(Name,Description,DisplayOrder,Active)Values('C#','C#',2,1)
Insert into Skill(Name,Description,DisplayOrder,Active)Values('SQL Server','SQL Server',3,1)

CREATE TABLE [dbo].[User](
	PKey [uniqueidentifier] NOT NULL DEFAULT (newid()) primary key nonclustered,
	UniqueKey SMALLINT IDENTITY(-32767,1) NOT NULL UNIQUE CLUSTERED,
	[Name] [nvarchar](255) NOT NULL,
	[DOB] date NOT NULL,
	[Designation] [nvarchar](255) NOT NULL,
	DisplayOrder smallint NOT NULL,
	[Active] [bit] NOT NULL DEFAULT (1),
	[LastUpdatedBy] [nvarchar](255) NOT NULL DEFAULT (suser_sname()),
	[LastUpdatedOn] [datetime2] NOT NULL DEFAULT (getutcdate())
)
GO

CREATE TABLE [dbo].[UserSkill](
	PKey [uniqueidentifier] NOT NULL DEFAULT (newid()) primary key nonclustered,
	UniqueKey SMALLINT IDENTITY(-32767,1) NOT NULL UNIQUE CLUSTERED,
	[UserKey] [uniqueidentifier] NULL references [dbo].[User] (PKey),
	[SkillKey] [uniqueidentifier] NULL references [dbo].Skill (PKey),
	[Active] [bit] NOT NULL DEFAULT (1),
	[LastUpdatedBy] [nvarchar](255) NOT NULL DEFAULT (suser_sname()),
	[LastUpdatedOn] [datetime2] NOT NULL DEFAULT (getutcdate())
)
GO

IF Exists (select name FROM sysobjects
      where name = 'UpdateUserDetails' AND type = 'P')
   drop PROCEDURE dbo.UpdateUserDetails
GO

	CREATE PROCEDURE [dbo].UpdateUserDetails @PKey uniqueidentifier
	AS
	BEGIN
	Update [dbo].[User] SET
LastUpdatedOn = GETUTCDATE(),
Active=0
where PKey=@PKey and Active=1;
	
	Delete from UserSkill where UserKey=@PKey and Active=1;
	END
GO