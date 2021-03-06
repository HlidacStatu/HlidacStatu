/****** Object:  Database [Firmy]    Script Date: 10. 4. 2019 10:11:53 ******/
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Firmy')
BEGIN
CREATE DATABASE [Firmy]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FIRMY', FILENAME = N'f:\Data\SQL\FIRMY.mdf' , SIZE = 15680512KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'FIRMY_log', FILENAME = N'f:\Data\SQL\FIRMY_log.ldf' , SIZE = 5605504KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
END

GO
ALTER DATABASE [Firmy] ADD FILEGROUP [Secondary]
GO
ALTER DATABASE [Firmy] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Firmy].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Firmy] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Firmy] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Firmy] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Firmy] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Firmy] SET ARITHABORT OFF 
GO
ALTER DATABASE [Firmy] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Firmy] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Firmy] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Firmy] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Firmy] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Firmy] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Firmy] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Firmy] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Firmy] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Firmy] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Firmy] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Firmy] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Firmy] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Firmy] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Firmy] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Firmy] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Firmy] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Firmy] SET RECOVERY FULL 
GO
ALTER DATABASE [Firmy] SET  MULTI_USER 
GO
ALTER DATABASE [Firmy] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Firmy] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Firmy] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Firmy] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Firmy] SET DELAYED_DURABILITY = DISABLED 
GO
/****** Object:  User [firmy]    Script Date: 10. 4. 2019 10:11:53 ******/
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'firmy')
CREATE USER [firmy] FOR LOGIN [firmy] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [firmy]
GO
/****** Object:  UserDefinedFunction [dbo].[IsDateInInterval]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsDateInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [dbo].[IsDateInInterval]
(
	@from date,
	@to date,
	@date date
)
RETURNS bit
AS
BEGIN
	DECLARE @res bit

	if @date is null
		set @res = 0
	else if (@from <= @date and @date <= @to)
		or (@from is null and @date <= @to)
		or (@date <= @to and @to is null)
		set @res = 1
	else
		set @res = 0

	--print (@from + '' < '' + @date + '' < '' + @to)


	return @res
END

' 
END

GO
/****** Object:  UserDefinedFunction [dbo].[IsSomehowInInterval]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsSomehowInInterval]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'CREATE FUNCTION [dbo].[IsSomehowInInterval]
(
	@dateIntervalFrom date,
	@dateIntervalTo date,
	@dateRelFrom date,
	@dateRelTo date
)
RETURNS bit
AS
BEGIN
	DECLARE @res bit

	declare @oks int; set @oks = 0;

	if (@dateIntervalFrom is null AND @dateIntervalTo is null)
		return 1;
	if (@dateRelFrom is null AND @dateRelTo is null)
		return 1;

	if (@dateRelFrom is null)
		set @dateRelFrom = ''1900-01-01'';
	if (@dateRelTo is null)
		set @dateRelto = DATEADD(dy,10,GetDate());


	if (dbo.IsDateInInterval(@dateRelFrom, @dateRelTo, @dateIntervalFrom) = 1)
		set @oks = @oks +1;

	if (dbo.IsDateInInterval(@dateRelFrom, @dateRelTo, @dateIntervalTo) = 1)
		set @oks = @oks +1;

	if (@dateIntervalFrom <= @dateRelFrom and @dateRelTo <= @dateIntervalTo
		and @dateIntervalFrom is not null and  @dateRelFrom is not null and @dateRelTo is not null and @dateIntervalTo is not null
		)
		set @oks = @oks +1;

	if (@oks=0
		and @dateIntervalFrom is null 
		and @dateIntervalTo >@dateRelTo
		)
		set @oks = @oks +1;

	if (@oks=0
		and @dateIntervalTo is null 
		and @dateIntervalFrom <@dateRelFrom
		)
		set @oks = @oks +1;


	if (@oks > 0)
		set @res= 1;
	else
		set @res = 0;


	RETURN @res

END


' 
END

GO
/****** Object:  UserDefinedFunction [dbo].[RemoveAccent]    Script Date: 10. 4. 2019 10:11:54 ******/
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
/****** Object:  Table [dbo].[Angazovanost_2016]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Angazovanost_2016]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Angazovanost_2016](
	[pk] [bigint] IDENTITY(1,1) NOT NULL,
	[ICO] [nvarchar](30) NOT NULL,
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Kod_angm] [nchar](10) NULL,
	[Funkce] [nvarchar](150) NULL,
	[Titul_pred] [nvarchar](50) NULL,
	[Jmeno] [nvarchar](150) NULL,
	[Prijmeni] [nvarchar](150) NULL,
	[Titul_po] [nvarchar](50) NULL,
	[Datum_narozeni] [date] NULL,
	[Bydliste_Kod_statu] [nvarchar](10) NULL,
	[Bydliste_Nazev_statu] [nvarchar](150) NULL,
	[Bydliste_Nazev_obce] [nvarchar](150) NULL,
	[Bydliste_Ulice] [nvarchar](150) NULL,
	[Bydliste_PSC] [nvarchar](10) NULL,
	[CLD_Od] [date] NULL,
	[CLD_Do] [date] NULL,
	[Funkce_Od] [date] NULL,
	[Funkce_Do] [date] NULL
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Angazovanost_2017]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Angazovanost_2017]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Angazovanost_2017](
	[pk] [bigint] IDENTITY(1,1) NOT NULL,
	[ICO] [nvarchar](30) NOT NULL,
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Kod_angm] [nchar](10) NULL,
	[Funkce] [nvarchar](150) NULL,
	[Titul_pred] [nvarchar](50) NULL,
	[Jmeno] [nvarchar](150) NULL,
	[Prijmeni] [nvarchar](150) NULL,
	[Titul_po] [nvarchar](50) NULL,
	[Datum_narozeni] [date] NULL,
	[Bydliste_Kod_statu] [nvarchar](10) NULL,
	[Bydliste_Nazev_statu] [nvarchar](150) NULL,
	[Bydliste_Nazev_obce] [nvarchar](150) NULL,
	[Bydliste_Ulice] [nvarchar](150) NULL,
	[Bydliste_PSC] [nvarchar](10) NULL,
	[CLD_Od] [date] NULL,
	[CLD_Do] [date] NULL,
	[Funkce_Od] [date] NULL,
	[Funkce_Do] [date] NULL,
 CONSTRAINT [PK_Osoba] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[AngazovanostFirma_2016]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AngazovanostFirma_2016]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AngazovanostFirma_2016](
	[pk] [bigint] IDENTITY(1,1) NOT NULL,
	[ICO] [nvarchar](30) NOT NULL,
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Kod_angm] [nchar](10) NULL,
	[Funkce] [nvarchar](150) NULL,
	[PO_ICO] [nvarchar](30) NULL,
	[PO_IZO] [nvarchar](50) NULL,
	[Jmeno] [nvarchar](250) NULL,
	[O_forma] [nvarchar](150) NULL,
	[Bydliste_Kod_statu] [nvarchar](10) NULL,
	[Bydliste_Nazev_statu] [nvarchar](150) NULL,
	[Bydliste_Nazev_obce] [nvarchar](150) NULL,
	[Bydliste_Ulice] [nvarchar](150) NULL,
	[Bydliste_PSC] [nvarchar](10) NULL,
	[CLD_Od] [date] NULL,
	[CLD_Do] [date] NULL,
	[Funkce_Od] [date] NULL,
	[Funkce_Do] [date] NULL
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[AngazovanostFirma_2017]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AngazovanostFirma_2017]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AngazovanostFirma_2017](
	[pk] [bigint] IDENTITY(1,1) NOT NULL,
	[ICO] [nvarchar](30) NOT NULL,
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Kod_angm] [nchar](10) NULL,
	[Funkce] [nvarchar](150) NULL,
	[PO_ICO] [nvarchar](30) NULL,
	[PO_IZO] [nvarchar](50) NULL,
	[Jmeno] [nvarchar](250) NULL,
	[O_forma] [nvarchar](150) NULL,
	[Bydliste_Kod_statu] [nvarchar](10) NULL,
	[Bydliste_Nazev_statu] [nvarchar](150) NULL,
	[Bydliste_Nazev_obce] [nvarchar](150) NULL,
	[Bydliste_Ulice] [nvarchar](150) NULL,
	[Bydliste_PSC] [nvarchar](10) NULL,
	[CLD_Od] [date] NULL,
	[CLD_Do] [date] NULL,
	[Funkce_Od] [date] NULL,
	[Funkce_Do] [date] NULL,
	[podil] [decimal](18, 9) NULL,
	[created] [datetime] NULL,
	[updated] [datetime] NULL,
 CONSTRAINT [PK_angFirmy] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AspNetUserTokens](
	[Id] [nvarchar](128) NOT NULL,
	[Token] [uniqueidentifier] NOT NULL CONSTRAINT [DF_AspNetUserTokens_Token]  DEFAULT (newid()),
	[Created] [datetime] NOT NULL CONSTRAINT [DF_AspNetUserTokens_Created]  DEFAULT (getdate()),
	[LastAccess] [datetime] NULL,
	[Count] [int] NOT NULL CONSTRAINT [DF_AspNetUserTokens_Count]  DEFAULT ((0)),
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Audit]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Audit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Audit](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NOT NULL CONSTRAINT [DF_Audit_date]  DEFAULT (getdate()),
	[userId] [nvarchar](50) NOT NULL,
	[operation] [nvarchar](50) NOT NULL,
	[objectType] [nvarchar](150) NOT NULL,
	[objectId] [nvarchar](150) NOT NULL,
	[valueBefore] [nvarchar](max) NULL,
	[valueAfter] [nvarchar](max) NULL,
	[IP] [nvarchar](15) NULL,
 CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Bookmarks]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Bookmarks]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Bookmarks](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](256) NOT NULL,
	[Folder] [nvarchar](50) NOT NULL CONSTRAINT [DF_Bookmarks_Folder]  DEFAULT (''),
	[Name] [nvarchar](250) NOT NULL,
	[Url] [varchar](4000) NOT NULL,
	[ItemType] [int] NOT NULL,
	[ItemId] [nvarchar](256) NOT NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_Bookmarks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DatoveSchranky]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DatoveSchranky]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DatoveSchranky](
	[DS] [nvarchar](50) NOT NULL,
	[Identifikator] [nvarchar](50) NOT NULL,
	[DetailJson] [nvarchar](max) NULL,
	[Url] [nvarchar](100) NULL,
 CONSTRAINT [PK_DatoveSchranky] PRIMARY KEY CLUSTERED 
(
	[DS] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DatoveSchranky_bak]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DatoveSchranky_bak]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DatoveSchranky_bak](
	[DS] [nvarchar](50) NOT NULL,
	[Identifikator] [nvarchar](50) NOT NULL,
	[DetailJson] [nvarchar](max) NULL,
	[Url] [nvarchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DumpData]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DumpData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DumpData](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Processed] [datetime] NULL,
	[mesic] [int] NOT NULL,
	[rok] [int] NOT NULL,
	[hash] [nvarchar](65) NOT NULL,
	[velikost] [bigint] NOT NULL,
	[casGenerovani] [datetime] NOT NULL,
	[exception] [nvarchar](max) NULL,
 CONSTRAINT [PK_DumpData] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Esa2010]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Esa2010]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Esa2010](
	[KodPol] [int] NOT NULL,
	[Nazev] [nvarchar](150) NOT NULL,
	[Verejne] [bit] NULL,
 CONSTRAINT [PK_CZCISS] PRIMARY KEY CLUSTERED 
(
	[KodPol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Firma]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Firma]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Firma](
	[ICO] [nvarchar](30) NOT NULL,
	[DIC] [nvarchar](30) NULL,
	[Datum_zapisu_OR] [date] NULL,
	[Stav_subjektu] [tinyint] NULL,
	[Jmeno] [nvarchar](500) NULL,
	[Kod_PF] [int] NULL,
	[VersionUpdate] [int] NOT NULL CONSTRAINT [DF_Firma_VersionUpdate]  DEFAULT ((0)),
	[Esa2010] [int] NULL,
	[Source] [nvarchar](100) NULL,
	[Popis] [nvarchar](100) NULL,
	[vazbyRaw] [nvarchar](max) NULL,
	[JmenoAscii] [nvarchar](500) NULL,
	[IsInRS] [smallint] NULL,
 CONSTRAINT [PK_Firma] PRIMARY KEY CLUSTERED 
(
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Firma_DS]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Firma_DS]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Firma_DS](
	[ICO] [nvarchar](30) NOT NULL,
	[DatovaSchranka] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Firma_DS] PRIMARY KEY CLUSTERED 
(
	[ICO] ASC,
	[DatovaSchranka] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Firma_NACE]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Firma_NACE]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Firma_NACE](
	[ICO] [nvarchar](30) NOT NULL,
	[NACE] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Firma_NACE] PRIMARY KEY CLUSTERED 
(
	[ICO] ASC,
	[NACE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[FirmaEvent]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FirmaEvent]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FirmaEvent](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[ICO] [nvarchar](30) NOT NULL,
	[Title] [nvarchar](200) NULL,
	[Description] [nvarchar](max) NULL,
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Type] [int] NOT NULL,
	[AddInfo] [nvarchar](max) NULL,
	[AddInfoNum] [decimal](18, 9) NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_FirmaEvent_Created]  DEFAULT (getdate()),
	[Zdroj] [nvarchar](max) NULL,
 CONSTRAINT [PK_FirmaEvent_1] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[FirmaVazby]    Script Date: 10. 4. 2019 10:11:54 ******/
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
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Zdroj] [nvarchar](50) NULL,
	[LastUpdate] [date] NOT NULL CONSTRAINT [DF_FirmaVazby_LastUpdate]  DEFAULT (getdate()),
	[RucniZapis] [int] NULL,
	[Poznamka] [nvarchar](500) NULL,
 CONSTRAINT [PK_FirmaVazby] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[InvoiceItems]    Script Date: 10. 4. 2019 10:11:54 ******/
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
/****** Object:  Table [dbo].[Invoices]    Script Date: 10. 4. 2019 10:11:54 ******/
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
/****** Object:  Table [dbo].[ItemToOcrQueue]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemToOcrQueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ItemToOcrQueue](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[itemType] [nvarchar](100) NOT NULL,
	[itemSubType] [nvarchar](100) NULL,
	[itemId] [nvarchar](150) NOT NULL,
	[created] [datetime] NOT NULL CONSTRAINT [DF_ItemToOcrQueue_created]  DEFAULT (getdate()),
	[done] [datetime] NULL,
	[started] [datetime] NULL,
	[result] [nvarchar](max) NULL,
	[success] [int] NULL,
	[priority] [int] NULL,
 CONSTRAINT [PK_ItemToOcrQueue] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[KOD_PF]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KOD_PF]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[KOD_PF](
	[Kod] [int] NOT NULL,
	[PravniForma] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_KOD_PF] PRIMARY KEY CLUSTERED 
(
	[Kod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Media_Article]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_Article]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Media_Article](
	[ArticleId] [int] IDENTITY(1,1) NOT NULL,
	[serverId] [int] NOT NULL,
	[ArticleUrl] [nvarchar](800) NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastChecked] [datetime] NOT NULL,
 CONSTRAINT [PK_Media_Article] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Media_ArticleHistory]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_ArticleHistory]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Media_ArticleHistory](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[ArticleId] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Title] [nvarchar](800) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Published] [datetime] NULL,
	[TitleDiff] [nvarchar](max) NULL,
	[BodyDiff] [nvarchar](max) NULL,
 CONSTRAINT [PK_Media_ArticleHistory] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Media_Server]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media_Server]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Media_Server](
	[ServerId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[URL] [nvarchar](150) NOT NULL,
	[LogoUrl] [nvarchar](150) NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_Media_Server] PRIMARY KEY CLUSTERED 
(
	[ServerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[NespolehlivyPlatceDPH]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NespolehlivyPlatceDPH]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NespolehlivyPlatceDPH](
	[Ico] [nvarchar](50) NOT NULL,
	[FromDate] [date] NULL,
	[ToDate] [date] NULL,
 CONSTRAINT [PK_NespolehlivyPlatceDPH] PRIMARY KEY CLUSTERED 
(
	[Ico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(2016,1) NOT NULL,
	[ICO] [nvarchar](20) NULL,
	[DIC] [nvarchar](50) NULL,
	[Jmeno] [nvarchar](150) NULL,
	[Created] [datetime] NOT NULL,
	[PaidUntil] [date] NULL,
	[Deleted] [datetime] NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Osoba]    Script Date: 10. 4. 2019 10:11:54 ******/
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
	[TitulPred] [nvarchar](50) NULL,
	[Jmeno] [nvarchar](150) NOT NULL,
	[Prijmeni] [nvarchar](150) NOT NULL,
	[TitulPo] [nvarchar](50) NULL,
	[Pohlavi] [char](1) NULL,
	[Narozeni] [date] NULL,
	[Umrti] [date] NULL,
	[Ulice] [nvarchar](500) NULL,
	[Mesto] [nvarchar](150) NULL,
	[PSC] [nvarchar](25) NULL,
	[CountryCode] [nvarchar](5) NULL,
	[OnRadar] [bit] NOT NULL CONSTRAINT [DF_Osoba_OnRadar]  DEFAULT ((0)),
	[Status] [int] NOT NULL,
	[LastUpdate] [datetime] NOT NULL CONSTRAINT [DF_Osoba_LastUpdate]  DEFAULT (getdate()),
	[NameId] [nvarchar](50) NOT NULL,
	[PuvodniPrijmeni] [nvarchar](150) NULL,
	[JmenoAscii] [nvarchar](150) NULL,
	[PrijmeniAscii] [nvarchar](150) NULL,
	[PuvodniPrijmeniAscii] [nvarchar](150) NULL,
 CONSTRAINT [PK_Osoba_2] PRIMARY KEY CLUSTERED 
