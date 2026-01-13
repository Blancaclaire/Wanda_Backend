INSERT INTO wandaDB.dbo.TRANSACTIONS (account_id,user_id,category_id,objective_id,amount,transaction_type,concept,transaction_date,isRecurring,frequency,end_date,split_type) VALUES
	 (1,1,3,1,100.00,N'saving',N'Aportación Japón','2026-01-13 09:30:03.723',0,NULL,NULL,N'individual'),
	 (3,2,3,NULL,50.00,N'expense',N'Fibra Óptica','2026-01-13 09:30:03.733',0,NULL,NULL,N'contribution'),
	 (3,1,1,NULL,60.00,N'expense',N'Cena Viernes','2026-01-13 09:30:03.743',0,NULL,NULL,N'divided'),
	 (1,1,3,1,100.00,N'saving',N'Aportación Japón','2026-01-13 09:30:14.45',0,NULL,NULL,N'individual'),
	 (3,2,3,NULL,50.00,N'expense',N'Fibra Óptica','2026-01-13 09:30:14.46',0,NULL,NULL,N'contribution'),
	 (3,1,1,NULL,60.00,N'expense',N'Cena Viernes','2026-01-13 09:30:14.467',0,NULL,NULL,N'divided');
