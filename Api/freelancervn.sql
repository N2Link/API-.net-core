USE [master]
GO
/****** Object:  Database [FreeLancerVN]    Script Date: 4/23/2021 10:43:49 PM ******/
CREATE DATABASE [FreeLancerVN]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FreeLancerVN', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\FreeLancerVN.mdf' , SIZE = 3264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'FreeLancerVN_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\FreeLancerVN_log.ldf' , SIZE = 816KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [FreeLancerVN] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FreeLancerVN].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FreeLancerVN] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FreeLancerVN] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FreeLancerVN] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FreeLancerVN] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FreeLancerVN] SET ARITHABORT OFF 
GO
ALTER DATABASE [FreeLancerVN] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FreeLancerVN] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FreeLancerVN] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FreeLancerVN] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FreeLancerVN] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FreeLancerVN] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FreeLancerVN] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FreeLancerVN] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FreeLancerVN] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FreeLancerVN] SET  DISABLE_BROKER 
GO
ALTER DATABASE [FreeLancerVN] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FreeLancerVN] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FreeLancerVN] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FreeLancerVN] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FreeLancerVN] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FreeLancerVN] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FreeLancerVN] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FreeLancerVN] SET RECOVERY FULL 
GO
ALTER DATABASE [FreeLancerVN] SET  MULTI_USER 
GO
ALTER DATABASE [FreeLancerVN] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FreeLancerVN] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FreeLancerVN] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FreeLancerVN] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [FreeLancerVN] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'FreeLancerVN', N'ON'
GO
USE [FreeLancerVN]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Account](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[PasswordHash] [varbinary](1024) NOT NULL,
	[PasswordSalt] [varbinary](1024) NOT NULL,
	[Phone] [nvarchar](12) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[RoleID] [int] NOT NULL,
	[Tile] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[Website] [nvarchar](30) NULL,
	[Balance] [int] NOT NULL,
	[IsAccuracy] [bit] NOT NULL,
	[SpecialtyId] [int] NULL,
	[LevelID] [int] NULL,
	[OnReady] [bit] NULL,
	[FormOfWorkID] [int] NULL,
	[AvatarUrl] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CapacityProfile]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CapacityProfile](
	[FreelancerID] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Urlweb] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_CapacityProfile_1] PRIMARY KEY CLUSTERED 
(
	[FreelancerID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FormOfWork]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormOfWork](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FormOfWok] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FreelancerService]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreelancerService](
	[FreelancerID] [int] NOT NULL,
	[ServiceID] [int] NOT NULL,
 CONSTRAINT [PK_FreelancerService] PRIMARY KEY CLUSTERED 
(
	[FreelancerID] ASC,
	[ServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FreelancerSkill]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FreelancerSkill](
	[FreelancerID] [int] NOT NULL,
	[SkilID] [int] NOT NULL,
 CONSTRAINT [PK_FreelancerSkill] PRIMARY KEY CLUSTERED 
(
	[FreelancerID] ASC,
	[SkilID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Image]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Image](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](50) NOT NULL,
	[FreeLancerID] [int] NOT NULL,
	[CProfileName] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Job]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Job](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RenterID] [int] NOT NULL,
	[FreelancerID] [int] NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Details] [nvarchar](500) NOT NULL,
	[TypeID] [int] NOT NULL,
	[FormID] [int] NOT NULL,
	[WorkatID] [int] NOT NULL,
	[PayformID] [int] NOT NULL,
	[Deadline] [date] NOT NULL,
	[Floorprice] [int] NOT NULL,
	[Cellingprice] [int] NOT NULL,
	[IsPrivate] [int] NOT NULL,
	[SpecialtyID] [int] NOT NULL,
	[ServiceID] [int] NOT NULL,
 CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[JobSkill]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobSkill](
	[JobID] [int] NOT NULL,
	[SkillID] [int] NOT NULL,
 CONSTRAINT [PK_JobSkill] PRIMARY KEY CLUSTERED 
(
	[JobID] ASC,
	[SkillID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Level]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Level](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Level] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Message]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Message](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SenderID] [int] NOT NULL,
	[ReceiveID] [int] NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OfferHistory]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferHistory](
	[JobID] [int] NOT NULL,
	[FreelancerID] [int] NOT NULL,
	[OfferPrice] [int] NOT NULL,
	[ExpectedDay] [int] NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[TodoList] [nvarchar](500) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_OfferHistory] PRIMARY KEY CLUSTERED 