(
	[InternalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OsobaEvent]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OsobaEvent]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OsobaEvent](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[OsobaId] [int] NOT NULL,
	[Title] [nvarchar](200) NULL,
	[Description] [nvarchar](max) NULL,
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Type] [int] NOT NULL,
	[AddInfo] [nvarchar](max) NULL,
	[AddInfoNum] [decimal](18, 9) NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_OsobaEvent_Created]  DEFAULT (getdate()),
	[Zdroj] [nvarchar](max) NULL,
 CONSTRAINT [PK_OsobaEvent_1] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[OsobaExternalId]    Script Date: 10. 4. 2019 10:11:54 ******/
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
/****** Object:  Table [dbo].[OsobaVazby]    Script Date: 10. 4. 2019 10:11:54 ******/
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
	[DatumOd] [date] NULL,
	[DatumDo] [date] NULL,
	[Zdroj] [nvarchar](50) NULL,
	[LastUpdate] [date] NOT NULL CONSTRAINT [DF_Table_1_PlatneKDatum]  DEFAULT (getdate()),
	[VazbakOsobaId] [int] NULL,
	[RucniZapis] [int] NULL,
	[Poznamka] [nvarchar](500) NULL,
 CONSTRAINT [PK_OsobaVazby] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Review]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Review]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Review](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[itemType] [nvarchar](150) NOT NULL,
	[oldValue] [nvarchar](max) NULL,
	[newValue] [nvarchar](max) NOT NULL,
	[created] [datetime] NOT NULL CONSTRAINT [DF_Review_created]  DEFAULT (getdate()),
	[createdBy] [nvarchar](150) NOT NULL,
	[reviewed] [datetime] NULL,
	[reviewedBy] [nvarchar](150) NULL,
	[reviewResult] [int] NULL,
	[comment] [nvarchar](500) NULL,
 CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[SmlouvyIds]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmlouvyIds]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SmlouvyIds](
	[Id] [nvarchar](50) NOT NULL,
	[created] [datetime] NOT NULL CONSTRAINT [DF_SmlouvyIds_created]  DEFAULT (getdate()),
	[updated] [datetime] NOT NULL CONSTRAINT [DF_SmlouvyIds_updated]  DEFAULT (getdate()),
	[active] [int] NOT NULL CONSTRAINT [DF_SmlouvyIds_active]  DEFAULT ((1)),
 CONSTRAINT [PK_SmlouvyIds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[UserOptions]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOptions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserOptions](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[OptionId] [int] NOT NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_UserOptions_Created]  DEFAULT (getdate()),
	[Value] [nvarchar](max) NULL,
	[LanguageId] [int] NULL,
 CONSTRAINT [PK_UserOptions] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Visit]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Visit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Visit](
	[pk] [int] IDENTITY(1,1) NOT NULL,
	[page] [nvarchar](250) NOT NULL,
	[date] [date] NOT NULL,
	[count] [int] NOT NULL,
	[channel] [int] NOT NULL CONSTRAINT [DF_Visit_channel]  DEFAULT ((0)),
 CONSTRAINT [PK_Visit] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[WatchDog]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WatchDog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WatchDog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Expires] [datetime] NULL,
	[StatusId] [int] NOT NULL,
	[SearchTerm] [nvarchar](max) NOT NULL,
	[SearchRawQuery] [nvarchar](max) NULL,
	[RunCount] [int] NOT NULL CONSTRAINT [DF_WatchDog_RunCount]  DEFAULT ((0)),
	[SentCount] [int] NOT NULL CONSTRAINT [DF_WatchDog_SentCount]  DEFAULT ((0)),
	[PeriodId] [int] NOT NULL,
	[LastSent] [datetime] NULL,
	[LastSearched] [datetime] NULL,
	[ToEmail] [int] NOT NULL CONSTRAINT [DF_WatchDog_ToEmail]  DEFAULT ((1)),
	[ShowPublic] [int] NOT NULL CONSTRAINT [DF_WatchDog_Public]  DEFAULT ((0)),
	[Name] [nvarchar](50) NULL,
	[FocusId] [int] NOT NULL CONSTRAINT [DF_WatchDog_FocusId]  DEFAULT ((0)),
	[dataType] [nvarchar](50) NULL,
	[SpecificContact] [nvarchar](max) NULL,
	[LatestRec] [datetime] NULL,
 CONSTRAINT [PK_WatchDog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  View [dbo].[vw_VypisFlat]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_VypisFlat]'))
