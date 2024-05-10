
CREATE TABLE Customer (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(30) NOT NULL,
    Email VARCHAR(20) NOT NULL,
    FirstName VARCHAR(20) NOT NULL,
    LastName VARCHAR(20) NOT NULL,
    CreatedOn DATETIME NOT NULL,
    IsActive BIT NOT NULL
);
--DROP TABLE Customer ;


CREATE TABLE Supplier (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName VARCHAR(50) NOT NULL,
    CreatedOn DATETIME NOT NULL,
    IsActive BIT NOT NULL
);
--DROP TABLE Supplier;

CREATE TABLE Product (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductName VARCHAR(50) NOT NULL,
    UnitPrice DECIMAL NOT NULL,
    SupplierId INT,
    CreatedOn DATETIME NOT NULL,
    IsActive BIT NOT NULL,
    FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId)
);
--DROP TABLE Product;

CREATE TABLE [Order] (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT,
    OrderStatus TINYINT NOT NULL,
    OrderType TINYINT NOT NULL,
    OrderById INT,
    OrderedOn DATETIME,
    ShippedOn DATETIME,
    IsActive BIT NOT NULL,
	FOREIGN KEY (ProductId) REFERENCES Product(ProductId),
	FOREIGN KEY (OrderById) REFERENCES Customer(UserId)
);
--DROP TABLE [Order];

-- Inserting the Customer data
INSERT INTO Customer ( Username, Email, FirstName, LastName, CreatedOn, IsActive)
VALUES ('john_doe', 'john@example.com', 'John', 'Doe', GETDATE(), 1);

INSERT INTO Customer (Username, Email, FirstName, LastName, CreatedOn, IsActive)
VALUES ('jane_smith', 'jane@example.com', 'Jane', 'Smith', GETDATE(), 1);

INSERT INTO Customer ( Username, Email, FirstName, LastName, CreatedOn, IsActive)
VALUES ( 'mike_jones', 'mike@example.com', 'Mike', 'Jones', GETDATE(), 1);

-- Inserting the Supplier data
INSERT INTO Supplier (SupplierName, CreatedOn, IsActive)
VALUES ('supplier1', GETDATE(), 1);

INSERT INTO Supplier (SupplierName, CreatedOn, IsActive)
VALUES ('supplier2', GETDATE(), 1);

INSERT INTO Supplier (SupplierName, CreatedOn, IsActive)
VALUES ('supplier3', GETDATE(), 0);

-- Inserting the products
INSERT INTO Product (ProductName, UnitPrice, SupplierId, CreatedOn, IsActive)
VALUES 
    ('watch', 50.00, 1, GETDATE(), 1),
    ('phone', 500.00, 1, GETDATE(), 1),
    ('socks', 5.00, 2, GETDATE(), 1),
    ('gloves', 10.00, 2, GETDATE(), 1),
    ('pendrive', 20.00, 1, GETDATE(), 0),
    ('knife', 15.00, 3, GETDATE(), 1),
    ('sharpener', 8.00, 3, GETDATE(), 1);

-- Inserting the orders
INSERT INTO [Order] (ProductId, OrderStatus, OrderType, OrderById, OrderedOn, IsActive)
VALUES 
    (15, 1, 1, 1, GETDATE(), 1),
	(16, 1, 1, 1, GETDATE(), 1),
	(18, 1, 1, 2, GETDATE(), 1);

CREATE PROCEDURE GetActiveOrdersByCustomer
    @CustomerId INT
AS
BEGIN
    SELECT o.OrderId, p.UnitPrice, p.ProductName, c.Username, o.OrderedOn, o.OrderType
    FROM [Order] o
    INNER JOIN Customer c ON o.OrderById = c.UserId
    INNER JOIN Product p ON o.ProductId = p.ProductId
    WHERE o.OrderStatus = 1
    AND c.UserId = @CustomerId;
END

EXEC GetActiveOrdersByCustomer  @CustomerId = 1;

DROP PROCEDURE GetActiveOrdersByCustomer;

