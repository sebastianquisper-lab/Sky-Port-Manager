USE skyport_db;

-- Generado con dotnet ef dbcontext script, EF Core 9.0.0 y Pomelo MySQL 9.0.0.
-- Contexto: SkyPortIdentityDbContext; servidor de referencia: MySQL 8.4.13.
-- Se conserva la collation de la base definida por J02.
-- La existencia de AspNetUsers omite toda la instalacion, incluidos los indices.
-- Este instalador no repara instalaciones parciales ni actualiza tablas existentes.
DROP PROCEDURE IF EXISTS sp_instalar_identity;

DELIMITER //
CREATE PROCEDURE sp_instalar_identity()
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.tables
        WHERE table_schema = 'skyport_db'
          AND table_name = 'AspNetUsers'
    ) THEN
        ALTER DATABASE CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;


        CREATE TABLE `AspNetRoles` (
            `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
            `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
            `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
            CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
        ) CHARACTER SET=utf8mb4;


        CREATE TABLE `AspNetUsers` (
            `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
            `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
            `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
            `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
            `EmailConfirmed` tinyint(1) NOT NULL,
            `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
            `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
            `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
            `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
            `PhoneNumberConfirmed` tinyint(1) NOT NULL,
            `TwoFactorEnabled` tinyint(1) NOT NULL,
            `LockoutEnd` datetime(6) NULL,
            `LockoutEnabled` tinyint(1) NOT NULL,
            `AccessFailedCount` int NOT NULL,
            CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
        ) CHARACTER SET=utf8mb4;


        CREATE TABLE `AspNetRoleClaims` (
            `Id` int NOT NULL AUTO_INCREMENT,
            `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
            `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
            CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
            CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
        ) CHARACTER SET=utf8mb4;


        CREATE TABLE `AspNetUserClaims` (
            `Id` int NOT NULL AUTO_INCREMENT,
            `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
            `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
            CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
            CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
        ) CHARACTER SET=utf8mb4;


        CREATE TABLE `AspNetUserLogins` (
            `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
            `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
            CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
        ) CHARACTER SET=utf8mb4;


        CREATE TABLE `AspNetUserRoles` (
            `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
            CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
            CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
        ) CHARACTER SET=utf8mb4;


        CREATE TABLE `AspNetUserTokens` (
            `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
            `Value` longtext CHARACTER SET utf8mb4 NULL,
            CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
            CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
        ) CHARACTER SET=utf8mb4;


        CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);


        CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);


        CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);


        CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);


        CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);


        CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);


        CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);
    END IF;
END //
DELIMITER ;

CALL sp_instalar_identity();
DROP PROCEDURE sp_instalar_identity;

-- Datos propios del sistema de usuarios (compatible con una instalación Identity existente).
DROP PROCEDURE IF EXISTS sp_actualizar_usuarios_skyport;
DELIMITER //
CREATE PROCEDURE sp_actualizar_usuarios_skyport()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'skyport_db' AND table_name = 'AspNetUsers' AND column_name = 'NombreCompleto'
    ) THEN
        ALTER TABLE `AspNetUsers` ADD COLUMN `NombreCompleto` varchar(120) NOT NULL DEFAULT '';
    END IF;
END //
DELIMITER ;
CALL sp_actualizar_usuarios_skyport();
DROP PROCEDURE sp_actualizar_usuarios_skyport;