EXEC dbo.sp_executesql @statement = N'
CREATE VIEW [dbo].[vw_VypisFlat]
AS
SELECT        f.ICO, f.Jmeno AS JmenoFirmy, f.Datum_zapisu_OR, 
ov.PojmenovaniVazby, 
case ov.TypVazby 
when 00 then ''Podnikatel_z_OR''		
when 01 then ''Clen_statutarniho_organu''	
when 02 then ''Likvidator''		
when 03 then ''Prokurista''		
when 04 then ''Clen_dozorci_rady''		
when 05 then ''Jediny_akcionar''		
when 06 then ''Clen_druzstva_s_vkladem''	
when 07 then ''Clen_dozorci_rady_v_zastoupeni''
when 08 then ''Clen_kontrolni_komise_v_zastoupeni''
when 09 then ''Komplementar''	
when 10 then ''Komanditista''		
when 11 then ''Spravce_konkursu''		
when 12 then ''Likvidator_v_zastoupeni''	
when 13 then ''Oddeleny_insolvencni_spravce''
when 14 then ''Pobocny_spolek''		
when 15 then ''Podnikatel''		
when 16 then ''Predbezny_insolvencni_spravce''
when 17 then ''Predbezny_spravce''		
when 18 then ''Predstavenstvo''		
when 19 then ''Podilnik''
when 20 then ''Revizor''
when 21 then ''Revizor_v_zastoupeni''	
when 22 then ''Clen_rozhodci_komise''	
when 23 then ''Vedouci_odstepneho_zavodu''	
when 24 then ''Spolecnik''
when 25 then ''Clen_spravni_rady_v_zastoupeni''
when 26 then ''Clen_statutarniho_organu_zrizovatele''
when 28 then ''Clen_statutarniho_organu_v_zastoupeni''
when 29 then ''Insolvencni_spravce_vyrovnavaci''
when 31 then ''Clen_spravni_rady''		
when 32 then ''Statutarni_organ_zrizovatele_v_zastoupeni''
when 33 then ''Zakladatel''		
when 34 then ''Nastupce_zrizovatele''	
when 35 then ''Zakladatel_s_vkladem''	
when 36 then ''Clen_sdruzeni''		
when 37 then ''Zastupce_insolvencniho_spravce''
when 38 then ''Clen_kontrolni_komise''	
when 39 then ''Insolvencni_spravce''	
when 40 then ''Zastupce_spravce''		
when 41 then ''Zvlastni_insolvencni_spravce''
when 42 then ''Zvlastni_spravce''		
end AS vazba, 
ov.podil, ov.DatumOd AS vztah_osoby_k_firme_Od, ov.DatumDo AS vztah_osoby_k_firme_Do, 
                         o.Jmeno, o.Prijmeni, o.Narozeni, o.Mesto, o.CountryCode
