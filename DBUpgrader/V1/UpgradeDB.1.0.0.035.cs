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

			[DatabaseUpgradeMethod("1.0.0.35")]
			public static void Init_1_0_0_35(IDatabaseUpgrader du)
			{

                string sql = @"
insert into AspNetRoles(id,Name) values('4503d2bd-6f6d-4c7a-9ad7-96a45bd8929e','novinar')

declare @uid nvarchar(128)
select @uid =id from AspNetUsers where Email = 'michal@michalblaha.cz'
insert into AspNetUserRoles(UserId, RoleId) values(@uid,'4503d2bd-6f6d-4c7a-9ad7-96a45bd8929e')

set @uid='';

select @uid =id from AspNetUsers where Email = 'jaromir.novak@ctu.cz'
if (len(@uid)>0)
begin
	insert into AspNetUserRoles(UserId, RoleId) values(@uid,'4503d2bd-6f6d-4c7a-9ad7-96a45bd8929e')
end


";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
