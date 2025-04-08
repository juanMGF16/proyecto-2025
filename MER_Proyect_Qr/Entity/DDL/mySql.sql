CREATE database modeloqr;
use modeloqr;
CREATE TABLE `Form` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50) NOT NULL,
    `Description` VARCHAR(100),
    `CreationDate` TIMESTAMP NULL,
    `Active` BOOLEAN
);

CREATE TABLE `Module` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50) NOT NULL,
    `Description` VARCHAR(100),
    `CreationDate` TIMESTAMP NULL,
    `Active` BOOLEAN
);

CREATE TABLE `FormModule` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `FormId` BIGINT NOT NULL,
    `ModuleId` BIGINT NOT NULL,
    `Active` BOOLEAN,
    FOREIGN KEY (`FormId`) REFERENCES `Form`(`Id`),
    FOREIGN KEY (`ModuleId`) REFERENCES `Module`(`Id`)
);

CREATE TABLE `Permission` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50) NOT NULL,
    `Description` VARCHAR(100),
    `Active` BOOLEAN
);

CREATE TABLE `Rol` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50) NOT NULL,
    `Code` VARCHAR(50) NOT NULL,
    `Description` VARCHAR(100),
    `Active` BOOLEAN
);

CREATE TABLE `RolFormPermission` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `RolId` BIGINT NOT NULL,
    `FormId` BIGINT NOT NULL,
    `PermissionId` BIGINT NOT NULL,
    `Active` BOOLEAN,
    FOREIGN KEY (`RolId`) REFERENCES `Rol`(`Id`),
    FOREIGN KEY (`FormId`) REFERENCES `Form`(`Id`),
    FOREIGN KEY (`PermissionId`) REFERENCES `Permission`(`Id`)
);

CREATE TABLE `Person` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `FirstName` VARCHAR(50) NOT NULL,
    `LastName` VARCHAR(50),
    `PhoneNumber` VARCHAR(20),
    `Email` VARCHAR(100) NOT NULL,
    `Active` BOOLEAN
);

CREATE TABLE `User` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `UserName` VARCHAR(50) NOT NULL,
    `Password` VARCHAR(100) NOT NULL,
    `CreationDate` TIMESTAMP NULL,
    `Active` BOOLEAN,
    `PersonId` BIGINT NOT NULL,
    FOREIGN KEY (`PersonId`) REFERENCES `Person`(`Id`),
    UNIQUE (`PersonId`)
);

CREATE TABLE `RolUser` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `RolId` BIGINT NOT NULL,
    `UserId` BIGINT NOT NULL,
    `Active` BOOLEAN,
    FOREIGN KEY (`RolId`) REFERENCES `Rol`(`Id`),
    FOREIGN KEY (`UserId`) REFERENCES `User`(`Id`)
);