FROM            dbo.Firma AS f INNER JOIN
                         dbo.OsobaVazby AS ov ON f.ICO = ov.VazbakICO INNER JOIN
                         dbo.Osoba AS o ON ov.OsobaID = o.InternalId

' 
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Angazovanost_ICO]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Angazovanost_2017]') AND name = N'IX_Angazovanost_ICO')
CREATE NONCLUSTERED INDEX [IX_Angazovanost_ICO] ON [dbo].[Angazovanost_2017]
(
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Angazovanost_jmeno]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Angazovanost_2017]') AND name = N'IX_Angazovanost_jmeno')
CREATE NONCLUSTERED INDEX [IX_Angazovanost_jmeno] ON [dbo].[Angazovanost_2017]
(
	[Jmeno] ASC,
	[Prijmeni] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [_dta_index_AngazovanostFirma_9_437576597__K7_K3_K4_K1_2_5_6_8_9_10_11_12_13_14_15_16_17_18_19]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AngazovanostFirma_2017]') AND name = N'_dta_index_AngazovanostFirma_9_437576597__K7_K3_K4_K1_2_5_6_8_9_10_11_12_13_14_15_16_17_18_19')
CREATE NONCLUSTERED INDEX [_dta_index_AngazovanostFirma_9_437576597__K7_K3_K4_K1_2_5_6_8_9_10_11_12_13_14_15_16_17_18_19] ON [dbo].[AngazovanostFirma_2017]
(
	[PO_ICO] ASC,
	[DatumOd] ASC,
	[DatumDo] ASC,
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AngazovanostFirma_ICO]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AngazovanostFirma_2017]') AND name = N'IX_AngazovanostFirma_ICO')
CREATE NONCLUSTERED INDEX [IX_AngazovanostFirma_ICO] ON [dbo].[AngazovanostFirma_2017]
(
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AngazovanostFirma_PO_ICO]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AngazovanostFirma_2017]') AND name = N'IX_AngazovanostFirma_PO_ICO')
CREATE NONCLUSTERED INDEX [IX_AngazovanostFirma_PO_ICO] ON [dbo].[AngazovanostFirma_2017]
(
	[PO_ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [RoleNameIndex]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND name = N'RoleNameIndex')
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]') AND name = N'IX_UserId')
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]') AND name = N'IX_UserId')
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_RoleId]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND name = N'IX_RoleId')
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND name = N'IX_UserId')
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UserNameIndex]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = N'UserNameIndex')
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_audit_date]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Audit]') AND name = N'idx_audit_date')
CREATE NONCLUSTERED INDEX [idx_audit_date] ON [dbo].[Audit]
(
	[date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_audit_objectId]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Audit]') AND name = N'idx_audit_objectId')
