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

			[DatabaseUpgradeMethod("1.0.0.61")]
			public static void Init_1_0_0_61(IDatabaseUpgrader du)
			{
                string sql = @"
ALTER TABLE dbo.Firma ADD
	KrajId nvarchar(5) NULL,
	OkresId nvarchar(7) NULL

GO

/****** Object:  StoredProcedure [dbo].[Firma_Save]    Script Date: 27. 8. 2019 9:17:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Firma_Save]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'ALTER PROCEDURE [dbo].[Firma_Save]
	-- Add the parameters for the stored procedure here

	@ICO nvarchar(30),
    @DIC nvarchar(30),
    @Datum_zapisu_OR date,
    @Stav_subjektu tinyint,
    @Jmeno nvarchar(500),
    @JmenoAscii nvarchar(500),
    @Kod_PF int,
	@Source nvarchar(100),
	@Popis nvarchar(100),
    @VersionUpdate int,
	@KrajId nvarchar(5),
	@okresId nvarchar(7)

AS
BEGIN


IF EXISTS(SELECT ico FROM firma WITH(NOLOCK)
          WHERE ico = @ico)
    BEGIN
		UPDATE [dbo].[Firma]
		   SET 
			  [DIC] = @DIC, 
			  [Datum_zapisu_OR] = @Datum_zapisu_OR, 
			  [Stav_subjektu] = @Stav_subjektu, 
			  [Jmeno] = @Jmeno, 
			  JmenoAscii = @JmenoAscii,
			  [Kod_PF] = @Kod_PF, 
			  [Source] = @source, 
			  [Popis] = @popis, 
			  [VersionUpdate] = @VersionUpdate,
			  krajId = @krajId,
			  okresId = @okresId

		 WHERE [ICO] = @ico

    END
ELSE
    BEGIN
		INSERT INTO [dbo].[Firma] ([ICO],[DIC],[Datum_zapisu_OR],[Stav_subjektu],[Jmeno],[Kod_PF],[VersionUpdate], source, popis, krajId, okresId)
		VALUES (@ICO,@DIC,@Datum_zapisu_OR,@Stav_subjektu,@Jmeno,@Kod_PF,@VersionUpdate, @source, @popis, @krajId, @okresId)
    END


END
' 
END

";
    du.RunDDLCommands(sql);

            }




        }

	}
}
