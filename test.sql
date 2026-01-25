USE master;
GO
-- Esto cierra todas las conexiones y transacciones pendientes en wandaDB
ALTER DATABASE [wandaDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
-- Lo devolvemos a modo multiusuario
ALTER DATABASE [wandaDB] SET MULTI_USER;
GO