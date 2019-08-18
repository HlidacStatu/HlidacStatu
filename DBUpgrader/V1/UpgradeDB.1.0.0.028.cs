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

			[DatabaseUpgradeMethod("1.0.0.28")]
			public static void Init_1_0_0_28(IDatabaseUpgrader du)
			{

                string sql = @"
/****** Object:  UserDefinedFunction [dbo].[RemoveAccent]    Script Date: 28.05.2017 14:23:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RemoveAccent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[RemoveAccent]
GO

/****** Object:  UserDefinedFunction [dbo].[RemoveAccent]    Script Date: 28.05.2017 14:23:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RemoveAccent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'
CREATE FUNCTION [dbo].[RemoveAccent] 
(
   @text nvarchar(max)
)
RETURNS nvarchar(max)
AS
BEGIN
	declare @ascii nvarchar(max)
	DECLARE @t NVARCHAR(1)
	DECLARE @a NVARCHAR(1)
	DECLARE @I INT
 
	SELECT @I = 0
	set @ascii = ''''
	 
	WHILE(@I < LEN(@text)+1)
	BEGIN
		set @t = SUBSTRING(@text,@I,1)
		select @a = 
			CASE @t
				WHEN N''ě'' THEN N''e''
				WHEN N''š'' THEN N''s''
				WHEN N''č'' THEN N''c''
				WHEN N''ř'' THEN N''r''
				WHEN N''ž'' THEN N''z''
				WHEN N''ý'' THEN N''y''
				WHEN N''á'' THEN N''a''
				WHEN N''í'' THEN N''i''
				WHEN N''é'' THEN N''e''
				WHEN N''ů'' THEN N''u''
				WHEN N''ú'' THEN N''u''
				WHEN N''ď'' THEN N''d''
				WHEN N''ň'' THEN N''n''
				WHEN N''ť'' THEN N''t''
				WHEN N''Ě'' THEN N''E''
				WHEN N''Š'' THEN N''S''
				WHEN N''Č'' THEN N''C''
				WHEN N''Ř'' THEN N''R''
				WHEN N''Ž'' THEN N''Z''
				WHEN N''Ý'' THEN N''Y''
				WHEN N''Á'' THEN N''A''
				WHEN N''Í'' THEN N''I''
				WHEN N''É'' THEN N''E''
				WHEN N''Ů'' THEN N''U''
				WHEN N''Ú'' THEN N''U''
				WHEN N''Ď'' THEN N''D''
				WHEN N''Ň'' THEN N''N''
				WHEN N''Ť'' THEN N''T''
				WHEN N''Ä'' THEN N''A''
				WHEN N''ä'' THEN N''a''
				WHEN N''Ö'' THEN N''O''
				WHEN N''ö'' THEN N''o''
				WHEN N''Ü'' THEN N''U''
				WHEN N''ü'' THEN N''u''
				WHEN N''ß'' THEN N''ß''
				WHEN N''À'' THEN N''A''
				WHEN N''à'' THEN N''a''
				WHEN N''Â'' THEN N''A''
				WHEN N''â'' THEN N''a''
				WHEN N''Æ'' THEN N''Æ''
				WHEN N''æ'' THEN N''æ''
				WHEN N''Ç'' THEN N''C''
				WHEN N''ç'' THEN N''c''
				WHEN N''È'' THEN N''E''
				WHEN N''è'' THEN N''e''
				WHEN N''Ê'' THEN N''E''
				WHEN N''ê'' THEN N''e''
				WHEN N''Ë'' THEN N''E''
				WHEN N''ë'' THEN N''e''
				WHEN N''Î'' THEN N''I''
				WHEN N''î'' THEN N''i''
				WHEN N''Ï'' THEN N''I''
				WHEN N''ï'' THEN N''i''
				WHEN N''Ô'' THEN N''O''
				WHEN N''ô'' THEN N''o''
				WHEN N''Œ'' THEN N''Œ''
				WHEN N''œ'' THEN N''œ''
				WHEN N''Ù'' THEN N''U''
				WHEN N''ù'' THEN N''u''
				WHEN N''Û'' THEN N''U''
				WHEN N''û'' THEN N''u''
				WHEN N''Ü'' THEN N''U''
				WHEN N''ü'' THEN N''u''
				WHEN N''Ÿ'' THEN N''Y''
				WHEN N''ÿ'' THEN N''y''
				ELSE @t
			end
		set @ascii = @ascii + @a
		SET @I = @I + 1
 
	END

	return @ascii
END
' 
END

GO


ALTER TABLE dbo.Firma ADD
	JmenoAscii nvarchar(500) NULL
GO

ALTER TABLE dbo.Osoba ADD
	JmenoAscii nvarchar(150) NULL,
	PrijmeniAscii nvarchar(150) NULL,
	PuvodniPrijmeniAscii nvarchar(150) NULL
GO

update firma
set jmenoAscii = dbo.RemoveAccent(jmeno)
GO

update osoba
set jmenoAscii = dbo.RemoveAccent(jmeno),
prijmeniAscii = dbo.RemoveAccent(prijmeni),
puvodniprijmeniAscii = dbo.RemoveAccent(puvodniprijmeni)

GO


GO
CREATE NONCLUSTERED INDEX [idx_firma_jmenoascii] ON [dbo].[Firma]
(
	[JmenoAscii] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

CREATE NONCLUSTERED INDEX [idx_osoba_jmenoascii] ON [dbo].[Osoba]
(
	[JmenoAscii] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

CREATE NONCLUSTERED INDEX [idx_osoba_prijmeniascii] ON [dbo].[Osoba]
(
	[prijmeniAscii] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

--EXEC [Firmy].sys.sp_updateextendedproperty @name=N'_DatabaseUpgrader', @value=N'1.0.0.28' 

GO
update osoba
set status = 2
where InternalId in (select o.InternalId from Osoba o inner join OsobaEvent oe on o.internalId = oe.osobaid
where oe.type = 3 and o.Status != 2)

GO

";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
