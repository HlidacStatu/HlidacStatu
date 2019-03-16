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

			[DatabaseUpgradeMethod("1.0.0.17")]
			public static void Init_1_0_0_17(IDatabaseUpgrader du)
			{

                //fix of 1.0.0.14 script

                string sql = @"
USE [Firmy]
GO

/****** Object:  Index [idx_OsobaExternalId_external]    Script Date: 4. 5. 2017 23:47:32 ******/
if (EXISTS(SELECT * FROM sys.indexes WHERE name='idx_OsobaExternalId_external' AND object_id = OBJECT_ID('dbo.OsobaExternalId')))
BEGIN
    DROP INDEX [idx_OsobaExternalId_external] ON [dbo].[OsobaExternalId]
END
GO

/****** Object:  Index [idx_OsobaExternalId_external]    Script Date: 4. 5. 2017 23:47:32 ******/
CREATE NONCLUSTERED INDEX [idx_OsobaExternalId_external] ON [dbo].[OsobaExternalId]
(
	[ExternalId] ASC,
	[ExternalSource] ASC
)
INCLUDE ( 	[OsobaId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

USE [Firmy]
GO

/****** Object:  Index [IX_OsobaEvent_Osoba]    Script Date: 4. 5. 2017 23:48:20 ******/
if (EXISTS(SELECT * FROM sys.indexes WHERE name='IX_OsobaEvent_Osoba' AND object_id = OBJECT_ID('dbo.OsobaEvent')))
BEGIN
    DROP INDEX [IX_OsobaEvent_Osoba] ON [dbo].[OsobaEvent]
END
GO

/****** Object:  Index [IX_OsobaEvent_Osoba]    Script Date: 4. 5. 2017 23:48:20 ******/
CREATE NONCLUSTERED INDEX [IX_OsobaEvent_Osoba] ON [dbo].[OsobaEvent]
(
	[OsobaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



";
				du.RunDDLCommands(sql);


			}

	


		}

	}
}
