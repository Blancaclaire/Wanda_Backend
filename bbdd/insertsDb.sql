USE wandaDB;
GO

-- =============================================
-- 1. USUARIOS Y CUENTAS PARTICULARES
-- =============================================

-- Insertamos a Ana y Juan
INSERT INTO USERS (name, email, password) VALUES ('Ana García', 'ana@wanda.com', 'hash_password_1');
INSERT INTO USERS (name, email, password) VALUES ('Juan Pérez', 'juan@wanda.com', 'hash_password_2');

-- Cuentas particulares (Creadas automáticamente al registrarse)
INSERT INTO ACCOUNTS (name, account_type, amount, weekly_budget, monthly_budget) 
VALUES ('Ahorros Ana', 'personal', 1500.00, 100.00, 400.00); -- ID 1

INSERT INTO ACCOUNTS (name, account_type, amount, weekly_budget, monthly_budget) 
VALUES ('Cuenta Juan', 'personal', 1200.00, 80.00, 350.00); -- ID 2

-- Vinculamos usuarios con sus cuentas privadas otorgándoles el rol de 'admin'
INSERT INTO ACCOUNT_USERS (user_id, account_id, role) VALUES (1, 1, 'admin');
INSERT INTO ACCOUNT_USERS (user_id, account_id, role) VALUES (2, 2, 'admin');

-- =============================================
-- 2. CUENTA CONJUNTA (Con gestión de roles)
-- =============================================

-- Creamos la cuenta compartida
INSERT INTO ACCOUNTS (name, account_type, amount) 
VALUES ('Casa Ana y Juan', 'joint', 500.00); -- ID 3

-- Vinculamos a ambos a la cuenta conjunta: Ana es Admin (creadora) y Juan es Member
INSERT INTO ACCOUNT_USERS (user_id, account_id, role) VALUES (1, 3, 'admin');
INSERT INTO ACCOUNT_USERS (user_id, account_id, role) VALUES (2, 3, 'member');

-- =============================================
-- 3. OBJETIVOS Y AHORRO
-- =============================================

-- Ana tiene un objetivo de viaje en su cuenta particular
INSERT INTO OBJECTIVES (account_id, name, target_amount, current_ahorro, deadline)
VALUES (1, 'Viaje Japón', 3000.00, 0.00, '2026-12-01'); -- ID 1

-- Ana aporta 100€ a su objetivo (La categoría 'Ocio' se pasa como texto directo)
INSERT INTO TRANSACTIONS (account_id, user_id, category, objective_id, amount, transaction_type, concept, split_type)
VALUES (1, 1, 'Ocio', 1, 100.00, 'saving', 'Aportación Japón', 'individual');

-- =============================================
-- 4. MOVIMIENTOS COMPARTIDOS
-- =============================================

-- CASO A: Gasto compartido pagado al 100% por uno (Contribution)
-- Juan paga el Internet de la cuenta conjunta. Categoría 'Hogar' integrada como texto.
INSERT INTO TRANSACTIONS (account_id, user_id, category, amount, transaction_type, concept, split_type)
VALUES (3, 2, 'Hogar', 50.00, 'expense', 'Fibra Óptica', 'contribution');

-- CASO B: Gasto compartido dividido al 50% (Divided)
-- Cenan fuera y pagan con la cuenta conjunta 60€. Categoría 'Comida' integrada.
INSERT INTO TRANSACTIONS (account_id, user_id, category, amount, transaction_type, concept, split_type)
VALUES (3, 1, 'Comida', 60.00, 'expense', 'Cena Viernes', 'divided'); -- ID generado: 3

-- Registro de reparto en splits (Se mantiene igual, referenciando a la transacción 3)
INSERT INTO TRANSACTION_SPLITS (user_id, transaction_id, amount_assigned, status)
VALUES (1, 3, 30.00, 'settled');

INSERT INTO TRANSACTION_SPLITS (user_id, transaction_id, amount_assigned, status)
VALUES (2, 3, 30.00, 'pending');