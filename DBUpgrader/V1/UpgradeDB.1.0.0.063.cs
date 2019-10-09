using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.63")]
			public static void Init_1_0_0_63(IDatabaseUpgrader du)
			{
                string sql = @"
CREATE TABLE EventType(
   Id int,
   Name nvarchar(200),
   PRIMARY KEY(Id)
);

--inserty do event type 
insert into EventType(Id,Name) values (0,N'Speciální')
insert into EventType(Id,Name) values (1,N'Volená funkce')
insert into EventType(Id,Name) values (2,N'Soukromá pracovní')
insert into EventType(Id,Name) values (3,N'Sponzor')
insert into EventType(Id,Name) values (4,N'Osobní')
insert into EventType(Id,Name) values (6,N'Veřejná správa pracovní')
insert into EventType(Id,Name) values (7,N'Politická')
insert into EventType(Id,Name) values (9,N'Politická pracovní')
insert into EventType(Id,Name) values (10,N'Veřejná správa jiné')
insert into EventType(Id,Name) values (11,N'Jiné')

alter table OsobaEvent
   add Organizace nvarchar(max);

-- přejmenovat description na note
EXEC sp_RENAME 'OsobaEvent.Description' , 'Note', 'COLUMN'

";
    du.RunDDLCommands(sql);

            }
        }
	}
}
