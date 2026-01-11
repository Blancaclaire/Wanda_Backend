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