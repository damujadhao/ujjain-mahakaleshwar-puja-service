-- Create User table
CREATE TABLE dbo.[User] (
    UserId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(320) NOT NULL UNIQUE,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL
);

-- Create indexes
CREATE UNIQUE INDEX IX_User_Username ON dbo.[User](Username);
CREATE UNIQUE INDEX IX_User_Email ON dbo.[User](Email);
CREATE INDEX IX_User_Role ON dbo.[User](Role);
CREATE INDEX IX_User_IsActive ON dbo.[User](IsActive);

-- Insert sample admin user (password: Admin@123)
INSERT INTO dbo.[User] (Username, PasswordHash, Email, Role, IsActive)
VALUES 
('admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'admin@poojapathbooking.com', 'Admin', 1);

-- Insert sample manager user (password: Manager@123)
INSERT INTO dbo.[User] (Username, PasswordHash, Email, Role, IsActive)
VALUES 
('manager', 'sQnzu7wkTrgkQZF+0G1hi5AI3Qmzvv0bXgc5THBqi7M=', 'manager@poojapathbooking.com', 'Manager', 1);

-- Verify
SELECT * FROM dbo.[User];
