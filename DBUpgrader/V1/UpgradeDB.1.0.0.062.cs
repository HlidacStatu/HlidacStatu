using DatabaseUpgrader;


namespace HlidacStatu.DBUpgrades
{
    public static partial class DBUpgrader
	{

		private partial class UpgradeDB
		{

			[DatabaseUpgradeMethod("1.0.0.62")]
			public static void Init_1_0_0_62(IDatabaseUpgrader du)
			{
                string sql = @"
CREATE TABLE EventType(
   Id int,
   Name nvarchar(200),
   PRIMARY KEY(Id)
)

CREATE TABLE EventSubType(
   Id int,
   EventTypeId int,
   NameMale nvarchar(200),
   NameFemale nvarchar(200),
   PRIMARY KEY(Id)
)

--inserty do event type 
insert into EventType(Id,Name) values (0,N'Jiná')
insert into EventType(Id,Name) values (1,N'Volená funkce')
insert into EventType(Id,Name) values (2,N'Soukromá pracovní')
insert into EventType(Id,Name) values (3,N'Sponzor')
insert into EventType(Id,Name) values (4,N'Osobní')
insert into EventType(Id,Name) values (6,N'Veřejná správa pracovní')
insert into EventType(Id,Name) values (7,N'Politická')
insert into EventType(Id,Name) values (9,N'Politická pracovní')

-- sem přijdou inserty do event subtype
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (1,1,N'Prezident',N'Prezidentka')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (2,1,N'Poslanec',N'Poslankyně')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (3,1,N'Senátor',N'Senátorka')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (4,1,N'člen krajského zastupitelstva',N'členka krajského zastupitelstva')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (5,1,N'člen obecního zastupitelstva',N'členka obecního zastupitelstva')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (6,1,N'člen národního výboru',N'členka národního výboru')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (7,1,N'Poslanec Evropského parlamentu',N'Poslankyně Evropského parlamentu')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (8,9,N'Předseda Poslanecké sněmovny Parlamentu ČR',N'Předsedkyně Poslanecké sněmovny Parlamentu ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (9,9,N'Předseda Senátu Parlamentu ČR',N'Předsedkyně Senátu Parlamentu ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (10,9,N'Mistopředseda Poslanecké sněmovny Parlamentu ČR',N'Mistopředsedkyně Poslanecké sněmovny Parlamentu ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (11,9,N'Místopředseda Senátu Parlamentu ČR',N'Místopředsedkyně Senátu Parlamentu ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (12,9,N'Předseda vlády ČR',N'Předsedkyně vlády ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (13,9,N'Místopředseda vlády ČR',N'Místopředsedkyně vlády ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (14,9,N'Ministr vlády ČR',N'Ministryně vlády ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (15,9,N'Starosta/Primátor',N'Starostka/Primátorka')


-- rozšířit starou strukturu o nové sloupce
alter table OsobaEvent
   add SubType int,
       PolitickaStrana nvarchar(max)

GO

update OsobaEvent set PolitickaStrana = AddInfo 

";
    du.RunDDLCommands(sql);

            }
        }
	}
}
