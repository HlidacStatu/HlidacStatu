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
insert into EventType(Id,Name) values (0,'Jiná')
insert into EventType(Id,Name) values (1,'Volená funkce')
insert into EventType(Id,Name) values (2,'Soukromá pracovní')
insert into EventType(Id,Name) values (3,'Sponzor')
insert into EventType(Id,Name) values (4,'Osobní')
insert into EventType(Id,Name) values (6,'Veřejná správa pracovní')
insert into EventType(Id,Name) values (7,'Politická')
insert into EventType(Id,Name) values (9,'Politická pracovní')

-- sem přijdou inserty do event subtype
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (1,1,'Prezident','Prezidentka')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (2,1,'Poslanec','Poslankyně')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (3,1,'Senátor','Senátorka')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (4,1,'člen krajského zastupitelstva','členka krajského zastupitelstva')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (5,1,'člen obecního zastupitelstva','členka obecního zastupitelstva')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (6,1,'člen národního výboru','členka národního výboru')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (7,1,'Poslanec Evropského parlamentu','Poslankyně Evropského parlamentu')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (8,9,'Předseda Poslanecké sněmovny Parlamentu ČR','Předsedkyně Poslanecké sněmovny Parlamentu ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (9,9,'Předseda Senátu Parlamentu ČR','Předsedkyně Senátu Parlamentu ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (10,9,'Mistopředseda Poslanecké sněmovny Parlamentu ČR','Mistopředsedkyně Poslanecké sněmovny Parlamentu ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (11,9,'Místopředseda Senátu Parlamentu ČR','Místopředsedkyně Senátu Parlamentu ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (12,9,'Předseda vlády ČR','Předsedkyně vlády ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (13,9,'Místopředseda vlády ČR','Místopředsedkyně vlády ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (14,9,'Ministr vlády ČR','Ministryně vlády ČR')
insert into EventSubType(Id,EventTypeId,NameMale,NameFemale) values (15,9,'Starosta/Primátor','Starostka/Primátorka')

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
