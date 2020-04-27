using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.70")]
			public static void Init_1_0_0_70(IDatabaseUpgrader du)
			{

				string sql = @"
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

BEGIN
CREATE TABLE dbo.ClassificationOverride
(
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	IdSmlouvy nvarchar(50) NOT NULL,
	OriginalCat1 int NULL,
	OriginalCat2 int NULL,
	CorrectCat1 int NULL,
	CorrectCat2 int NULL,
	CreatedBy nvarchar(150) NULL,
	Created datetime NULL
)  ON [PRIMARY]
END
GO

";
				du.RunDDLCommands(sql);

			}
		}
	}
}
