using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseUpgrader;
using System.Configuration;


namespace HlidacStatu.DBUpgrades
{
	public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.43")]
			public static void Init_1_0_0_43(IDatabaseUpgrader du)
			{

                string sql = @"
/****** Object:  Table [dbo].[Visit]    Script Date: 13/04/2018 19:57:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Visit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Visit](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[page] [nvarchar](250) NOT NULL,
	[date] [date] NOT NULL,
	[count] [int] NOT NULL,
	[channel] [int] NOT NULL,
 CONSTRAINT [PK_Visit] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

SET ANSI_PADDING ON

GO

/****** Object:  Index [IX_Visit_page_date]    Script Date: 13/04/2018 19:57:50 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Visit]') AND name = N'IX_Visit_page_date')
CREATE NONCLUSTERED INDEX [IX_Visit_page_date] ON [dbo].[Visit]
(
	[page] ASC,
	[date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Visit_channel]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Visit] ADD  CONSTRAINT [DF_Visit_channel]  DEFAULT ((0)) FOR [channel]
END

GO

/****** Object:  Table [dbo].[Visit]    Script Date: 13/04/2018 19:58:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Visit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Visit](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[page] [nvarchar](250) NOT NULL,
	[date] [date] NOT NULL,
	[count] [int] NOT NULL,
	[channel] [int] NOT NULL,
 CONSTRAINT [PK_Visit] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO

SET ANSI_PADDING ON

GO

/****** Object:  Index [IX_Visit_page_date]    Script Date: 13/04/2018 19:58:11 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Visit]') AND name = N'IX_Visit_page_date')
CREATE NONCLUSTERED INDEX [IX_Visit_page_date] ON [dbo].[Visit]
(
	[page] ASC,
	[date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Visit_channel]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Visit] ADD  CONSTRAINT [DF_Visit_channel]  DEFAULT ((0)) FOR [channel]
END

GO

/****** Object:  StoredProcedure [dbo].[AddVisit]    Script Date: 13/04/2018 19:58:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddVisit]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[AddVisit] AS' 
END
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[AddVisit]
	-- Add the parameters for the stored procedure here
	@page nvarchar(250),
	@date date,
    @channel int = 0
AS
BEGIN

IF EXISTS(SELECT pk FROM visit
          WHERE [page]=@page and [date]=@date and channel = @channel)
    BEGIN
		update visit
		set [count] = [count]+1
		WHERE [page]=@page and [date]=@date and channel = @channel
	END
	ELSE
    BEGIN
		insert into visit([page], [date], [count], channel) values(@page, @date, 1, @channel)
	END

END

GO



";

                du.RunDDLCommands(sql);



            }




        }

	}
}
