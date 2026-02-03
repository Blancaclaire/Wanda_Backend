USE wandaDB;

GO
INSERT INTO TRANSACTIONS (
    account_id, user_id, category, amount, transaction_type, 
    concept, transaction_date, isRecurring, frequency, split_type, 
    last_execution_date 
)
VALUES (
    1, 
    1, 
    'Suscripciones', 
    15.00, 
    'expense', 
    'Netflix Mensual (Test Recurrencia)', 
    DATEADD(month, -1, GETDATE()), 
    1, 
    'monthly', 
    'individual',
    DATEADD(month, -1, GETDATE()) 
);