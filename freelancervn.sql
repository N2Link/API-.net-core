USE [master]
GO
drop database [FreeLancerVN]
go
/****** Object:  Database [FreeLancerVN]    Script Date: 4/16/2021 9:42:51 PM ******/
CREATE DATABASE [FreeLancerVN]
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
/****** Object:  Table [dbo].[Account]    Script Date: 4/16/2021 9:42:51 PM ******/
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
	[Speccializeid] [int] NULL,
	[LevelID] [int] NULL,
	[OnReady] [bit] NULL,
	[FormOnWorkID] [int] NULL,
	[AvatarUrl] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CapacityProfile]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[FormOfWork]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[FreelancerService]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[FreelancerSkill]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Image]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Job]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[JobSkill]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Level]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Message]    Script Date: 4/16/2021 9:42:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Message](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[JobID] [int] NOT NULL,
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
/****** Object:  Table [dbo].[OfferHistory]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Payform]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[ProfileService]    Script Date: 4/16/2021 9:42:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfileService](
	[FreelancerID] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[ServiceID] [int] NOT NULL,
 CONSTRAINT [PK_ProfileService] PRIMARY KEY CLUSTERED 
(
	[FreelancerID] ASC,
	[Name] ASC,
	[ServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Rating]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Role]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Service]    Script Date: 4/16/2021 9:42:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[SpecialtyID] [int] NULL,
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Skill]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Specialty]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[Todolist]    Script Date: 4/16/2021 9:42:51 PM ******/
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
/****** Object:  Table [dbo].[TypeOfWork]    Script Date: 4/16/2021 9:42:51 PM ******/
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
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_User_FormOfWork] FOREIGN KEY([FormOnWorkID])
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
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Service] ([ID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_Service]
GO
ALTER TABLE [dbo].[Job]  WITH CHECK ADD  CONSTRAINT [FK_Job_Specialty] FOREIGN KEY([SpecialtyID])
REFERENCES [dbo].[Specialty] ([ID])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_Specialty]
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
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Job] FOREIGN KEY([JobID])
REFERENCES [dbo].[Job] ([ID])
GO
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_Job]
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
ALTER TABLE [dbo].[Service]  WITH CHECK ADD  CONSTRAINT [FK_Service_Specialty] FOREIGN KEY([SpecialtyID])
REFERENCES [dbo].[Specialty] ([ID])
GO
ALTER TABLE [dbo].[Service] CHECK CONSTRAINT [FK_Service_Specialty]
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
