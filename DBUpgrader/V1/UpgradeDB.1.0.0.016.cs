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

			[DatabaseUpgradeMethod("1.0.0.16")]
			public static void Init_1_0_0_16(IDatabaseUpgrader du)
			{

                //fix of 1.0.0.14 script

                string sql = @"
        EXECUTE sp_rename @objname = N'[dbo].[AngazovanostFirma]', @newname = N'AngazovanostFirma_2017', @objtype = N'OBJECT';

GO
        EXECUTE sp_rename @objname = N'[dbo].[Angazovanost]', @newname = N'Angazovanost_2017', @objtype = N'OBJECT';
GO

/****** Object:  Table [dbo].[FirmaVazby]    Script Date: 04.05.2017 14:18:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FirmaVazby]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FirmaVazby](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[ICO] [nvarchar](30) NOT NULL,
	[VazbakICO] [nvarchar](30) NOT NULL,
	[TypVazby] [int] NULL,
	[PojmenovaniVazby] [nvarchar](150) NULL,
	[podil] [decimal](18, 9) NULL,
	[odDatum] [date] NULL,
	[doDatum] [date] NULL,
	[Zdroj] [nvarchar](50) NULL,
	[LastUpdate] [date] NOT NULL,
 CONSTRAINT [PK_FirmaVazby] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

SET ANSI_PADDING ON

GO

/****** Object:  Index [IX_FirmaVazby_ICO]    Script Date: 04.05.2017 14:18:32 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FirmaVazby]') AND name = N'IX_FirmaVazby_ICO')
CREATE NONCLUSTERED INDEX [IX_FirmaVazby_ICO] ON [dbo].[FirmaVazby]
(
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

SET ANSI_PADDING ON

GO

/****** Object:  Index [IX_FirmaVazby_Vazby]    Script Date: 04.05.2017 14:18:32 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FirmaVazby]') AND name = N'IX_FirmaVazby_Vazby')
CREATE NONCLUSTERED INDEX [IX_FirmaVazby_Vazby] ON [dbo].[FirmaVazby]
(
	[VazbakICO] ASC,
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_FirmaVazby_LastUpdate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FirmaVazby] ADD  CONSTRAINT [DF_FirmaVazby_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
END

GO

/****** Object:  Table [dbo].[Osoba]    Script Date: 04.05.2017 14:18:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Osoba]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Osoba](
	[InternalId] [int] IDENTITY(1,1) NOT NULL,
	[TitulPred] [nchar](10) NULL,
	[Jmeno] [nvarchar](150) NOT NULL,
	[Prijmeni] [nvarchar](150) NOT NULL,
	[TitulPo] [nchar](10) NULL,
	[Pohlavi] [char](1) NULL,
	[Narozeni] [date] NULL,
	[Umrti] [date] NULL,
	[Ulice] [nvarchar](500) NULL,
	[Mesto] [nvarchar](150) NULL,
	[PSC] [nvarchar](25) NULL,
	[CountryCode] [nvarchar](5) NULL,
	[OnRadar] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
	[NameId] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Osoba_2] PRIMARY KEY CLUSTERED 
(
	[InternalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Osoba_OnRadar]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Osoba] ADD  CONSTRAINT [DF_Osoba_OnRadar]  DEFAULT ((0)) FOR [OnRadar]
END

GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Osoba_LastUpdate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Osoba] ADD  CONSTRAINT [DF_Osoba_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
END

GO

/****** Object:  Table [dbo].[OsobaEvent]    Script Date: 04.05.2017 14:19:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OsobaEvent]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OsobaEvent](
	[pk] [int] NOT NULL,
	[OsobaId] [int] NOT NULL,
	[Title] [nvarchar](200) NULL,
	[Description] [nvarchar](max) NULL,
	[FromDate] [date] NOT NULL,
	[ToDate] [date] NULL,
	[Type] [int] NOT NULL,
 CONSTRAINT [PK_OsobaEvent_1] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

/****** Object:  Index [IX_OsobaEvent_Osoba]    Script Date: 04.05.2017 14:19:04 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OsobaEvent]') AND name = N'IX_OsobaEvent_Osoba')
CREATE NONCLUSTERED INDEX [IX_OsobaEvent_Osoba] ON [dbo].[OsobaEvent]
(
	[OsobaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[OsobaExternalId]    Script Date: 04.05.2017 14:19:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OsobaExternalId]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OsobaExternalId](
	[OsobaId] [int] NOT NULL,
	[ExternalId] [nvarchar](50) NOT NULL,
	[ExternalSource] [int] NOT NULL,
 CONSTRAINT [PK_OsobaExternalId] PRIMARY KEY CLUSTERED 
(
	[OsobaId] ASC,
	[ExternalId] ASC,
	[ExternalSource] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[OsobaVazby]    Script Date: 04.05.2017 14:19:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OsobaVazby]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OsobaVazby](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[OsobaID] [int] NOT NULL,
	[VazbakICO] [nvarchar](30) NOT NULL,
	[TypVazby] [int] NULL,
	[PojmenovaniVazby] [nvarchar](150) NULL,
	[podil] [decimal](18, 9) NULL,
	[odDatum] [date] NULL,
	[doDatum] [date] NULL,
	[Zdroj] [nvarchar](50) NULL,
	[LastUpdate] [date] NOT NULL,
 CONSTRAINT [PK_OsobaVazby] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Index [IX_OsobaVazby_OsobaId]    Script Date: 04.05.2017 14:19:30 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OsobaVazby]') AND name = N'IX_OsobaVazby_OsobaId')
CREATE NONCLUSTERED INDEX [IX_OsobaVazby_OsobaId] ON [dbo].[OsobaVazby]
(
	[OsobaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

SET ANSI_PADDING ON

GO

/****** Object:  Index [IX_OsobaVazby_Vazby]    Script Date: 04.05.2017 14:19:30 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OsobaVazby]') AND name = N'IX_OsobaVazby_Vazby')
CREATE NONCLUSTERED INDEX [IX_OsobaVazby_Vazby] ON [dbo].[OsobaVazby]
(
	[OsobaID] ASC,
	[VazbakICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Table_1_PlatneKDatum]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[OsobaVazby] ADD  CONSTRAINT [DF_Table_1_PlatneKDatum]  DEFAULT (getdate()) FOR [LastUpdate]
END

GO



";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
