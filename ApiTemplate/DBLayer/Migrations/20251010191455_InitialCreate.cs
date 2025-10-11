using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblActionType",
                columns: table => new
                {
                    ActionTypeID = table.Column<int>(type: "int", nullable: false),
                    ActionTypeTitle = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    ActionTypeCreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ActionTypeIsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblActio__62FE4C04B5ED9D4D", x => x.ActionTypeID);
                });

            migrationBuilder.CreateTable(
                name: "tblBranch",
                columns: table => new
                {
                    BranchID = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    BranchRefNo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    BranchTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblBranc__A1682FA549DC0CD4", x => x.BranchID);
                });

            migrationBuilder.CreateTable(
                name: "tblCompany",
                columns: table => new
                {
                    CompanyID = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    CompanyRefNo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    CompanyTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblCompa__2D971C4C49A740A2", x => x.CompanyID);
                });

            migrationBuilder.CreateTable(
                name: "tblCompanyUnit",
                columns: table => new
                {
                    CompanyUnitID = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    CompanyUnitRefNo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    CompanyUnitTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblCompa__8942C15160273FA8", x => x.CompanyUnitID);
                });

            migrationBuilder.CreateTable(
                name: "tblDepartment",
                columns: table => new
                {
                    DepartmentID = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    DepartmentRefNo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    DepartmentTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblDepar__B2079BCD048EDF58", x => x.DepartmentID);
                });

            migrationBuilder.CreateTable(
                name: "tblDivision",
                columns: table => new
                {
                    DivisionID = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    DivisionRefNo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    DivisionTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblDivis__20EFC6880BD734EE", x => x.DivisionID);
                });

            migrationBuilder.CreateTable(
                name: "tblInvoice",
                columns: table => new
                {
                    TranID = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    TranRefNo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    PricingListID = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    SalePrice = table.Column<long>(type: "bigint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblInvoi__F7089629B6D53D4A", x => x.TranID);
                });

            migrationBuilder.CreateTable(
                name: "tblItem",
                columns: table => new
                {
                    TranID = table.Column<int>(type: "int", nullable: false),
                    ItemRefNo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValue: ""),
                    ItemTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true, defaultValue: ""),
                    SaleRate = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    TransactionDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblItem__F7089629020F2AD7", x => x.TranID);
                });

            migrationBuilder.CreateTable(
                name: "tblOrganisation",
                columns: table => new
                {
                    OrganisationID = table.Column<long>(type: "bigint", nullable: false, defaultValue: -1L),
                    OrganisationRefNo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    OrganisationTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: ""),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblOrgan__722346BCA4BE2335", x => x.OrganisationID);
                });

            migrationBuilder.CreateTable(
                name: "tblPricingList",
                columns: table => new
                {
                    TranID = table.Column<int>(type: "int", nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    PricingTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true, defaultValue: ""),
                    SaleRate = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    EffictiveFrom = table.Column<DateTime>(type: "datetime", nullable: false),
                    EffictiveTo = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblPrici__F7089629E1C238BE", x => x.TranID);
                });

            migrationBuilder.CreateTable(
                name: "tblResource",
                columns: table => new
                {
                    ResourceID = table.Column<int>(type: "int", nullable: false),
                    ResourceName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    ResourceCreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ResourceIsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblResou__4ED1814FFC50535C", x => x.ResourceID);
                });

            migrationBuilder.CreateTable(
                name: "tblRole",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    RoleTitle = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    RoleCreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    RoleIsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblRole__8AFACE3A3A043175", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "tblTokenType",
                columns: table => new
                {
                    TokenType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblTokenType", x => x.TokenType);
                });

            migrationBuilder.CreateTable(
                name: "tblUsers",
                columns: table => new
                {
                    Userid = table.Column<long>(type: "bigint", nullable: false),
                    Firstname = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Lastname = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(233)", unicode: false, maxLength: 233, nullable: false),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Salt = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblUsers__1797D02499DA0056", x => x.Userid);
                });

            migrationBuilder.CreateTable(
                name: "test1",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tname = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__test1__3213E83FE4C48E12", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tblPermission",
                columns: table => new
                {
                    PermissionID = table.Column<int>(type: "int", nullable: false),
                    ResourceID = table.Column<int>(type: "int", nullable: false),
                    ActionTypeID = table.Column<int>(type: "int", nullable: false),
                    PermissionCreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    PermissionIsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblPermi__EFA6FB0F4AD1F430", x => x.PermissionID);
                    table.ForeignKey(
                        name: "FK_Permission_Action",
                        column: x => x.ActionTypeID,
                        principalTable: "tblActionType",
                        principalColumn: "ActionTypeID");
                    table.ForeignKey(
                        name: "FK_Permission_Resource",
                        column: x => x.ResourceID,
                        principalTable: "tblResource",
                        principalColumn: "ResourceID");
                });

            migrationBuilder.CreateTable(
                name: "tblRoleHierarchy",
                columns: table => new
                {
                    ParentRoleID = table.Column<int>(type: "int", nullable: false),
                    ChildRoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleHierarchy", x => new { x.ParentRoleID, x.ChildRoleID });
                    table.ForeignKey(
                        name: "FK_RoleHierarchy_Child",
                        column: x => x.ChildRoleID,
                        principalTable: "tblRole",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "FK_RoleHierarchy_Parent",
                        column: x => x.ParentRoleID,
                        principalTable: "tblRole",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateTable(
                name: "tblResetToken",
                columns: table => new
                {
                    ResetTokenId = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    TokenType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Token = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblResetToken", x => x.ResetTokenId);
                    table.ForeignKey(
                        name: "FK_ResetToken_TokenType",
                        column: x => x.TokenType,
                        principalTable: "tblTokenType",
                        principalColumn: "TokenType");
                    table.ForeignKey(
                        name: "FK_ResetToken_User",
                        column: x => x.UserID,
                        principalTable: "tblUsers",
                        principalColumn: "Userid");
                });

            migrationBuilder.CreateTable(
                name: "tblUserRole",
                columns: table => new
                {
                    UserRoleID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    UserRoleCreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserRoleIsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblUserR__3D978A55AEDF4DB3", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK_UserRole_Role",
                        column: x => x.RoleID,
                        principalTable: "tblRole",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "FK_UserRole_User",
                        column: x => x.UserID,
                        principalTable: "tblUsers",
                        principalColumn: "Userid");
                });

            migrationBuilder.CreateTable(
                name: "tblRolePermission",
                columns: table => new
                {
                    RolePermissionID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    PermissionID = table.Column<int>(type: "int", nullable: false),
                    RolePermissionCreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    RolePermissionIsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblRoleP__120F469A84A97AB2", x => x.RolePermissionID);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission",
                        column: x => x.PermissionID,
                        principalTable: "tblPermission",
                        principalColumn: "PermissionID");
                    table.ForeignKey(
                        name: "FK_RolePermission_Role",
                        column: x => x.RoleID,
                        principalTable: "tblRole",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblPermission_ActionTypeID",
                table: "tblPermission",
                column: "ActionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_tblPermission_ResourceID",
                table: "tblPermission",
                column: "ResourceID");

            migrationBuilder.CreateIndex(
                name: "IX_tblResetToken_TokenType",
                table: "tblResetToken",
                column: "TokenType");

            migrationBuilder.CreateIndex(
                name: "IX_tblResetToken_UserID",
                table: "tblResetToken",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_tblRoleHierarchy_ChildRoleID",
                table: "tblRoleHierarchy",
                column: "ChildRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_tblRolePermission_PermissionID",
                table: "tblRolePermission",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_tblRolePermission_RoleID",
                table: "tblRolePermission",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserRole_RoleID",
                table: "tblUserRole",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserRole_UserID",
                table: "tblUserRole",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBranch");

            migrationBuilder.DropTable(
                name: "tblCompany");

            migrationBuilder.DropTable(
                name: "tblCompanyUnit");

            migrationBuilder.DropTable(
                name: "tblDepartment");

            migrationBuilder.DropTable(
                name: "tblDivision");

            migrationBuilder.DropTable(
                name: "tblInvoice");

            migrationBuilder.DropTable(
                name: "tblItem");

            migrationBuilder.DropTable(
                name: "tblOrganisation");

            migrationBuilder.DropTable(
                name: "tblPricingList");

            migrationBuilder.DropTable(
                name: "tblResetToken");

            migrationBuilder.DropTable(
                name: "tblRoleHierarchy");

            migrationBuilder.DropTable(
                name: "tblRolePermission");

            migrationBuilder.DropTable(
                name: "tblUserRole");

            migrationBuilder.DropTable(
                name: "test1");

            migrationBuilder.DropTable(
                name: "tblTokenType");

            migrationBuilder.DropTable(
                name: "tblPermission");

            migrationBuilder.DropTable(
                name: "tblRole");

            migrationBuilder.DropTable(
                name: "tblUsers");

            migrationBuilder.DropTable(
                name: "tblActionType");

            migrationBuilder.DropTable(
                name: "tblResource");
        }
    }
}