CREATE NONCLUSTERED INDEX [idx_audit_objectId] ON [dbo].[Audit]
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_audit_userid]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Audit]') AND name = N'idx_audit_userid')
CREATE NONCLUSTERED INDEX [idx_audit_userid] ON [dbo].[Audit]
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_DatoveSchranky_url]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DatoveSchranky]') AND name = N'IX_DatoveSchranky_url')
CREATE NONCLUSTERED INDEX [IX_DatoveSchranky_url] ON [dbo].[DatoveSchranky]
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_firma_jmenoascii]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Firma]') AND name = N'idx_firma_jmenoascii')
CREATE NONCLUSTERED INDEX [idx_firma_jmenoascii] ON [dbo].[Firma]
(
	[JmenoAscii] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Firma_Ico]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Firma]') AND name = N'IX_Firma_Ico')
CREATE NONCLUSTERED INDEX [IX_Firma_Ico] ON [dbo].[Firma]
(
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [ix_firma_jmeno]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Firma]') AND name = N'ix_firma_jmeno')
CREATE NONCLUSTERED INDEX [ix_firma_jmeno] ON [dbo].[Firma]
(
	[Jmeno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FirmaEvent_Osoba]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FirmaEvent]') AND name = N'IX_FirmaEvent_Osoba')
