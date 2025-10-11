--Roles = Job Titles (HR, Admin, CEO, Developer).

--Permissions = Capabilities (View Employee, Add Invoice, Delete Project).

--RolePermission = Job Description (HR can View + Add Employee, but not Delete Invoice).

--UserRole = Assigning someone a Job Title (Ali is an HR, Sara is a CEO).

--Roles = Job Titles
--tblRole → (HR, Admin, CEO, Developer)

--Resources = Things/Objects we control
--tblResource → (Employee, Invoice, Project)

--ActionTypes = The verbs (basic operations)
--tblActionType → (View, Add, Delete, Edit)

--Permissions = Resource + ActionType (full capability)
--tblPermission → (View Employee, Add Employee, Delete Invoice, Edit Project)
--👉 This is where ActionType comes in — it’s part of building a permission.
--Think of tblPermission as a sentence = verb + object.
--Example: "View (ActionType) + Employee (Resource)"

--RolePermission = Job Description
--tblRolePermission → HR can (View Employee + Edit Employee), Admin can (View + Add + Delete + Edit Everything)

--UserRole = Assigning Job Title to a person
--tblUserRole → Ali is HR, Sara is CEO, John is Admin

--🔹 Why Not Just “Permission = ActionType”?

--Because an action without a resource is meaningless.

--"View" → view what?

--"Add" → add what?

--That’s why we combine ActionType + Resource to make a real permission.

--ActionType = Add + Resource = Invoice → Permission = Add Invoice.

--ActionType = Delete + Resource = Project → Permission = Delete Project.

--🔹 Flow in Plain Words

--Define Actions (verbs) in tblActionType (view, add, delete, edit).

--Define Resources (objects) in tblResource (Employee, Invoice, Project).

--Combine them into Permissions in tblPermission.

--Example: "View Employee" = (ActionTypeID=1, ResourceID=1).

--Assign Permissions to Roles in tblRolePermission.

--Example: HR gets (View Employee + Edit Employee).

--Assign Roles to Users in tblUserRole.

--Example: Ali is HR → Ali inherits all HR permissions.

--✅ So in your analogy:

--ActionType = the “verbs” (add, delete, view, edit)

--Permission = ActionType + Resource = “full sentence” (Add Employee, Delete Invoice)

--Would you like me to write you a sample SQL query that explicitly shows:

--👉 “Ali can do: View Employee, Edit Employee”

--— by joining tblUserRole → tblRolePermission → tblPermission → (tblActionType + tblResource)?

create table tblRole(
	RoleID int not null primary key,
	RoleTitle varchar(50) not null default(''),
	RoleCreatedAt datetime not null default(getdate()),
	RoleIsActive bit not null default(1)
)

create table tblActionType(
	ActionTypeID int not null primary key,
	ActionTypeTitle varchar(50) not null default(''),
	ActionTypeCreatedAt datetime not null default(getdate()),
	ActionTypeIsActive bit not null default(1)
)

create table tblResource(
    ResourceID int not null primary key,
    ResourceName varchar(100) not null,
    ResourceCreatedAt datetime not null default(getdate()),
    ResourceIsActive bit not null default(1)
);

create table tblPermission(
    PermissionID int not null primary key,
    ResourceID int not null,
    ActionTypeID int not null,
    PermissionCreatedAt datetime not null default(getdate()),
    PermissionIsActive bit not null default(1),
    constraint FK_Permission_Resource foreign key (ResourceID) references tblResource(ResourceID),
    constraint FK_Permission_Action foreign key (ActionTypeID) references tblActionType(ActionTypeID)
);

create table tblRolePermission(
    RolePermissionID int not null primary key,
    RoleID int not null,
    PermissionID int not null,
    RolePermissionCreatedAt datetime not null default(getdate()),
    RolePermissionIsActive bit not null default(1),
    constraint FK_RolePermission_Role foreign key (RoleID) references tblRole(RoleID),
    constraint FK_RolePermission_Permission foreign key (PermissionID) references tblPermission(PermissionID)
);


create table tblUserRole(
    UserRoleID int not null primary key,
    UserID bigint not null,
    RoleID int not null,
    UserRoleCreatedAt datetime not null default(getdate()),
    UserRoleIsActive bit not null default(1),
    constraint FK_UserRole_User foreign key (UserID) references tblUsers(UserID),
    constraint FK_UserRole_Role foreign key (RoleID) references tblRole(RoleID)
);


create table tblRoleHierarchy(
    ParentRoleID int not null,
    ChildRoleID int not null,
    constraint PK_RoleHierarchy primary key (ParentRoleID, ChildRoleID),
    constraint FK_RoleHierarchy_Parent foreign key (ParentRoleID) references tblRole(RoleID),
    constraint FK_RoleHierarchy_Child foreign key (ChildRoleID) references tblRole(RoleID)
);


select * from tblActionType
insert into tblActionType (ActionTypeID,ActionTypeTitle) values(1,'view'),
(2,'add'),
(3,'delete'),
(4,'edit')

select * from tblRole
insert into tblRole(RoleID,RoleTitle) values(1,'admin'),
(2,'general'),
(3,'hr'),
(4,'ceo')


