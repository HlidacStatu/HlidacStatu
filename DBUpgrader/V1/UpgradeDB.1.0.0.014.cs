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

			[DatabaseUpgradeMethod("1.0.0.14")]
			public static void Init_1_0_0_14(IDatabaseUpgrader du)
			{
				string sql = @"

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_SmlouvyIds_active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SmlouvyIds] DROP CONSTRAINT [DF_SmlouvyIds_active]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_SmlouvyIds_updated]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SmlouvyIds] DROP CONSTRAINT [DF_SmlouvyIds_updated]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_SmlouvyIds_created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SmlouvyIds] DROP CONSTRAINT [DF_SmlouvyIds_created]
END

GO

/****** Object:  Table [dbo].[SmlouvyIds]    Script Date: 14.04.2017 4:53:44 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmlouvyIds]') AND type in (N'U'))
DROP TABLE [dbo].[SmlouvyIds]
GO

/****** Object:  Table [dbo].[SmlouvyIds]    Script Date: 14.04.2017 4:53:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmlouvyIds]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SmlouvyIds](
	[Id] [nvarchar](50) NOT NULL,
	[created] [datetime] NOT NULL,
	[updated] [datetime] NOT NULL,
	[active] [int] NOT NULL,
 CONSTRAINT [PK_SmlouvyIds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_SmlouvyIds_created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SmlouvyIds] ADD  CONSTRAINT [DF_SmlouvyIds_created]  DEFAULT (getdate()) FOR [created]
END

GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_SmlouvyIds_updated]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SmlouvyIds] ADD  CONSTRAINT [DF_SmlouvyIds_updated]  DEFAULT (getdate()) FOR [updated]
END

GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_SmlouvyIds_active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SmlouvyIds] ADD  CONSTRAINT [DF_SmlouvyIds_active]  DEFAULT ((1)) FOR [active]
END

GO

/****** Object:  StoredProcedure [dbo].[SmlouvaId_Save]    Script Date: 14.04.2017 5:06:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmlouvaId_Save]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
ALTER PROCEDURE [dbo].[SmlouvaId_Save] 
	@id nvarchar(50),
    @active int,
	@created datetime = null,
	@updated datetime = null
AS
BEGIN
	SET NOCOUNT ON;
	declare @dt datetime;
	set @dt = getdate();

if (@created is null)
	set @created=@dt;
if (@updated is null)
	set @updated=@dt;

IF EXISTS(SELECT id FROM SmlouvyIds 
          WHERE id=@id)
    BEGIN
		update SmlouvyIds
		set updated = @updated, active = @active
		where id=@id
	END
	ELSE
    BEGIN
		insert into SmlouvyIds(id, created, updated, active) values(@id, @created,@updated,@active)
	END


END' 
END

GO

";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
