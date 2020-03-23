using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.67")]
			public static void Init_1_0_0_67(IDatabaseUpgrader du)
			{

				string sql = @"

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.UserOptions
	DROP CONSTRAINT DF_UserOptions_Created
GO
CREATE TABLE dbo.Tmp_UserOptions
	(
	pk int NOT NULL IDENTITY (1, 1),
	UserId nvarchar(128) NULL,
	OptionId int NOT NULL,
	Created datetime NOT NULL,
	Value nvarchar(MAX) NULL,
	LanguageId int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_UserOptions SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_UserOptions ADD CONSTRAINT
	DF_UserOptions_Created DEFAULT (getdate()) FOR Created
GO
SET IDENTITY_INSERT dbo.Tmp_UserOptions ON
GO
IF EXISTS(SELECT * FROM dbo.UserOptions)
	 EXEC('INSERT INTO dbo.Tmp_UserOptions (pk, UserId, OptionId, Created, Value, LanguageId)
		SELECT pk, CONVERT(nvarchar(128), UserId), OptionId, Created, Value, LanguageId FROM dbo.UserOptions WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_UserOptions OFF
GO
DROP TABLE dbo.UserOptions
GO
EXECUTE sp_rename N'dbo.Tmp_UserOptions', N'UserOptions', 'OBJECT' 
GO
ALTER TABLE dbo.UserOptions ADD CONSTRAINT
	PK_UserOptions PRIMARY KEY CLUSTERED 
	(
	pk
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

GO
/****** Object:  StoredProcedure [dbo].[UserOption_Remove]    Script Date: 23.03.2020 20:24:44 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UserOption_Remove]
	@optionId int,
	@UserId nvarchar(128) = null,
	@languageId int = null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	delete from [UserOptions]
	where optionId = @optionId and
	(@UserId is null and UserId is null or UserId = @UserId)
	and (@languageId is null and languageid is null or  languageid = @languageId)
	
END

GO
/****** Object:  StoredProcedure [dbo].[UserOption_Add]    Script Date: 23.03.2020 20:24:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UserOption_Add]
	@optionId int,
	@userId nvarchar(128) = null,
	@value nvarchar(max) = null,
	@languageId int = null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @pk int
	select @pk = pk from [UserOptions] where optionId = @optionId and
		(@UserId is null and UserId is null or UserId = @UserId)
		and (@languageId is null and languageid is null or  languageid = @languageId)
	
	if (@pk is not null)
	begin
		update [UserOptions]
		set value = @value,  created = GETDATE(), languageid = @languageid
		where pk = @pk
		
		select @pk
	end
	else	
	begin 
		insert into [UserOptions](UserId, optionId, value, languageid)
		values (@UserId, @optionId, @value,@languageid)
		
		set @pk = SCOPE_IDENTITY() 
		select @pk
	end
END

GO
/****** Object:  StoredProcedure [dbo].[UserOption_Get]    Script Date: 23.03.2020 20:24:33 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UserOption_Get]
	@optionId int,
	@UserId nvarchar(128) = null,
	@languageId int = null	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select * from [UserOptions]
	where optionId = @optionId and 
	(@UserId is null and UserId is null or UserId = @UserId)
	and (@languageId is null and languageid is null or  languageid = @languageId)
END

";
				du.RunDDLCommands(sql);

			}
		}
	}
}