CREATE NONCLUSTERED INDEX [IX_FirmaEvent_Osoba] ON [dbo].[FirmaEvent]
(
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [_dta_index_FirmaVazby_9_1701581100__K2_K3_K1_4_5_6_7_8]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FirmaVazby]') AND name = N'_dta_index_FirmaVazby_9_1701581100__K2_K3_K1_4_5_6_7_8')
CREATE NONCLUSTERED INDEX [_dta_index_FirmaVazby_9_1701581100__K2_K3_K1_4_5_6_7_8] ON [dbo].[FirmaVazby]
(
	[ICO] ASC,
	[VazbakICO] ASC,
	[pk] ASC
)
INCLUDE ( 	[TypVazby],
	[PojmenovaniVazby],
	[podil],
	[DatumOd],
	[DatumDo]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [_dta_index_FirmaVazby_9_1701581100__K3_K1_K2_4_5_6_7_8]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FirmaVazby]') AND name = N'_dta_index_FirmaVazby_9_1701581100__K3_K1_K2_4_5_6_7_8')
CREATE NONCLUSTERED INDEX [_dta_index_FirmaVazby_9_1701581100__K3_K1_K2_4_5_6_7_8] ON [dbo].[FirmaVazby]
(
	[VazbakICO] ASC,
	[pk] ASC,
	[ICO] ASC
)
INCLUDE ( 	[TypVazby],
	[PojmenovaniVazby],
	[podil],
	[DatumOd],
	[DatumDo]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FirmaVazby_ICO]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FirmaVazby]') AND name = N'IX_FirmaVazby_ICO')
