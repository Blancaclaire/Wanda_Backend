


-- =============================================
-- 1. LIMPIEZA Y CREACIÓN DE LA BASE DE DATOS
-- =============================================
USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'wandaDB')
BEGIN
    ALTER DATABASE wandaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE wandaDB;
END
GO

CREATE DATABASE wandaDB;
GO

USE wandaDB;
GO

-- =============================================
-- 2. CREACIÓN DE TABLAS (ESTRUCTURA)
-- =============================================

-- TABLA DE USUARIOS
CREATE TABLE USERS (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL
);

-- TABLA DE CUENTAS
CREATE TABLE ACCOUNTS (
    account_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    account_type NVARCHAR(20) NOT NULL CHECK (account_type IN ('personal', 'joint')),
    amount DECIMAL(18, 2) DEFAULT 0.00,
    weekly_budget DECIMAL(18, 2) NULL,
    monthly_budget DECIMAL(18, 2) NULL,
    account_picture_url NVARCHAR(100) NULL,
    creation_date DATETIME DEFAULT GETDATE() 
);

-- TABLA INTERMEDIA (USUARIOS <-> CUENTAS)
CREATE TABLE ACCOUNT_USERS (
    user_id INT NOT NULL,
    account_id INT NOT NULL,
    role NVARCHAR(20) DEFAULT 'member' CHECK (role IN ('admin', 'member')),
    joined_at DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (user_id, account_id),
    -- Agregamos ON DELETE CASCADE aquí:
    CONSTRAINT FK_AccountUsers_User FOREIGN KEY (user_id) REFERENCES USERS(user_id) ON DELETE CASCADE,
    CONSTRAINT FK_AccountUsers_Account FOREIGN KEY (account_id) REFERENCES ACCOUNTS(account_id) ON DELETE CASCADE
);

-- TABLA DE OBJETIVOS DE AHORRO
CREATE TABLE OBJECTIVES (
    objective_id INT IDENTITY(1,1) PRIMARY KEY,
    account_id INT NOT NULL,
    name NVARCHAR(100) NOT NULL,
    target_amount DECIMAL(18, 2) NOT NULL,
    current_save DECIMAL(18, 2) DEFAULT 0.00,
    deadline DATE NULL,
    objective_picture_url NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Objectives_Account FOREIGN KEY (account_id) REFERENCES ACCOUNTS(account_id) ON DELETE CASCADE
);

-- TABLA DE TRANSACCIONES
CREATE TABLE TRANSACTIONS (
    transaction_id INT IDENTITY(1,1) PRIMARY KEY,
    account_id INT NOT NULL,
    user_id INT NOT NULL,
    category NVARCHAR(50) NOT NULL, 
    objective_id INT NULL, 
    amount DECIMAL(18, 2) NOT NULL,
    transaction_type NVARCHAR(20) NOT NULL CHECK (transaction_type IN ('income', 'expense', 'saving')),
    concept NVARCHAR(255) NOT NULL,
    transaction_date DATETIME DEFAULT GETDATE(),
    isRecurring BIT DEFAULT 0,
    frequency NVARCHAR(20) NULL CHECK (frequency IN ('monthly', 'weekly', 'annual')),
    end_date DATE NULL,
    split_type NVARCHAR(20) NOT NULL CHECK (split_type IN ('individual', 'contribution', 'divided')),
    
    CONSTRAINT FK_Transactions_Account FOREIGN KEY (account_id) REFERENCES ACCOUNTS(account_id) ON DELETE CASCADE,
    CONSTRAINT FK_Transactions_User FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_Transactions_Objective FOREIGN KEY (objective_id) REFERENCES OBJECTIVES(objective_id) ON DELETE NO ACTION 
);

-- TABLA DE REPARTO DE GASTOS (SPLITS)
CREATE TABLE TRANSACTION_SPLITS (
    split_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    transaction_id INT NOT NULL,
    amount_assigned DECIMAL(18, 2) NOT NULL,
    status NVARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'settled')),
    
    CONSTRAINT FK_Splits_User FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_Splits_Transaction FOREIGN KEY (transaction_id) REFERENCES TRANSACTIONS(transaction_id) ON DELETE CASCADE
);
GO

-- =============================================
-- 3. CARGA DE DATOS INICIALES (SEEDING)
-- =============================================

INSERT INTO USERS (name, email, password) VALUES ('Ana García', 'ana@wanda.com', 'hash_password_1');
INSERT INTO USERS (name, email, password) VALUES ('Juan Pérez', 'juan@wanda.com', 'hash_password_2');

INSERT INTO ACCOUNTS (name, account_type, amount, weekly_budget, monthly_budget) 
VALUES ('Ahorros Ana', 'personal', 1500.00, 100.00, 400.00); 
INSERT INTO ACCOUNTS (name, account_type, amount, weekly_budget, monthly_budget) 
VALUES ('Cuenta Juan', 'personal', 1200.00, 80.00, 350.00); 

INSERT INTO ACCOUNTS (name, account_type, amount) 
VALUES ('Casa Ana y Juan', 'joint', 500.00); 

INSERT INTO ACCOUNT_USERS (user_id, account_id, role) VALUES (1, 1, 'admin'); 
INSERT INTO ACCOUNT_USERS (user_id, account_id, role) VALUES (2, 2, 'admin'); 
INSERT INTO ACCOUNT_USERS (user_id, account_id, role) VALUES (1, 3, 'admin'); 
INSERT INTO ACCOUNT_USERS (user_id, account_id, role) VALUES (2, 3, 'member');

INSERT INTO OBJECTIVES (account_id, name, target_amount, current_save, deadline)
VALUES (1, 'Viaje Japón', 3000.00, 0.00, '2026-12-01');

INSERT INTO TRANSACTIONS (account_id, user_id, category, objective_id, amount, transaction_type, concept, split_type)
VALUES (1, 1, 'Ocio', 1, 100.00, 'saving', 'Aportación Japón', 'individual');

INSERT INTO TRANSACTIONS (account_id, user_id, category, amount, transaction_type, concept, split_type)
VALUES (3, 2, 'Hogar', 50.00, 'expense', 'Fibra Óptica', 'contribution');

INSERT INTO TRANSACTIONS (account_id, user_id, category, amount, transaction_type, concept, split_type)
VALUES (3, 1, 'Comida', 60.00, 'expense', 'Cena Viernes', 'divided'); 

INSERT INTO TRANSACTION_SPLITS (user_id, transaction_id, amount_assigned, status)
VALUES (1, 3, 30.00, 'settled');
INSERT INTO TRANSACTION_SPLITS (user_id, transaction_id, amount_assigned, status)
VALUES (2, 3, 30.00, 'pending');
GO