USE [master]
GO
/****** Object:  Database [SellingProject]    Script Date: 07-06-2017 13:39:00 ******/
CREATE DATABASE [SellingProject]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SellingProject', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\SellingProject.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SellingProject_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\SellingProject_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SellingProject] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SellingProject].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SellingProject] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SellingProject] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SellingProject] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SellingProject] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SellingProject] SET ARITHABORT OFF 
GO
ALTER DATABASE [SellingProject] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SellingProject] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SellingProject] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SellingProject] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SellingProject] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SellingProject] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SellingProject] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SellingProject] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SellingProject] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SellingProject] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SellingProject] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SellingProject] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SellingProject] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SellingProject] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SellingProject] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SellingProject] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SellingProject] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SellingProject] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SellingProject] SET  MULTI_USER 
GO
ALTER DATABASE [SellingProject] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SellingProject] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SellingProject] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SellingProject] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [SellingProject] SET DELAYED_DURABILITY = DISABLED 
GO
USE [SellingProject]
GO
/****** Object:  UserDefinedFunction [dbo].[fnCalcDistanceKM]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fnCalcDistanceKM](@lat1 FLOAT, @lon1 FLOAT, @lat2 FLOAT, @lon2 FLOAT)
RETURNS FLOAT 
AS
BEGIN

    RETURN ACOS(SIN(PI()*@lat1/180.0)*SIN(PI()*@lat2/180.0)+COS(PI()*@lat1/180.0)*COS(PI()*@lat2/180.0)*COS(PI()*@lon2/180.0-PI()*@lon1/180.0))*6371
END
GO
/****** Object:  UserDefinedFunction [dbo].[PostTimeLeft]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
 
 
CREATE FUNCTION [dbo].[PostTimeLeft]
(
@datePost datetime
)
RETURNS nvarchar(50)
AS
BEGIN
-- Declare the return variable here
	DECLARE @DateMessage nvarchar(50)

declare @day int 
declare @hour int
set @day=(SELECT DATEDIFF(day, @datePost,getdate())) 
if(@day>1)
begin
set @DateMessage=(SELECT CONVERT(varchar, @datePost, 101) )
end
else --if lease than on day send message with hours =========
begin
set @hour =(SELECT DATEDIFF(hour, @datePost,getdate())) -- get hours
if(@hour>1) 
begin
if(@hour>23)
set @DateMessage=N' '  + '1  Days ';
else
set @DateMessage=N'  '  +  convert(varchar,@hour)  +  N'left Hours ';
end
else
begin --if lease than on hour send message with minute =========
declare @minute int
 set  @minute=(SELECT DATEDIFF(minute, @datePost,getdate())) -- get minute
if(@minute>0)
begin
if(@minute>59)
set @DateMessage=N' '  + '1 Hours ';
else
set @DateMessage=N'  '  +  convert(varchar,@minute)  +  N' left  Minutes ';

end
else
begin
declare @second int 
set @second =(SELECT DATEDIFF(second, @datePost,getdate())) -- get second
if(@second>0)
set @DateMessage=N' '  +  convert(varchar,@second)  +  N' left  Seconds ';
else
set @DateMessage=N'Now' 

end

end

end--===========================================
	RETURN @DateMessage

END



GO
/****** Object:  Table [dbo].[AdminsOperations]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminsOperations](
	[AdminsOperationTypeID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[AdminsOperationName] [varchar](30) NULL,
 CONSTRAINT [PK_AdminsOperations] PRIMARY KEY CLUSTERED 
(
	[AdminsOperationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminsTracking]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminsTracking](
	[TableID] [int] NULL,
	[RecordID] [int] NULL,
	[OldValue] [nvarchar](255) NULL,
	[NewValue] [nvarchar](255) NULL,
	[DateModify] [datetime] NULL,
	[PersonID] [int] NULL,
	[AdminsOperationTypeID] [int] NULL,
	[Notes] [nvarchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Pictures]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pictures](
	[ToolID] [bigint] NULL,
	[PicPath] [nvarchar](250) NULL,
	[PicDesc] [nvarchar](150) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tools]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tools](
	[ToolID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserID] [int] NULL,
	[ToolName] [nvarchar](150) NULL,
	[ToolDes] [nvarchar](500) NULL,
	[ToolPrice] [float] NULL,
	[State] [bit] NULL,
	[ToolTypeID] [int] NULL,
	[DateAdd] [datetime] NULL,
 CONSTRAINT [PK_Tools] PRIMARY KEY CLUSTERED 
(
	[ToolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ToolType]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ToolType](
	[ToolTypeID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ToolTypeName] [nvarchar](25) NULL,
	[lng] [char](2) NULL,
 CONSTRAINT [PK_ToolType] PRIMARY KEY CLUSTERED 
(
	[ToolTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserName] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[PhoneNumber] [varchar](15) NULL,
	[Logtit] [float] NULL,
	[Latitle] [float] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tools] ADD  CONSTRAINT [DF_Tools_State]  DEFAULT ((1)) FOR [State]
GO
ALTER TABLE [dbo].[AdminsTracking]  WITH CHECK ADD  CONSTRAINT [FK_AdminsTracking_AdminsOperations] FOREIGN KEY([AdminsOperationTypeID])
REFERENCES [dbo].[AdminsOperations] ([AdminsOperationTypeID])
GO
ALTER TABLE [dbo].[AdminsTracking] CHECK CONSTRAINT [FK_AdminsTracking_AdminsOperations]
GO
ALTER TABLE [dbo].[Tools]  WITH CHECK ADD  CONSTRAINT [FK_Tools_ToolType] FOREIGN KEY([ToolTypeID])
REFERENCES [dbo].[ToolType] ([ToolTypeID])
GO
ALTER TABLE [dbo].[Tools] CHECK CONSTRAINT [FK_Tools_ToolType]
GO
ALTER TABLE [dbo].[Tools]  WITH CHECK ADD  CONSTRAINT [FK_Tools_Users1] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tools] CHECK CONSTRAINT [FK_Tools_Users1]
GO
/****** Object:  StoredProcedure [dbo].[ToolListing]    Script Date: 07-06-2017 13:39:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/
--ToolListing 26,50000,1,10,0,0,'%'
CREATE procedure [dbo].[ToolListing](@UserID int,@Distance float,@From int ,@to int,@ToolTypeID int,@ToolID int, @q nvarchar(25) )
 as
 begin
declare @Logt float,@lat float
--get user location
SELECT   @Logt=[Logtit]
      ,@lat=[Latitle]
  FROM  [Users] where  [UserID]=@UserID
  set @From =@From -1;
  declare @NewsDate datetime
 set @NewsDate=(select [DateAdd] from Tools where ToolID=@ToolID)
 
SELECT   ROW_NUMBER() OVER(ORDER BY [ToolID] DESC) AS [Row],
 [ToolID]
      ,[ToolName]
      ,LEFT ([ToolDes], 100)  as  ToolDes 
      ,[ToolPrice]
	  ,dbo.PostTimeLeft([DateAdd]) as [DateAdd]
	  ,( SELECT  top 1 [PicPath]  FROM  [Pictures] where Pictures.[ToolID]=[Tools].[ToolID]) as PictureLink
  FROM  [Tools]
  -- only active tool in result
  where [State]=1 and 
  -- search on depratment or all
  (ToolTypeID=@ToolTypeID or @ToolTypeID=0) and
  -- query search title and details
  ( [ToolName] like @q or [ToolDes] like @q)and
  -- search for user who lives in my range
  ( UserID in(
  SELECT   [UserID]
  FROM  [Users]
  where (dbo.fnCalcDistanceKM(Latitle,Logtit,@lat,@Logt)<=@Distance --distance range
 )))
  and  (@ToolID=0 or (([DateAdd]<=@NewsDate) and(@ToolID<>0))) -- get only the previous news incuase we load more
  order by [ToolID] DESC OFFSET @From ROWS FETCH NEXT 20 ROWS ONLY
   

  
  end


GO
USE [master]
GO
ALTER DATABASE [SellingProject] SET  READ_WRITE 
GO
