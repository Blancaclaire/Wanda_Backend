
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