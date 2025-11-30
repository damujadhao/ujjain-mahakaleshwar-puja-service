-- Create Users with Different Roles

-- Admin User (username: admin, password: Admin@123)
INSERT INTO dbo.[User] (Username, PasswordHash, Email, Role, IsActive)
VALUES 
('admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'admin@poojapathbooking.com', 'Admin', 1);

-- Manager User (username: manager, password: Manager@123)
INSERT INTO dbo.[User] (Username, PasswordHash, Email, Role, IsActive)
VALUES 
('manager', 'sQnzu7wkTrgkQZF+0G1hi5AI3Qmzvv0bXgc5THBqi7M=', 'manager@poojapathbooking.com', 'Manager', 1);

-- Regular User (username: testuser, password: User@123)
INSERT INTO dbo.[User] (Username, PasswordHash, Email, Role, IsActive)
VALUES 
('testuser', 'GW/4mfoZ1nNwS4QE24aihU2TSXqF1PmJp3Jg8xZdqTg=', 'user@poojapathbooking.com', 'User', 1);

-- Verify users created
SELECT UserId, Username, Email, Role, IsActive, CreatedAt 
FROM dbo.[User]
ORDER BY CreatedAt DESC;