(
	[JobID] ASC,
	[FreelancerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Payform]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payform](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Payform] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProfileService]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfileService](
	[FreelancerID] [int] NOT NULL,
	[ServiceID] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_ProfileService] PRIMARY KEY CLUSTERED 
(
	[FreelancerID] ASC,
	[ServiceID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Rating]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rating](
	[JobID] [int] NOT NULL,
	[FreelancerID] [int] NULL,
	[Quality] [int] NULL,
	[Level] [int] NULL,
	[Price] [int] NULL,
	[Time] [int] NULL,
	[Profession] [int] NULL,
 CONSTRAINT [PK_Rating] PRIMARY KEY CLUSTERED 
(
	[JobID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Role]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Service]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Skill]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Skill](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Specialty]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Specialty](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Specialty] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SpecialtyService]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SpecialtyService](
	[SpecialtyID] [int] NOT NULL,
	[ServiceID] [int] NOT NULL,
 CONSTRAINT [PK_SpecialtyService] PRIMARY KEY CLUSTERED 
(
	[SpecialtyID] ASC,
	[ServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Todolist]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Todolist](
	[JobID] [int] NOT NULL,
	[Todo] [nvarchar](200) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Todolist] PRIMARY KEY CLUSTERED 
(
	[JobID] ASC,
	[Todo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TypeOfWork]    Script Date: 4/23/2021 10:43:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeOfWork](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TypeOfWork] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Account] ON 

INSERT [dbo].[Account] ([ID], [Name], [PasswordHash], [PasswordSalt], [Phone], [Email], [RoleID], [Tile], [Description], [Website], [Balance], [IsAccuracy], [SpecialtyId], [LevelID], [OnReady], [FormOfWorkID], [AvatarUrl]) VALUES (2, N'Nguyen Ngoc Linh', 0x4A536D4C6BBEE7E81F8439B5EA2A378FF30973777096DB0AE9A08342332FBE21180E970F776DFE5C1F78F94499EBFC75FCAFA484F22B2E0CBE7AD16B878EA7AC, 0x5C2F2EA5FB5F0CDE2C0BFFA716EDCAA45A73826652E1A72E35A02E3C6F85D87B1CCFCECB3A4F29CBBA0E0275BF3B46F65A80BC517FA88DAAF4936F2CBA98D9E5DA0EAC3997E775E1A56E94378A3882C4F3D81A65A0294A68EDEB5E38BE4F4125657B103371EF238B1CAB238A8000AEDA18E2DF574011D2E59BF368B010AA2407, N'0969789456', N'shjnna1234@gmail.com', 2, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, N'C:\Users\shjnn\source\repos\Demo\Api\wwwroot\Avatars\default.jpg')
INSERT [dbo].[Account] ([ID], [Name], [PasswordHash], [PasswordSalt], [Phone], [Email], [RoleID], [Tile], [Description], [Website], [Balance], [IsAccuracy], [SpecialtyId], [LevelID], [OnReady], [FormOfWorkID], [AvatarUrl]) VALUES (3, N'string', 0x3B7CF17AE1A224F739ABA6653E33C6CD62A22470F8807836381C06D8D797DF75FF1D5DE09EDDC78849FAC913A970EA031900B6798DBDD917C9FE8ED829B77B2A, 0xF00A4B5B2CF6A374864F441AB458391BCCB389927620B5D7537D509ED28DB18E9073D1E913EA7D03896C4CA520DA8B9995705692B211C39B603CBC7978304D973B898BC5520B11F366845C5901B14E05E6D39EDFC5E0C2A47846DBDD3CAF107F97984A34BF21D6544F5F9989E88C6B1D59CB0FE3515E8EDEC89256D7B2E9020B, N'0969762039', N'string@gmail.com', 2, N'One punch man', N'one hit one kill', N'string', 0, 0, 2, 1, NULL, 1, N'\Avatars\default.jpg')
INSERT [dbo].[Account] ([ID], [Name], [PasswordHash], [PasswordSalt], [Phone], [Email], [RoleID], [Tile], [Description], [Website], [Balance], [IsAccuracy], [SpecialtyId], [LevelID], [OnReady], [FormOfWorkID], [AvatarUrl]) VALUES (4, N'string', 0xAF61ACB8088053D0AF187A20CA63E034CBAE937B2D5789D3971595E34329CCD0029795B5E5C318CE88C8F7ACABBFE40D07B0C7558CBA993C6BD7690F8F44B2BE, 0x507FB7ECD1F8751277E72DE6AE23BD8FFCE43B9D9C44D2E1863674D1282A6A7D87BE79E59AF23879A35DB890266EFE57B9A70B7AC4FF50DB89A21E0E73DA3BD86427BC3B7D2B772FF1E889ACE382E54F7DC68321F14BA46D11328376EC6F739B551501FFC2FC204D24B6151D71E2FC6423929978314EFE56CB955FDF9A29A426, N'string', N'string123', 2, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, N'default.jpg')
SET IDENTITY_INSERT [dbo].[Account] OFF
SET IDENTITY_INSERT [dbo].[FormOfWork] ON 

