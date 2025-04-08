CREATE TABLE "Form" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL,
    "Description" VARCHAR(100),
    "CreationDate" TIMESTAMP,
    "Active" BOOLEAN
);

CREATE TABLE "Module"(
    	"Id" SERIAL PRIMARY KEY,
    	"Name" VARCHAR(50) NOT NULL,
    	"Description" VARCHAR(100),
    	"CreationDate" TIMESTAMP,
    	"Active" BOOLEAN
);

CREATE TABLE "FormModule"(
	"Id" SERIAL PRIMARY KEY,
	"FormId" INTEGER NOT NULL,
	"ModuleId" INTEGER NOT NULL,
	"Active" BOOLEAN,
	FOREIGN KEY("FormId") REFERENCES "Form"("Id"),
	FOREIGN KEY("ModuleId") REFERENCES "Module"("Id")
);

CREATE TABLE "Permission"(
	"Id" SERIAL PRIMARY KEY,
    	"Name" VARCHAR(50) NOT NULL,
    	"Description" VARCHAR(100),
    	"Active" BOOLEAN
);

CREATE TABLE "Rol"(
	"Id" SERIAL PRIMARY KEY,
    	"Name" VARCHAR(50) NOT NULL,
	"Code" VARCHAR(50) NOT NULL,
    	"Description" VARCHAR(100),
    	"Active" BOOLEAN
);

CREATE TABLE "RolFormPermission"(
	"Id" SERIAL PRIMARY KEY,
    	"RolId" INTEGER NOT NULL,
	"FormId" INTEGER NOT NULL,
    	"PermissionId" INTEGER NOT NULL,
    	"Active" BOOLEAN,
	FOREIGN KEY("RolId") REFERENCES "Rol"("Id"),
	FOREIGN KEY("FormId") REFERENCES "Form"("Id"),
	FOREIGN KEY("PermissionId") REFERENCES "Permission"("Id")
);

CREATE TABLE "Person"(
	"Id" SERIAL PRIMARY KEY,
	"FirstName" VARCHAR(50) NOT NULL,
	"LastName" VARCHAR(50),
	"PhoneNumber" VARCHAR(20),
	"Email" VARCHAR(100) NOT NULL,
	"Active" BOOLEAN
);

CREATE TABLE "User"(
	"Id" SERIAL PRIMARY KEY,
	"UserName" VARCHAR(50) NOT NULL,
	"Password" VARCHAR(100) NOT NULL,
	"CreationDate" TIMESTAMP,
	"Active" BOOLEAN,
	"PersonId" INTEGER NOT NULL,
	FOREIGN KEY("PersonId") REFERENCES "Person"("Id"),
	UNIQUE("PersonId")
);

CREATE TABLE "RolUser"(
	"Id" SERIAL PRIMARY KEY,
	"RolId" INTEGER NOT NULL,
	"UserId" INTEGER NOT NULL,
	"Active" BOOLEAN,
	FOREIGN KEY("RolId") REFERENCES "Rol"("Id"),
	FOREIGN KEY("UserId") REFERENCES "User"("Id")
)