CREATE NONCLUSTERED INDEX [IX_FirmaVazby_ICO] ON [dbo].[FirmaVazby]
(
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FirmaVazby_Vazby]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FirmaVazby]') AND name = N'IX_FirmaVazby_Vazby')
CREATE NONCLUSTERED INDEX [IX_FirmaVazby_Vazby] ON [dbo].[FirmaVazby]
(
	[VazbakICO] ASC,
	[ICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_osoba_jmenoascii]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Osoba]') AND name = N'idx_osoba_jmenoascii')
CREATE NONCLUSTERED INDEX [idx_osoba_jmenoascii] ON [dbo].[Osoba]
(
	[JmenoAscii] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_osoba_jmenoAsciiPrijmeniAsciiNarozeni]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Osoba]') AND name = N'idx_osoba_jmenoAsciiPrijmeniAsciiNarozeni')
CREATE NONCLUSTERED INDEX [idx_osoba_jmenoAsciiPrijmeniAsciiNarozeni] ON [dbo].[Osoba]
(
	[JmenoAscii] ASC,
	[PrijmeniAscii] ASC,
	[Narozeni] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_osoba_jmenoPrijmeniNarozeni]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Osoba]') AND name = N'idx_osoba_jmenoPrijmeniNarozeni')
CREATE NONCLUSTERED INDEX [idx_osoba_jmenoPrijmeniNarozeni] ON [dbo].[Osoba]
(
	[Jmeno] ASC,
	[Prijmeni] ASC,
	[Narozeni] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_Osoba_nameId]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Osoba]') AND name = N'idx_Osoba_nameId')
CREATE NONCLUSTERED INDEX [idx_Osoba_nameId] ON [dbo].[Osoba]
(
	[NameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_osoba_prijmeniascii]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Osoba]') AND name = N'idx_osoba_prijmeniascii')
CREATE NONCLUSTERED INDEX [idx_osoba_prijmeniascii] ON [dbo].[Osoba]
(
	[PrijmeniAscii] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OsobaEvent_Osoba]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OsobaEvent]') AND name = N'IX_OsobaEvent_Osoba')
CREATE NONCLUSTERED INDEX [IX_OsobaEvent_Osoba] ON [dbo].[OsobaEvent]
(
	[OsobaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_OsobaExternalId_external]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OsobaExternalId]') AND name = N'idx_OsobaExternalId_external')
CREATE NONCLUSTERED INDEX [idx_OsobaExternalId_external] ON [dbo].[OsobaExternalId]
(
	[ExternalId] ASC,
	[ExternalSource] ASC
)
INCLUDE ( 	[OsobaId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [_dta_index_OsobaVazby_9_1877581727__K3_2]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OsobaVazby]') AND name = N'_dta_index_OsobaVazby_9_1877581727__K3_2')
CREATE NONCLUSTERED INDEX [_dta_index_OsobaVazby_9_1877581727__K3_2] ON [dbo].[OsobaVazby]
(
	[VazbakICO] ASC
)
INCLUDE ( 	[OsobaID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OsobaVazby_OsobaId]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OsobaVazby]') AND name = N'IX_OsobaVazby_OsobaId')
CREATE NONCLUSTERED INDEX [IX_OsobaVazby_OsobaId] ON [dbo].[OsobaVazby]
(
	[OsobaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_OsobaVazby_Vazby]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OsobaVazby]') AND name = N'IX_OsobaVazby_Vazby')
CREATE NONCLUSTERED INDEX [IX_OsobaVazby_Vazby] ON [dbo].[OsobaVazby]
(
	[OsobaID] ASC,
	[VazbakICO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Visit_page_date]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Visit]') AND name = N'IX_Visit_page_date')
CREATE NONCLUSTERED INDEX [IX_Visit_page_date] ON [dbo].[Visit]
(
	[page] ASC,
	[date] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_WatchDog_UserId]    Script Date: 10. 4. 2019 10:11:54 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WatchDog]') AND name = N'IX_WatchDog_UserId')
