DROP DATABASE wandaDB;
CREATE DATABASE wandaDB;
GO
USE wandaDB;
GO


CREATE TABLE USERS (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL
);


CREATE TABLE ACCOUNTS (
    account_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    account_type NVARCHAR(20) NOT NULL CHECK (account_type IN ('personal', 'joint')),
    amount DECIMAL(18, 2) DEFAULT 0.00,
    weekly_budget DECIMAL(18, 2) NULL,
    monthly_budget DECIMAL(18, 2) NULL,
    account_picture_url NVARCHAR(MAX) NULL
);

CREATE TABLE ACCOUNT_USERS (
    user_id INT NOT NULL,
    account_id INT NOT NULL,
    PRIMARY KEY (user_id, account_id),
    CONSTRAINT FK_AccountUsers_User FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_AccountUsers_Account FOREIGN KEY (account_id) REFERENCES ACCOUNTS(account_id)
);


CREATE TABLE CATEGORIES (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    account_id INT NULL, 
    name NVARCHAR(50) NOT NULL,
    icon_url NVARCHAR(MAX) NULL,
    type NVARCHAR(20) NOT NULL CHECK (type IN ('income', 'expense')),
    CONSTRAINT FK_Categories_Account FOREIGN KEY (account_id) REFERENCES ACCOUNTS(account_id)
);


CREATE TABLE OBJECTIVES (
    objective_id INT IDENTITY(1,1) PRIMARY KEY,
    account_id INT NOT NULL,
    name NVARCHAR(100) NOT NULL,
    target_amount DECIMAL(18, 2) NOT NULL,
    current_ahorro DECIMAL(18, 2) DEFAULT 0.00,
    deadline DATE NULL,
    objetive_picture_url NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Objectives_Account FOREIGN KEY (account_id) REFERENCES ACCOUNTS(account_id)
);


CREATE TABLE TRANSACTIONS (
    transaction_id INT IDENTITY(1,1) PRIMARY KEY,
    account_id INT NOT NULL,
    user_id INT NOT NULL,
    category_id INT NOT NULL,
    objective_id INT NULL, 
    amount DECIMAL(18, 2) NOT NULL,
    transaction_type NVARCHAR(20) NOT NULL CHECK (transaction_type IN ('income', 'expense', 'saving')),
    concept NVARCHAR(255) NOT NULL,
    transaction_date DATETIME DEFAULT GETDATE(),
    isRecurring BIT DEFAULT 0,
    frequency NVARCHAR(20) NULL CHECK (frequency IN ('monthly', 'weekly', 'annual')),
    end_date DATE NULL,
    split_type NVARCHAR(20) NOT NULL CHECK (split_type IN ('individual', 'contribution', 'divided')),
    
    CONSTRAINT FK_Transactions_Account FOREIGN KEY (account_id) REFERENCES ACCOUNTS(account_id),
    CONSTRAINT FK_Transactions_User FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_Transactions_Category FOREIGN KEY (category_id) REFERENCES CATEGORIES(category_id),
    CONSTRAINT FK_Transactions_Objective FOREIGN KEY (objective_id) REFERENCES OBJECTIVES(objective_id)
);


CREATE TABLE TRANSACTION_SPLITS (
    split_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    transaction_id INT NOT NULL,
    amount_assigned DECIMAL(18, 2) NOT NULL,
    status NVARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'settled')),
    
    CONSTRAINT FK_Splits_User FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    CONSTRAINT FK_Splits_Transaction FOREIGN KEY (transaction_id) REFERENCES TRANSACTIONS(transaction_id)
);

GO
USE wandaDB;
GO

-- =============================================
-- 1. USUARIOS Y CUENTAS PARTICULARES
-- =============================================

-- Insertamos a Ana y Juan
INSERT INTO USERS (name, email, password) VALUES ('Ana García', 'ana@wanda.com', 'hash_password_1');
INSERT INTO USERS (name, email, password) VALUES ('Juan Pérez', 'juan@wanda.com', 'hash_password_2');

