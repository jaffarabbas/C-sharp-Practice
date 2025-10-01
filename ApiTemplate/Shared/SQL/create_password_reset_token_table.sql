-- Create the password reset token table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblPasswordResetToken' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[tblResetToken] (
        [ResetTokenId] int NOT NULL,
        [UserID] bigint NOT NULL,
        [Token] varchar(255) NOT NULL,
        [ExpiresAt] datetime NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (getdate()),
        [IsUsed] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK__tblPassw__ResetTokenId] PRIMARY KEY ([ResetTokenId]),
        CONSTRAINT [FK_PasswordResetToken_User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[tblUsers] ([Userid])
    );

    PRINT 'Password reset token table created successfully.';
END
ELSE
BEGIN
    PRINT 'Password reset token table already exists.';
END