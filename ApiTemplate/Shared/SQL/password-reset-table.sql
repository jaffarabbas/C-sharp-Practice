 IF NOT EXISTS (SELECT * FROM sysobjects WHERE
     name='tblResetToken' AND xtype='U')
     BEGIN
         CREATE TABLE [dbo].[tblResetToken] (
             [ResetTokenId] int NOT NULL,
             [UserID] bigint NOT NULL,
             [TokenType] varchar(50) NOT NULL,
             [Token] varchar(255) NOT NULL,
             [ExpiresAt] datetime NOT NULL,
             [CreatedAt] datetime NOT NULL DEFAULT (getdate()),      
             [IsUsed] bit NOT NULL DEFAULT 0,
             PRIMARY KEY ([ResetTokenId])
         );
     END

CREATE TABLE [dbo].[tblTokenType] (
    [TokenType] varchar(50) NOT NULL,
    [Description] varchar(255) NULL,
    PRIMARY KEY ([TokenType])
);