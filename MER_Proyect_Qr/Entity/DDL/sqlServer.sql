CREATE TABLE [Form] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(100),
    [CreationDate] DATETIME,
    [Active] BIT
);

CREATE TABLE [Module] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(100),
    [CreationDate] DATETIME,
    [Active] BIT
);

CREATE TABLE [FormModule] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FormId] INT NOT NULL,
    [ModuleId] INT NOT NULL,
    [Active] BIT,
    FOREIGN KEY ([FormId]) REFERENCES [Form]([Id]),
    FOREIGN KEY ([ModuleId]) REFERENCES [Module]([Id])
);

CREATE TABLE [Permission] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(100),
    [Active] BIT
);

CREATE TABLE [Rol] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(100),
    [Active] BIT
);

CREATE TABLE [RolFormPermission] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [RolId] INT NOT NULL,
    [FormId] INT NOT NULL,
    [PermissionId] INT NOT NULL,
    [Active] BIT,
    FOREIGN KEY ([RolId]) REFERENCES [Rol]([Id]),
    FOREIGN KEY ([FormId]) REFERENCES [Form]([Id]),
    FOREIGN KEY ([PermissionId]) REFERENCES [Permission]([Id])
);

CREATE TABLE [Person] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50),
    [PhoneNumber] NVARCHAR(20),
    [Email] NVARCHAR(100) NOT NULL,
    [Active] BIT
);

CREATE TABLE [User] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserName] NVARCHAR(50) NOT NULL,
    [Password] NVARCHAR(100) NOT NULL,
    [CreationDate] DATETIME,
    [Active] BIT,
    [PersonId] INT NOT NULL,
    FOREIGN KEY ([PersonId]) REFERENCES [Person]([Id]),
    CONSTRAINT UQ_User_PersonId UNIQUE ([PersonId])
);

CREATE TABLE [RolUser] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [RolId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Active] BIT,
    FOREIGN KEY ([RolId]) REFERENCES [Rol]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [User]([Id])
);
