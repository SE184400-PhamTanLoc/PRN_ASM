CREATE DATABASE FUMiniTikiSystem;
GO

USE FUMiniTikiSystem;
GO

-- Bảng Customers
CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL
);

-- Bảng Orders
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderAmount DECIMAL(18,2) NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    CustomerID INT NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending',
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);

-- Bảng Categories
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Picture NVARCHAR(255),
    Description NVARCHAR(255)
);

-- Bảng Products
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(MAX),
    CategoryID INT NOT NULL,
    OrderID INT NULL,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);
