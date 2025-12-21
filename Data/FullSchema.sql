-- =============================================
-- Database Initialization
-- =============================================
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'CosmaticsDb')
BEGIN
    CREATE DATABASE CosmaticsDb;
END
GO

USE CosmaticsDb;
GO

-- =============================================
-- Core Identity Tables
-- =============================================

-- Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(MAX) NOT NULL,
        Email NVARCHAR(MAX) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        Role NVARCHAR(MAX) NOT NULL DEFAULT 'Customer',
        ProfilePhotoUrl NVARCHAR(MAX) NULL,
        CountryCode NVARCHAR(10) NOT NULL DEFAULT N'',
        PhoneNumber NVARCHAR(20) NOT NULL DEFAULT N'',
        IsVerified BIT NOT NULL DEFAULT 0
    );
END
GO

-- Update Users Table with additional columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'ProfilePhotoUrl')
BEGIN
    ALTER TABLE [Users] ADD [ProfilePhotoUrl] NVARCHAR(MAX) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'CountryCode')
BEGIN
    ALTER TABLE [Users] ADD [CountryCode] NVARCHAR(10) NOT NULL DEFAULT N'';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'PhoneNumber')
BEGIN
    ALTER TABLE [Users] ADD [PhoneNumber] NVARCHAR(20) NOT NULL DEFAULT N'';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'IsVerified')
BEGIN
    ALTER TABLE [Users] ADD [IsVerified] BIT NOT NULL DEFAULT 0;
END
GO

-- CountryCodes Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CountryCodes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [CountryCodes](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Code] [nvarchar](10) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
     CONSTRAINT [PK_CountryCodes] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )
    )
END
GO

-- Default Country Codes
IF NOT EXISTS (SELECT * FROM [CountryCodes] WHERE [Code] = '+20')
BEGIN
    INSERT INTO [CountryCodes] ([Code], [Name]) VALUES ('+20', 'Egypt');
    INSERT INTO [CountryCodes] ([Code], [Name]) VALUES ('+966', 'Saudi Arabia');
    INSERT INTO [CountryCodes] ([Code], [Name]) VALUES ('+971', 'UAE');
END
GO


-- =============================================
-- E-Commerce Tables
-- =============================================

-- Categories Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE [Categories] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(MAX) NOT NULL,
        [ImageUrl] NVARCHAR(MAX) NOT NULL DEFAULT ''
    );
END
GO

-- Products Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(MAX) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Stock INT NOT NULL,
        ImageUrl NVARCHAR(MAX) NOT NULL
    );
END
GO

-- Add CategoryId to Products
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Products]') AND name = 'CategoryId')
BEGIN
    ALTER TABLE [Products] ADD [CategoryId] INT NULL;
END
GO

-- CartItems Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CartItems')
BEGIN
    CREATE TABLE CartItems (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        CONSTRAINT FK_CartItems_Users_UserId FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_CartItems_Products_ProductId FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
    );
END
GO

-- Orders Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        OrderDate DATETIME2 NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(MAX) NOT NULL DEFAULT 'Pending',
        CONSTRAINT FK_Orders_Users_UserId FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO

-- OrderItems Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_OrderItems_Orders_OrderId FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_Products_ProductId FOREIGN KEY (ProductId) REFERENCES Products(Id)
    );
END
GO

-- =============================================
-- Content & Features Tables
-- =============================================

-- ContactMessages
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContactMessages')
BEGIN
    CREATE TABLE ContactMessages (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(MAX) NOT NULL,
        Email NVARCHAR(MAX) NOT NULL,
        Message NVARCHAR(MAX) NOT NULL,
        SentAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Applications
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Applications')
BEGIN
    CREATE TABLE Applications (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(MAX) NOT NULL,
        Email NVARCHAR(MAX) NOT NULL,
        Phone NVARCHAR(MAX) NOT NULL,
        AppliedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- FAQs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FAQs')
BEGIN
    CREATE TABLE FAQs (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Question NVARCHAR(MAX) NOT NULL,
        Answer NVARCHAR(MAX) NOT NULL
    );
END
GO

-- Services
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Services')
BEGIN
    CREATE TABLE Services (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(MAX) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        ImageUrl NVARCHAR(MAX) NULL
    );
END
GO

-- Notifications
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notifications' and xtype='U')
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [IsRead] bit NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
    );
END;
GO

-- WebsiteContents
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WebsiteContents' and xtype='U')
BEGIN
    CREATE TABLE [WebsiteContents] (
        [Id] int NOT NULL IDENTITY,
        [Section] nvarchar(max) NOT NULL,
        [Key] nvarchar(max) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        [Type] nvarchar(max) NULL DEFAULT N'text',
        CONSTRAINT [PK_WebsiteContents] PRIMARY KEY ([Id])
    );
END;
GO

-- Add Language to WebsiteContents
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[WebsiteContents]') AND name = 'Language')
BEGIN
    ALTER TABLE [WebsiteContents] ADD [Language] nvarchar(max) NOT NULL DEFAULT N'en';
END;
GO

-- SuccessStories
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SuccessStories')
BEGIN
    CREATE TABLE SuccessStories (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PersonName NVARCHAR(MAX) NOT NULL,
        Role NVARCHAR(MAX) NOT NULL,
        Quote NVARCHAR(MAX) NOT NULL,
        ImageUrl NVARCHAR(MAX) NULL
    );
END
GO

-- QuickTips
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'QuickTips')
BEGIN
    CREATE TABLE QuickTips (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(MAX) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Icon NVARCHAR(MAX) NULL
    );
END
GO

-- Projects
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Projects' AND xtype='U')
CREATE TABLE Projects (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    ImageUrl NVARCHAR(MAX) NULL,
    FigmaUrl NVARCHAR(MAX) NULL,
    ApiCollectionUrl NVARCHAR(MAX) NULL
);
GO

-- ProjectVideos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProjectVideos' AND xtype='U')
CREATE TABLE ProjectVideos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VideoUrl NVARCHAR(MAX) NOT NULL,
    ProjectId INT NOT NULL,
    CONSTRAINT FK_ProjectVideos_Projects_ProjectId FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- Default Data / Seeding
-- =============================================

-- Create Default Admin (Password: 12345)
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Email, PasswordHash, Role, IsVerified, PhoneNumber, CountryCode)
    VALUES (
        'admin', 
        'admin@cosmatics.com', 
        'sU6CuIma8bsu1gSCwwrPl6W31ZQ7wEsN2mlNomcSArly5TAuMVOrYoe7r+/vseE8bcHsmNl0VbtMZAxJUznmJg==:YIwbcWt4mTQ+fYbWt6GRTh3g7SbfX54rslUffiWqYEaV1OB+0xiky2JUgCblXxmbx2qQVR/V5qJUagR+LEdb73lIqh1M2C5irpRqjTGDew/JCht4Q4tubrnDP78AS/Lg3m8ZdmjqVIDckIBVKKqcX0RKionRwd8ypD2AgHaNasY=', 
        'Admin', 
        1, 
        '0000000000', 
        '+20'
    );
END
GO
