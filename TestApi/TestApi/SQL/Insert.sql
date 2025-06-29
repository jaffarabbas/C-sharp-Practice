-- INSERT SCRIPTS FOR ALL TABLES

-- Insert data into tblCompany
INSERT INTO tblCompany (CompanyID, CompanyRefNo, CompanyTitle) VALUES
(1, 'COMP001', 'TechCorp Solutions'),
(2, 'COMP002', 'Global Industries Ltd'),
(3, 'COMP003', 'InnovateTech Systems'),
(4, 'COMP004', 'Metro Business Group'),
(5, 'COMP005', 'Summit Enterprises'),
(6, 'COMP006', 'NextGen Technologies'),
(7, 'COMP007', 'Prime Solutions Inc'),
(8, 'COMP008', 'Elite Corporate Services'),
(9, 'COMP009', 'Digital Dynamics Corp'),
(10, 'COMP010', 'Apex Business Solutions');

-- Insert data into tblDepartment
INSERT INTO tblDepartment (DepartmentID, DepartmentRefNo, DepartmentTitle) VALUES
(1, 'DEPT001', 'Human Resources'),
(2, 'DEPT002', 'Information Technology'),
(3, 'DEPT003', 'Finance & Accounting'),
(4, 'DEPT004', 'Sales & Marketing'),
(5, 'DEPT005', 'Operations Management'),
(6, 'DEPT006', 'Research & Development'),
(7, 'DEPT007', 'Customer Service'),
(8, 'DEPT008', 'Quality Assurance'),
(9, 'DEPT009', 'Legal & Compliance'),
(10, 'DEPT010', 'Strategic Planning');

-- Insert data into tblDivision
INSERT INTO tblDivision (DivisionID, DivisionRefNo, DivisionTitle) VALUES
(1, 'DIV001', 'North America Division'),
(2, 'DIV002', 'Europe Division'),
(3, 'DIV003', 'Asia Pacific Division'),
(4, 'DIV004', 'Middle East Division'),
(5, 'DIV005', 'Latin America Division'),
(6, 'DIV006', 'Corporate Services Division'),
(7, 'DIV007', 'Technology Division'),
(8, 'DIV008', 'Manufacturing Division'),
(9, 'DIV009', 'Retail Division'),
(10, 'DIV010', 'Healthcare Division');

-- Insert data into tblBranch
INSERT INTO tblBranch (BranchID, BranchRefNo, BranchTitle) VALUES
(1, 'BR001', 'Main Branch - Downtown'),
(2, 'BR002', 'North Side Branch'),
(3, 'BR003', 'South Central Branch'),
(4, 'BR004', 'East District Branch'),
(5, 'BR005', 'West Plaza Branch'),
(6, 'BR006', 'Airport Branch'),
(7, 'BR007', 'Mall Branch'),
(8, 'BR008', 'Industrial Zone Branch'),
(9, 'BR009', 'Suburban Branch'),
(10, 'BR010', 'Waterfront Branch');

-- Insert data into tblOrganisation
INSERT INTO tblOrganisation (OrganisationID, OrganisationRefNo, OrganisationTitle) VALUES
(1, 'ORG001', 'Global Holding Company'),
(2, 'ORG002', 'Regional Operations Group'),
(3, 'ORG003', 'International Consortium'),
(4, 'ORG004', 'Strategic Alliance Network'),
(5, 'ORG005', 'Corporate Headquarters'),
(6, 'ORG006', 'Subsidiary Management Group'),
(7, 'ORG007', 'Joint Venture Partners'),
(8, 'ORG008', 'Business Unit Collective'),
(9, 'ORG009', 'Enterprise Resource Center'),
(10, 'ORG010', 'Operational Excellence Hub');

-- Insert data into tblCompanyUnit
INSERT INTO tblCompanyUnit (CompanyUnitID, CompanyUnitRefNo, CompanyUnitTitle) VALUES
(1, 'UNIT001', 'Production Unit A'),
(2, 'UNIT002', 'Administration Unit'),
(3, 'UNIT003', 'Sales Unit - North'),
(4, 'UNIT004', 'Technical Support Unit'),
(5, 'UNIT005', 'Quality Control Unit'),
(6, 'UNIT006', 'Logistics Unit'),
(7, 'UNIT007', 'Training & Development Unit'),
(8, 'UNIT008', 'Marketing Unit'),
(9, 'UNIT009', 'Finance Unit'),
(10, 'UNIT010', 'Strategic Planning Unit');