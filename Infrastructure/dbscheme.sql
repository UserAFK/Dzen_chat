
CREATE DATABASE IF NOT EXISTS comment_app;
USE comment_app;

CREATE TABLE `User` (
    `Id` CHAR(36) NOT NULL,
    `Username` VARCHAR(255) NOT NULL,
    `Email` VARCHAR(255) NOT NULL,
    `HomePage` VARCHAR(255),
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `UX_User_Username` (`Username`),
    UNIQUE INDEX `UX_User_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Comment` (
    `Id` CHAR(36) NOT NULL,
    `Content` TEXT NOT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UserId` CHAR(36) NOT NULL,
    `FileType` VARCHAR(50),
    `FileData` LONGBLOB,
    `ParentCommentId` CHAR(36),
    PRIMARY KEY (`Id`),
    INDEX `IX_Comment_UserId` (`UserId`),
    CONSTRAINT `FK_Comment_User` FOREIGN KEY (`UserId`) 
        REFERENCES `User`(`Id`) 
        ON DELETE RESTRICT,
    CONSTRAINT `FK_Comment_ParentComment` FOREIGN KEY (`ParentCommentId`) 
        REFERENCES `Comment`(`Id`) 
        ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