CREATE NONCLUSTERED INDEX [IX_WatchDog_UserId] ON [dbo].[WatchDog]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Article_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Article] ADD  CONSTRAINT [DF_Media_Article_Created]  DEFAULT (getdate()) FOR [Created]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Article_LastChecked]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Article] ADD  CONSTRAINT [DF_Media_Article_LastChecked]  DEFAULT (getdate()) FOR [LastChecked]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_ArticleHistory_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_ArticleHistory] ADD  CONSTRAINT [DF_Media_ArticleHistory_Created]  DEFAULT (getdate()) FOR [Created]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Media_Server_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Media_Server] ADD  CONSTRAINT [DF_Media_Server_Created]  DEFAULT (getdate()) FOR [Created]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Orders_Created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF_Orders_Created]  DEFAULT (getdate()) FOR [Created]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]'))
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]'))
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]'))
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]'))
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InvoiceItems_Invoices]') AND parent_object_id = OBJECT_ID(N'[dbo].[InvoiceItems]'))
ALTER TABLE [dbo].[InvoiceItems]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceItems_Invoices] FOREIGN KEY([ID_Invoice])
REFERENCES [dbo].[Invoices] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InvoiceItems_Invoices]') AND parent_object_id = OBJECT_ID(N'[dbo].[InvoiceItems]'))
ALTER TABLE [dbo].[InvoiceItems] CHECK CONSTRAINT [FK_InvoiceItems_Invoices]
GO
/****** Object:  StoredProcedure [dbo].[AddVisit]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddVisit]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[AddVisit] AS' 
END
GO
ALTER PROCEDURE [dbo].[AddVisit]
	-- Add the parameters for the stored procedure here
	@page nvarchar(250),
	@date date,
    @channel int = 0
AS
BEGIN

IF EXISTS(SELECT pk FROM visit
          WHERE [page]=@page and [date]=@date and channel = @channel)
    BEGIN
		update visit
		set [count] = [count]+1
		WHERE [page]=@page and [date]=@date and channel = @channel
	END
	ELSE
    BEGIN
		insert into visit([page], [date], [count], channel) values(@page, @date, 1, @channel)
	END

END
GO
/****** Object:  StoredProcedure [dbo].[Firma_IsInRS_Save]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Firma_IsInRS_Save]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[Firma_IsInRS_Save] AS' 
END
GO

ALTER PROCEDURE [dbo].[Firma_IsInRS_Save] 
	@ico nvarchar(30)

AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT ico FROM firma
          WHERE ico=@ico and IsInRs is null)
	begin
	update firma set IsInRS = 1 where ico=@ico
	end
END

GO
/****** Object:  StoredProcedure [dbo].[Firma_Save]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Firma_Save]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[Firma_Save] AS' 
END
GO
ALTER PROCEDURE [dbo].[Firma_Save]
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
    @VersionUpdate int

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
			  [VersionUpdate] = @VersionUpdate

		 WHERE [ICO] = @ico

    END
ELSE
    BEGIN
		INSERT INTO [dbo].[Firma] ([ICO],[DIC],[Datum_zapisu_OR],[Stav_subjektu],[Jmeno],[Kod_PF],[VersionUpdate], source, popis)
		VALUES (@ICO,@DIC,@Datum_zapisu_OR,@Stav_subjektu,@Jmeno,@Kod_PF,@VersionUpdate, @source, @popis)
    END


END

GO
/****** Object:  StoredProcedure [dbo].[SmlouvaId_Save]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmlouvaId_Save]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SmlouvaId_Save] AS' 
END
GO
ALTER PROCEDURE [dbo].[SmlouvaId_Save] 
	@id nvarchar(50),
    @active int,
	@created datetime = null,
	@updated datetime = null
AS
BEGIN
	SET NOCOUNT ON;
	declare @dt datetime;
	set @dt = getdate();

if (@created is null)
	set @created=@dt;
if (@updated is null)
	set @updated=@dt;

set transaction isolation level serializable
begin transaction

	IF EXISTS(SELECT id FROM SmlouvyIds with (updlock) 
			  WHERE id=@id)
	BEGIN
		update SmlouvyIds
		set updated = @updated, active = @active
		where id=@id
	END
	ELSE
	BEGIN
		insert into SmlouvyIds(id, created, updated, active) values(@id, @created,@updated,@active)
	END
commit

END
GO
/****** Object:  StoredProcedure [dbo].[UserOption_Add]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Add]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UserOption_Add] AS' 
END
GO
ALTER PROCEDURE [dbo].[UserOption_Add]
	@optionId int,
	@userId int = null,
	@value nvarchar(max) = null,
	@languageId int = null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @pk int
	select @pk = pk from [UserOptions] where optionId = @optionId and
		(@UserId is null and UserId is null or UserId = @UserId)
		and (@languageId is null and languageid is null or  languageid = @languageId)
	
	if (@pk is not null)
	begin
		update [UserOptions]
		set value = @value,  created = GETDATE(), languageid = @languageid
		where pk = @pk
		
		select @pk
	end
	else	
	begin 
		insert into [UserOptions](UserId, optionId, value, languageid)
		values (@UserId, @optionId, @value,@languageid)
		
		set @pk = SCOPE_IDENTITY() 
		select @pk
	end
END
GO
/****** Object:  StoredProcedure [dbo].[UserOption_Get]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UserOption_Get] AS' 
END
GO
ALTER PROCEDURE [dbo].[UserOption_Get]
	@optionId int,
	@UserId int = null,
	@languageId int = null	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select * from [UserOptions]
	where optionId = @optionId and 
	(@UserId is null and UserId is null or UserId = @UserId)
	and (@languageId is null and languageid is null or  languageid = @languageId)
END
GO
/****** Object:  StoredProcedure [dbo].[UserOption_Remove]    Script Date: 10. 4. 2019 10:11:54 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserOption_Remove]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UserOption_Remove] AS' 
END
GO
ALTER PROCEDURE [dbo].[UserOption_Remove]
	@optionId int,
	@UserId int = null,
	@languageId int = null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	delete from [UserOptions]
	where optionId = @optionId and
	(@UserId is null and UserId is null or UserId = @UserId)
	and (@languageId is null and languageid is null or  languageid = @languageId)
	
END
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'_DatabaseUpgrader' , NULL,NULL, NULL,NULL, NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'_DatabaseUpgrader', @value=N'1.0.0.58' 
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPaneCount' , N'SCHEMA',N'dbo', N'VIEW',N'vw_VypisFlat', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_VypisFlat'
GO
ALTER DATABASE [Firmy] SET  READ_WRITE 
GO
