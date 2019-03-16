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

			[DatabaseUpgradeMethod("1.0.0.33")]
			public static void Init_1_0_0_33(IDatabaseUpgrader du)
			{

                string sql = @"
/****** Object:  Table [dbo].[Invoices]    Script Date: 15.08.2017 21:14:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Invoices](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Customer] [nvarchar](128) NOT NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_Invoices_Created]  DEFAULT (getdate()),
	[VAT] [money] NOT NULL,
	[TotalPriceNoVat] [money] NOT NULL,
	[Text] [nvarchar](500) NOT NULL CONSTRAINT [DF_Invoices_Text]  DEFAULT (''),
	[Paid] [datetime] NULL,
	[Company] [nvarchar](100) NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Address] [nvarchar](100) NULL,
	[Address2] [nvarchar](100) NULL,
	[City] [nvarchar](100) NULL,
	[Zip] [nvarchar](10) NULL,
	[Country] [nvarchar](100) NULL,
	[VatID] [nvarchar](30) NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_Invoices_Status]  DEFAULT ((0)),
	[InvoiceNumber] [nvarchar](50) NULL,
	[CompanyID] [nvarchar](50) NULL,
 CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


/****** Object:  Table [dbo].[InvoiceItems]    Script Date: 15.08.2017 21:14:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceItems]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InvoiceItems](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Invoice] [int] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[ID_ShopItem] [int] NULL,
	[Amount] [int] NOT NULL,
	[Price] [money] NOT NULL,
	[Discount] [money] NOT NULL,
	[Expires] [datetime] NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_InvoiceItems_Created]  DEFAULT (getdate()),
	[VAT] [money] NOT NULL,
 CONSTRAINT [PK_InvoiceItems] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InvoiceItems_Invoices]') AND parent_object_id = OBJECT_ID(N'[dbo].[InvoiceItems]'))
ALTER TABLE [dbo].[InvoiceItems]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceItems_Invoices] FOREIGN KEY([ID_Invoice])
REFERENCES [dbo].[Invoices] ([ID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InvoiceItems_Invoices]') AND parent_object_id = OBJECT_ID(N'[dbo].[InvoiceItems]'))
ALTER TABLE [dbo].[InvoiceItems] CHECK CONSTRAINT [FK_InvoiceItems_Invoices]
GO





";
				du.RunDDLCommands(sql);

                //add new bank account

                

			}

	


		}

	}
}
