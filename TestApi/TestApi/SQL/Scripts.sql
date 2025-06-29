select * from tblItem

select * from tblPricingList

select tblItem.*,tblPricingList.SaleRate from tblItem 
inner join tblPricingList on tblItem.TranID = tblPricingList.ItemID

select * from tblInvoice

create table tblInvoice
(
	TranID bigint not null default(-1) primary key,
	TranRefNo varchar(100) not null default(''),
	ItemID bigint not null default(0),
	PricingListID bigint not null default(0),
	Quantity bigint not null default(0),
	SalePrice bigint not null default(0),
	CreationDate datetime not null default(getdate()),
)


create table tblCompany
(
	CompanyID bigint not null default(-1) primary key,
	CompanyRefNo varchar(100) not null default(''),
	CompanyTitle varchar(100) not null default(''),
	CreationDate datetime not null default(getdate()),
)



-- Department Table
create table tblDepartment
(
	DepartmentID bigint not null default(-1) primary key,
	DepartmentRefNo varchar(100) not null default(''),
	DepartmentTitle varchar(100) not null default(''),
	CreationDate datetime not null default(getdate()),
)

-- Division Table
create table tblDivision
(
	DivisionID bigint not null default(-1) primary key,
	DivisionRefNo varchar(100) not null default(''),
	DivisionTitle varchar(100) not null default(''),
	CreationDate datetime not null default(getdate()),
)

-- Branch Table
create table tblBranch
(
	BranchID bigint not null default(-1) primary key,
	BranchRefNo varchar(100) not null default(''),
	BranchTitle varchar(100) not null default(''),
	CreationDate datetime not null default(getdate()),
)

-- Organisation Table
create table tblOrganisation
(
	OrganisationID bigint not null default(-1) primary key,
	OrganisationRefNo varchar(100) not null default(''),
	OrganisationTitle varchar(100) not null default(''),
	CreationDate datetime not null default(getdate()),
)

-- Company Unit Table
create table tblCompanyUnit
(
	CompanyUnitID bigint not null default(-1) primary key,
	CompanyUnitRefNo varchar(100) not null default(''),
	CompanyUnitTitle varchar(100) not null default(''),
	CreationDate datetime not null default(getdate()),
)

-- Select all records from tblCompany
SELECT CompanyID, CompanyRefNo, CompanyTitle, CreationDate FROM tblCompany;

-- Select all records from tblDepartment
SELECT DepartmentID, DepartmentRefNo, DepartmentTitle, CreationDate FROM tblDepartment;

-- Select all records from tblDivision
SELECT DivisionID, DivisionRefNo, DivisionTitle, CreationDate FROM tblDivision;

-- Select all records from tblBranch
SELECT BranchID, BranchRefNo, BranchTitle, CreationDate FROM tblBranch;

-- Select all records from tblOrganisation
SELECT OrganisationID, OrganisationRefNo, OrganisationTitle, CreationDate FROM tblOrganisation;

-- Select all records from tblCompanyUnit
SELECT CompanyUnitID, CompanyUnitRefNo, CompanyUnitTitle, CreationDate FROM tblCompanyUnit;
