using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blogs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Outbox",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostStatus",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blog",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blog_Person_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "dbo",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    PublishedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Blog_BlogId",
                        column: x => x.BlogId,
                        principalSchema: "dbo",
                        principalTable: "Blog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_PostStatus_PostStatusId",
                        column: x => x.PostStatusId,
                        principalSchema: "dbo",
                        principalTable: "PostStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Person_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "dbo",
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Post_PostId",
                        column: x => x.PostId,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tag_Post_PostId",
                        column: x => x.PostId,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "PostStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Draft" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Published" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Archived" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_OwnerId",
                schema: "dbo",
                table: "Blog",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Title",
                schema: "dbo",
                table: "Blog",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AuthorId",
                schema: "dbo",
                table: "Comment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                schema: "dbo",
                table: "Comment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_DeletedOn",
                schema: "dbo",
                table: "Person",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Person_EmailAddress_DeletedOn",
                schema: "dbo",
                table: "Person",
                columns: new[] { "EmailAddress", "DeletedOn" },
                unique: true,
                filter: "[DeletedOn] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Post_BlogId",
                schema: "dbo",
                table: "Post",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostStatusId",
                schema: "dbo",
                table: "Post",
                column: "PostStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Title",
                schema: "dbo",
                table: "Post",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_PostStatus_Name",
                schema: "dbo",
                table: "PostStatus",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_PostId",
                schema: "dbo",
                table: "Tag",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Outbox",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tag",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Post",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Blog",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PostStatus",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Person",
                schema: "dbo");
        }
    }
}
