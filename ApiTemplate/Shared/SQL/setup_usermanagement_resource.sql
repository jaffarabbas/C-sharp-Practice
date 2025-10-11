-- ================================================================
-- Setup Script: UserManagement Resource
-- ================================================================
-- This script creates the "UserManagement" resource and sets up
-- permissions for managing the permission system itself.
-- Only Admin role will have full access to manage user permissions.
-- ================================================================

-- Step 1: Add UserManagement resource
INSERT INTO tblResource (ResourceID, ResourceName, ResourceIsActive, ResourceCreatedAt)
VALUES (4, 'UserManagement', 1, GETDATE());

-- Step 2: Create permissions for UserManagement resource
-- Note: We're using existing ActionTypes (view, add, edit, delete)

DECLARE @ViewActionTypeID INT = (SELECT ActionTypeID FROM tblActionType WHERE ActionTypeTitle = 'view');
DECLARE @AddActionTypeID INT = (SELECT ActionTypeID FROM tblActionType WHERE ActionTypeTitle = 'add');
DECLARE @EditActionTypeID INT = (SELECT ActionTypeID FROM tblActionType WHERE ActionTypeTitle = 'edit');
DECLARE @DeleteActionTypeID INT = (SELECT ActionTypeID FROM tblActionType WHERE ActionTypeTitle = 'delete');

-- Get next PermissionID
DECLARE @NextPermissionID INT = (SELECT ISNULL(MAX(PermissionID), 0) + 1 FROM tblPermission);

-- UserManagement + View
INSERT INTO tblPermission (PermissionID, ResourceID, ActionTypeID, PermissionIsActive, PermissionCreatedAt)
VALUES (@NextPermissionID, 4, @ViewActionTypeID, 1, GETDATE());

-- UserManagement + Add
INSERT INTO tblPermission (PermissionID, ResourceID, ActionTypeID, PermissionIsActive, PermissionCreatedAt)
VALUES (@NextPermissionID + 1, 4, @AddActionTypeID, 1, GETDATE());

-- UserManagement + Edit
INSERT INTO tblPermission (PermissionID, ResourceID, ActionTypeID, PermissionIsActive, PermissionCreatedAt)
VALUES (@NextPermissionID + 2, 4, @EditActionTypeID, 1, GETDATE());

-- UserManagement + Delete
INSERT INTO tblPermission (PermissionID, ResourceID, ActionTypeID, PermissionIsActive, PermissionCreatedAt)
VALUES (@NextPermissionID + 3, 4, @DeleteActionTypeID, 1, GETDATE());

-- Step 3: Assign all UserManagement permissions to Admin role (RoleID = 1)
DECLARE @NextRolePermissionID INT = (SELECT ISNULL(MAX(RolePermissionID), 0) + 1 FROM tblRolePermission);

-- Admin can view UserManagement
INSERT INTO tblRolePermission (RolePermissionID, RoleID, PermissionID, RolePermissionIsActive, RolePermissionCreatedAt)
VALUES (@NextRolePermissionID, 1, @NextPermissionID, 1, GETDATE());

-- Admin can add UserManagement
INSERT INTO tblRolePermission (RolePermissionID, RoleID, PermissionID, RolePermissionIsActive, RolePermissionCreatedAt)
VALUES (@NextRolePermissionID + 1, 1, @NextPermissionID + 1, 1, GETDATE());

-- Admin can edit UserManagement
INSERT INTO tblRolePermission (RolePermissionID, RoleID, PermissionID, RolePermissionIsActive, RolePermissionCreatedAt)
VALUES (@NextRolePermissionID + 2, 1, @NextPermissionID + 2, 1, GETDATE());

-- Admin can delete UserManagement
INSERT INTO tblRolePermission (RolePermissionID, RoleID, PermissionID, RolePermissionIsActive, RolePermissionCreatedAt)
VALUES (@NextRolePermissionID + 3, 1, @NextPermissionID + 3, 1, GETDATE());

-- ================================================================
-- Verification Queries
-- ================================================================

-- View the UserManagement resource
SELECT * FROM tblResource WHERE ResourceName = 'UserManagement';

-- View UserManagement permissions
SELECT
    p.PermissionID,
    r.ResourceName,
    at.ActionTypeTitle
FROM tblPermission p
JOIN tblResource r ON p.ResourceID = r.ResourceID
JOIN tblActionType at ON p.ActionTypeID = at.ActionTypeID
WHERE r.ResourceName = 'UserManagement';

-- View which roles have UserManagement permissions
SELECT
    ro.RoleTitle,
    res.ResourceName,
    at.ActionTypeTitle
FROM tblRolePermission rp
JOIN tblRole ro ON rp.RoleID = ro.RoleID
JOIN tblPermission p ON rp.PermissionID = p.PermissionID
JOIN tblResource res ON p.ResourceID = res.ResourceID
JOIN tblActionType at ON p.ActionTypeID = at.ActionTypeID
WHERE res.ResourceName = 'UserManagement'
ORDER BY ro.RoleTitle, at.ActionTypeTitle;

PRINT 'UserManagement resource setup completed successfully!';