INSERT [dbo].[FormOfWork] ([ID], [Name]) VALUES (1, N'Working in an Office')
INSERT [dbo].[FormOfWork] ([ID], [Name]) VALUES (2, N'Remote Work')
SET IDENTITY_INSERT [dbo].[FormOfWork] OFF
SET IDENTITY_INSERT [dbo].[Level] ON 

INSERT [dbo].[Level] ([ID], [Name]) VALUES (1, N'Junior')
INSERT [dbo].[Level] ([ID], [Name]) VALUES (2, N'Senior')
SET IDENTITY_INSERT [dbo].[Level] OFF
SET IDENTITY_INSERT [dbo].[Payform] ON 

INSERT [dbo].[Payform] ([ID], [Name]) VALUES (1, N'Pay per project')
INSERT [dbo].[Payform] ([ID], [Name]) VALUES (2, N'Pay per hour
')
INSERT [dbo].[Payform] ([ID], [Name]) VALUES (3, N'Pay per month')
SET IDENTITY_INSERT [dbo].[Payform] OFF
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([ID], [Name]) VALUES (1, N'Admin')
INSERT [dbo].[Role] ([ID], [Name]) VALUES (2, N'Freelancer')
INSERT [dbo].[Role] ([ID], [Name]) VALUES (3, N'Renter')
SET IDENTITY_INSERT [dbo].[Role] OFF
SET IDENTITY_INSERT [dbo].[Service] ON 

INSERT [dbo].[Service] ([ID], [Name]) VALUES (1, N'Convert Template to Website')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (2, N'Convert the template into a WordPress theme')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (3, N'Set up / configure the server')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (4, N'Cut HTML / CSS from PSD')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (5, N'Build a sales website')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (6, N'Enter data on the website')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (7, N'SEO website')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (8, N'Error checking')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (9, N'Mockup Design for Website')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (10, N'Optimize SEO for the website')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (11, N'Create on-demand mobile app')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (12, N'Mockup Design for Mobile app')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (13, N'Mobile UI Design')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (14, N'Create a mobile app based on available website')
INSERT [dbo].[Service] ([ID], [Name]) VALUES (15, N'Mobile game development')
SET IDENTITY_INSERT [dbo].[Service] OFF
SET IDENTITY_INSERT [dbo].[Skill] ON 

INSERT [dbo].[Skill] ([ID], [Name]) VALUES (1, N'C#')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (2, N'Java')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (3, N'Flutter ')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (4, N'Kotlin')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (5, N'2D Animation')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (6, N'3D Animation')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (7, N'A/B Testing')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (8, N'Data Driven')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (9, N'Web content')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (10, N'Web design')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (11, N'React Native')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (12, N'React.js')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (13, N'Xamarin')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (14, N'Game Design')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (15, N'C++')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (16, N'Adobe Photoshop')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (17, N'After Effect')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (18, N'AI')
INSERT [dbo].[Skill] ([ID], [Name]) VALUES (19, N'Python')
SET IDENTITY_INSERT [dbo].[Skill] OFF
SET IDENTITY_INSERT [dbo].[Specialty] ON 

INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (1, N'Web development')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (2, N'Mobile development')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (3, N'Other programming')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (4, N'Software development')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (5, N'Search Engine Optimization-SEO')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (6, N'Consulting, designing network systems')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (7, N'QA Tester')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (8, N'Project management')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (9, N'Embedded programming')
INSERT [dbo].[Specialty] ([ID], [Name]) VALUES (10, N'I - Artificial intelligence, Machine Learning')
SET IDENTITY_INSERT [dbo].[Specialty] OFF
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 1)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 2)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 3)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 4)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 5)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 6)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 7)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 8)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 9)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (1, 10)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (2, 3)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (2, 11)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (2, 12)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (2, 13)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (2, 14)
INSERT [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID]) VALUES (2, 15)
SET IDENTITY_INSERT [dbo].[TypeOfWork] ON 

