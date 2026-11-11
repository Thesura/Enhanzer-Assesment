CREATE TABLE dbo.Locations
(
    Location_Code nvarchar(100) NOT NULL,
    Location_Name nvarchar(200) NOT NULL,
    Stock_Handle int NOT NULL,
    Address nvarchar(1000) NOT NULL,
    Phone nvarchar(50) NOT NULL,
    Status int NOT NULL,
    CONSTRAINT PK_Locations PRIMARY KEY (Location_Code)
);