insert into tblResource (ResourceID, ResourceName) values
(1, 'Employee'),
(2, 'Invoice'),
(3, 'Project');

insert into tblResource (ResourceID, ResourceName) values
(5, 'UserManagement')



select * from tblPermission

-- Employee Permissions
insert into tblPermission (PermissionID, ResourceID, ActionTypeID) values
(1, 1, 1), -- Employee + View
(2, 1, 2), -- Employee + Add
(3, 1, 3), -- Employee + Delete
(4, 1, 4); -- Employee + Edit

-- Invoice Permissions
insert into tblPermission (PermissionID, ResourceID, ActionTypeID) values
(5, 2, 1), -- Invoice + View
(6, 2, 2), -- Invoice + Add
(7, 2, 3), -- Invoice + Delete
(8, 2, 4); -- Invoice + Edit

-- Project Permissions
insert into tblPermission (PermissionID, ResourceID, ActionTypeID) values
(9, 3, 1), -- Project + View
(10, 3, 2), -- Project + Add
(11, 3, 3), -- Project + Delete
(12, 3, 4); -- Project + Edit

insert into tblPermission (PermissionID, ResourceID, ActionTypeID) values
(13, 4, 1),
(14, 4, 2),
(15, 4, 3),
(16, 4, 4);

INSERT into tblPermission (PermissionID, ResourceID, ActionTypeID) values
(17, 5, 1),
(18, 5, 2),
(19, 5, 3),
(20, 5, 4);


-- Admin gets all permissions
insert into tblRolePermission (RolePermissionID, RoleID, PermissionID) values
(1, 1, 1),(2, 1, 2),(3, 1, 3),(4, 1, 4),
(5, 1, 5),(6, 1, 6),(7, 1, 7),(8, 1, 8),
(9, 1, 9),(10, 1, 10),(11, 1, 11),(12, 1, 12);

-- General only gets view permissions
insert into tblRolePermission (RolePermissionID, RoleID, PermissionID) values
(13, 2, 1),  -- View Employee
(14, 2, 5),  -- View Invoice
(15, 2, 9);  -- View Project

-- HR can manage employees only
insert into tblRolePermission (RolePermissionID, RoleID, PermissionID) values
(16, 3, 1),(17, 3, 2),(18, 3, 4); -- View, Add, Edit Employees

-- CEO can view everything
insert into tblRolePermission (RolePermissionID, RoleID, PermissionID) values
(19, 4, 1),(20, 4, 5),(21, 4, 9); -- View Employee, Invoice, Project

insert into tblRolePermission (RolePermissionID, RoleID, PermissionID) values
(22, 1, 14);

insert into tblRolePermission (RolePermissionID, RoleID, PermissionID) values
(23, 1, 13),
(24, 1, 15),
(25, 1, 16);

INSERT into tblRolePermission (RolePermissionID, RoleID, PermissionID) values
(26, 1, 17),
(27, 1, 18),
(28, 1, 19),
(29, 1, 20);

select * from tblUsers
-- User 101 = Admin
insert into tblUserRole (UserRoleID, UserID, RoleID) values (1, 1, 1);

-- User 102 = General
insert into tblUserRole (UserRoleID, UserID, RoleID) values (2, 2, 2);

-- User 103 = HR
insert into tblUserRole (UserRoleID, UserID, RoleID) values (3, 3, 3);

-- User 104 = CEO
insert into tblUserRole (UserRoleID, UserID, RoleID) values (4, 4, 4);


insert into tblRoleHierarchy (ParentRoleID, ChildRoleID) values
(3, 4), -- HR → CEO
(2, 4); -- General → CEO

select 
    u.UserID,
    u.UserName,
    r.RoleTitle,
    res.ResourceName,
    at.ActionTypeTitle
from tblUsers u
join tblUserRole ur on u.UserID = ur.UserID
join tblRole r on ur.RoleID = r.RoleID
join tblRolePermission rp on r.RoleID = rp.RoleID
join tblPermission p on rp.PermissionID = p.PermissionID
join tblResource res on p.ResourceID = res.ResourceID
join tblActionType at on p.ActionTypeID = at.ActionTypeID
where u.UserID = 2; -- change UserID here


;with RoleCTE as (
    -- Start with direct roles of the user
    select ur.RoleID
    from tblUserRole ur
    where ur.UserID = 2 -- Example: CEO user

    union all

    -- Recursively add parent roles
    select rh.ParentRoleID
    from tblRoleHierarchy rh
    join RoleCTE rc on rh.ChildRoleID = rc.RoleID
)
select distinct 
    u.UserID,
    r.RoleTitle,
    res.ResourceName,
    at.ActionTypeTitle
from tblUsers u
join tblUserRole ur on u.UserID = ur.UserID
join RoleCTE rc on ur.RoleID = rc.RoleID
join tblRole r on rc.RoleID = r.RoleID
join tblRolePermission rp on r.RoleID = rp.RoleID
join tblPermission p on rp.PermissionID = p.PermissionID
join tblResource res on p.ResourceID = res.ResourceID
join tblActionType at on p.ActionTypeID = at.ActionTypeID
where u.UserID = 2;


select * from tblUsers