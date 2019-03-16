using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseUpgrader;
using System.Configuration;


namespace HlidacSmluv.DBUpgrades
{
	public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.55")]
			public static void Init_1_0_0_55(IDatabaseUpgrader du)
			{

                string sql = @"

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_UserOptions_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserOptions] DROP CONSTRAINT [DF_UserOptions_Created]
END

GO

/****** Object:  Table [dbo].[UserOptions]    Script Date: 10/30/2015 15:25:20 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOptions]') AND type in (N'U'))
DROP TABLE [dbo].[UserOptions]
GO

/****** Object:  Table [dbo].[UserOptions]    Script Date: 10/30/2015 15:25:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOptions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserOptions](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[OptionId] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Value] [nvarchar](max) NULL,
	[LanguageId] [int] NULL,
 CONSTRAINT [PK_UserOptions] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_UserOptions_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[UserOptions] ADD  CONSTRAINT [DF_UserOptions_Created]  DEFAULT (getdate()) FOR [Created]
END

GO

/****** Object:  StoredProcedure [dbo].[UserOption_Add]    Script Date: 10/30/2015 15:51:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Add]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserOption_Add]
GO

/****** Object:  StoredProcedure [dbo].[UserOption_Add]    Script Date: 10/30/2015 15:51:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Add]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UserOption_Add]
	@optionId int,
	@userId int = null,
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
END' 
END
GO



/****** Object:  StoredProcedure [dbo].[UserOption_Get]    Script Date: 10/30/2015 15:51:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserOption_Get]
GO

/****** Object:  StoredProcedure [dbo].[UserOption_Get]    Script Date: 10/30/2015 15:51:31 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UserOption_Get]
	@optionId int,
	@UserId int = null,
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
END' 
END
GO

/****** Object:  StoredProcedure [dbo].[UserOption_Remove]    Script Date: 10/30/2015 15:53:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Remove]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserOption_Remove]
GO

/****** Object:  StoredProcedure [dbo].[UserOption_Remove]    Script Date: 10/30/2015 15:53:14 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Remove]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UserOption_Remove]
	@optionId int,
	@UserId int = null,
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
	
END' 
END
GO


";
                du.RunDDLCommands(sql);


            }




        }

	}
}