INSERT [dbo].[TypeOfWork] ([ID], [Name]) VALUES (1, N'Working in an Office')
INSERT [dbo].[TypeOfWork] ([ID], [Name]) VALUES (2, N'Remote Work')
SET IDENTITY_INSERT [dbo].[TypeOfWork] OFF
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_Specialty] FOREIGN KEY([SpecialtyId])
REFERENCES [dbo].[Specialty] ([ID])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_Specialty]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_User_FormOfWork] FOREIGN KEY([FormOfWorkID])
REFERENCES [dbo].[FormOfWork] ([ID])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_User_FormOfWork]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_User_Level] FOREIGN KEY([LevelID])
REFERENCES [dbo].[Level] ([ID])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_User_Level]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Role] ([ID])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_User_Role]
GO
ALTER TABLE [dbo].[CapacityProfile]  WITH CHECK ADD  CONSTRAINT [FK_CapacityProfile_User] FOREIGN KEY([FreelancerID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[CapacityProfile] CHECK CONSTRAINT [FK_CapacityProfile_User]
GO
ALTER TABLE [dbo].[FreelancerService]  WITH CHECK ADD  CONSTRAINT [FK_FreelancerService_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Service] ([ID])
GO
ALTER TABLE [dbo].[FreelancerService] CHECK CONSTRAINT [FK_FreelancerService_Service]
GO
ALTER TABLE [dbo].[FreelancerService]  WITH CHECK ADD  CONSTRAINT [FK_FreelancerService_User] FOREIGN KEY([FreelancerID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[FreelancerService] CHECK CONSTRAINT [FK_FreelancerService_User]
GO
ALTER TABLE [dbo].[FreelancerSkill]  WITH CHECK ADD  CONSTRAINT [FK_FreelancerSkill_Skill] FOREIGN KEY([SkilID])
REFERENCES [dbo].[Skill] ([ID])
GO
ALTER TABLE [dbo].[FreelancerSkill] CHECK CONSTRAINT [FK_FreelancerSkill_Skill]
GO
ALTER TABLE [dbo].[FreelancerSkill]  WITH CHECK ADD  CONSTRAINT [FK_FreelancerSkill_User] FOREIGN KEY([FreelancerID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[FreelancerSkill] CHECK CONSTRAINT [FK_FreelancerSkill_User]
GO
ALTER TABLE [dbo].[Image]  WITH CHECK ADD  CONSTRAINT [FK_Image_CapacityProfile] FOREIGN KEY([FreeLancerID], [CProfileName])
REFERENCES [dbo].[CapacityProfile] ([FreelancerID], [Name])
GO
ALTER TABLE [dbo].[Image] CHECK CONSTRAINT [FK_Image_CapacityProfile]
GO
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_FormOfWork] FOREIGN KEY([FormID])
REFERENCES [dbo].[FormOfWork] ([ID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_FormOfWork]
GO
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_Payform] FOREIGN KEY([PayformID])
REFERENCES [dbo].[Payform] ([ID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_Payform]
GO
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_Rating] FOREIGN KEY([ID])
REFERENCES [dbo].[Rating] ([JobID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_Rating]
GO
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_SpecialtyService] FOREIGN KEY([SpecialtyID], [ServiceID])
REFERENCES [dbo].[SpecialtyService] ([SpecialtyID], [ServiceID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_SpecialtyService]
GO
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_TypeOfWork] FOREIGN KEY([TypeID])
REFERENCES [dbo].[TypeOfWork] ([ID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_TypeOfWork]
GO
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_User] FOREIGN KEY([RenterID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_User]
GO
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_User1] FOREIGN KEY([FreelancerID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_User1]
GO
ALTER TABLE [dbo].[JobSkill]  WITH CHECK ADD  CONSTRAINT [FK_JobSkill_Job] FOREIGN KEY([JobID])
REFERENCES [dbo].[Job] ([ID])
GO
ALTER TABLE [dbo].[JobSkill] CHECK CONSTRAINT [FK_JobSkill_Job]
GO
ALTER TABLE [dbo].[JobSkill]  WITH CHECK ADD  CONSTRAINT [FK_JobSkill_Skill] FOREIGN KEY([SkillID])
REFERENCES [dbo].[Skill] ([ID])
GO
ALTER TABLE [dbo].[JobSkill] CHECK CONSTRAINT [FK_JobSkill_Skill]
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Account] FOREIGN KEY([SenderID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_Account]
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Account1] FOREIGN KEY([ReceiveID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_Account1]
GO
ALTER TABLE [dbo].[OfferHistory]  WITH CHECK ADD  CONSTRAINT [FK_OfferHistory_Job] FOREIGN KEY([JobID])
REFERENCES [dbo].[Job] ([ID])
GO
ALTER TABLE [dbo].[OfferHistory] CHECK CONSTRAINT [FK_OfferHistory_Job]
GO
ALTER TABLE [dbo].[OfferHistory]  WITH CHECK ADD  CONSTRAINT [FK_OfferHistory_User] FOREIGN KEY([FreelancerID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[OfferHistory] CHECK CONSTRAINT [FK_OfferHistory_User]
GO
ALTER TABLE [dbo].[ProfileService]  WITH CHECK ADD  CONSTRAINT [FK_ProfileService_CapacityProfile] FOREIGN KEY([FreelancerID], [Name])
REFERENCES [dbo].[CapacityProfile] ([FreelancerID], [Name])
GO
ALTER TABLE [dbo].[ProfileService] CHECK CONSTRAINT [FK_ProfileService_CapacityProfile]
GO
ALTER TABLE [dbo].[ProfileService]  WITH CHECK ADD  CONSTRAINT [FK_ProfileService_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Service] ([ID])
GO
ALTER TABLE [dbo].[ProfileService] CHECK CONSTRAINT [FK_ProfileService_Service]
GO
ALTER TABLE [dbo].[Rating]  WITH CHECK ADD  CONSTRAINT [FK_Rating_User] FOREIGN KEY([FreelancerID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[Rating] CHECK CONSTRAINT [FK_Rating_User]
GO
ALTER TABLE [dbo].[SpecialtyService]  WITH CHECK ADD  CONSTRAINT [FK_SpecialtyService_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Service] ([ID])
GO
ALTER TABLE [dbo].[SpecialtyService] CHECK CONSTRAINT [FK_SpecialtyService_Service]
GO
ALTER TABLE [dbo].[SpecialtyService]  WITH CHECK ADD  CONSTRAINT [FK_SpecialtyService_Specialty] FOREIGN KEY([SpecialtyID])
REFERENCES [dbo].[Specialty] ([ID])
GO
ALTER TABLE [dbo].[SpecialtyService] CHECK CONSTRAINT [FK_SpecialtyService_Specialty]
GO
ALTER TABLE [dbo].[Todolist]  WITH CHECK ADD  CONSTRAINT [FK_Todolist_Job] FOREIGN KEY([JobID])
REFERENCES [dbo].[Job] ([ID])
GO
ALTER TABLE [dbo].[Todolist] CHECK CONSTRAINT [FK_Todolist_Job]
GO
USE [master]
GO
ALTER DATABASE [FreeLancerVN] SET  READ_WRITE 
GO
