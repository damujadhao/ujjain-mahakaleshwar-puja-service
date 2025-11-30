-- Add PasswordHash and IsActive columns to existing Customer table
ALTER TABLE dbo.Customer
ADD PasswordHash NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1;

-- Create unique indexes for email and contact number
CREATE UNIQUE INDEX IX_Customer_Email ON dbo.Customer(Email) WHERE Email IS NOT NULL;
CREATE UNIQUE INDEX IX_Customer_ContactNumber ON dbo.Customer(ContactNumber);
CREATE INDEX IX_Customer_IsActive ON dbo.Customer(IsActive);

-- Update existing customers with a default password (Password: Customer@123)
-- Password hash for "Customer@123" is: jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
UPDATE dbo.Customer
SET PasswordHash = 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=',
    IsActive = 1
WHERE PasswordHash IS NULL;

-- For Rahul specifically (Password: Rahul@123)
UPDATE dbo.Customer
SET PasswordHash = 'Xohimfz+tAELjrGfWZaFCq8xVF3F2ZL/7F8W9H6qD4A='
WHERE ContactNumber = '9876543210';

-- For Priya (Password: Priya@123)
UPDATE dbo.Customer
SET PasswordHash = 'sQnzu7wkTrgkQZF+0G1hi5AI3Qmzvv0bXgc5THBqi7M='
WHERE ContactNumber = '9823456789';

-- Verify the update
SELECT CustomerId, FirstName, LastName, Email, ContactNumber, 
       CASE WHEN PasswordHash IS NOT NULL THEN 'Yes' ELSE 'No' END AS HasPassword,
       IsActive, CreatedAt
FROM dbo.Customer
ORDER BY CreatedAt DESC;

-- Show sample credentials
SELECT 
    FirstName + ' ' + LastName AS FullName,
    Email,
    ContactNumber,
    'Can login with email or contact number' AS LoginNote,
    CASE ContactNumber
        WHEN '9876543210' THEN 'Rahul@123'
        WHEN '9823456789' THEN 'Priya@123'
        ELSE 'Customer@123'
    END AS Password
FROM dbo.Customer
WHERE IsActive = 1;