-- Cuentas particulares (Obligatorias antes de la conjunta)
INSERT INTO ACCOUNTS (name, account_type, amount, weekly_budget, monthly_budget) 
VALUES ('Ahorros Ana', 'personal', 1500.00, 100.00, 400.00); -- ID 1

INSERT INTO ACCOUNTS (name, account_type, amount, weekly_budget, monthly_budget) 
VALUES ('Cuenta Juan', 'personal', 1200.00, 80.00, 350.00); -- ID 2

-- Vinculamos usuarios con sus cuentas privadas
INSERT INTO ACCOUNT_USERS (user_id, account_id) VALUES (1, 1);
INSERT INTO ACCOUNT_USERS (user_id, account_id) VALUES (2, 2);

-- =============================================
-- 2. CATEGORÍAS (Globales y Personalizadas)
-- =============================================

-- Categorías Globales (account_id es NULL)
INSERT INTO CATEGORIES (account_id, name, icon_url, type) VALUES (NULL, 'Comida', 'url_icon_food', 'expense'); -- ID 1
INSERT INTO CATEGORIES (account_id, name, icon_url, type) VALUES (NULL, 'Sueldo', 'url_icon_salary', 'income'); -- ID 2
INSERT INTO CATEGORIES (account_id, name, icon_url, type) VALUES (NULL, 'Ocio', 'url_icon_fun', 'expense'); -- ID 3

-- Categoría Personalizada para la cuenta de Ana
INSERT INTO CATEGORIES (account_id, name, icon_url, type) VALUES (1, 'Curso Diseño', 'url_icon_study', 'expense'); -- ID 4

-- =============================================
-- 3. CUENTA CONJUNTA
-- =============================================

-- Creamos la cuenta compartida
INSERT INTO ACCOUNTS (name, account_type, amount) 
VALUES ('Casa Ana y Juan', 'joint', 500.00); -- ID 3

-- Vinculamos a ambos a la cuenta conjunta
INSERT INTO ACCOUNT_USERS (user_id, account_id) VALUES (1, 3);
INSERT INTO ACCOUNT_USERS (user_id, account_id) VALUES (2, 3);

-- =============================================
-- 4. OBJETIVOS Y AHORRO
-- =============================================

-- Ana tiene un objetivo de viaje en su cuenta particular
INSERT INTO OBJECTIVES (account_id, name, target_amount, current_ahorro, deadline)
VALUES (1, 'Viaje Japón', 3000.00, 0.00, '2026-12-01'); -- ID 1

-- Ana aporta 100€ a su objetivo (Transacción tipo 'saving')
INSERT INTO TRANSACTIONS (account_id, user_id, category_id, objective_id, amount, transaction_type, concept, split_type)
VALUES (1, 1, 3, 1, 100.00, 'saving', 'Aportación Japón', 'individual');

-- =============================================
-- 5. MOVIMIENTOS COMPARTIDOS (La joya de WANDA)
-- =============================================

-- CASO A: Gasto compartido pagado al 100% por uno (Contribution)
-- Juan paga el Internet de la cuenta conjunta
INSERT INTO TRANSACTIONS (account_id, user_id, category_id, amount, transaction_type, concept, split_type)
VALUES (3, 2, 3, 50.00, 'expense', 'Fibra Óptica', 'contribution');

-- CASO B: Gasto compartido dividido al 50% (Divided)
-- Cenan fuera y pagan con la cuenta conjunta 60€
INSERT INTO TRANSACTIONS (account_id, user_id, category_id, amount, transaction_type, concept, split_type)
VALUES (3, 1, 1, 60.00, 'expense', 'Cena Viernes', 'divided'); -- ID de Transacción generado (ej: 3)

-- Registramos el reparto en la tabla de splits (30€ cada uno)
INSERT INTO TRANSACTION_SPLITS (user_id, transaction_id, amount_assigned, status)
VALUES (1, 3, 30.00, 'settled'); -- Ana ya pagó su parte al ser la cuenta conjunta

INSERT INTO TRANSACTION_SPLITS (user_id, transaction_id, amount_assigned, status)
VALUES (2, 3, 30.00, 'pending'); -- Juan tiene pendiente "devolver" o equilibrar su parte