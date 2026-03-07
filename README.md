Wanda Backend
API en .NET 8 para la gestión de finanzas personales y compartidas.

🚀 Tecnologías
Stack: .NET 8, SQL Server, Entity Framework.

Seguridad: JWT (Autenticación) y BCrypt (Hasheo).

Extras: Cloudinary (Imágenes) y xUnit (Tests).

🛠️ Configuración Rápida
Base de Datos: Levantar SQL Server y ejecutar el script /bbdd/createDb.sql.

Configuración: Actualizar ConnectionStrings y CloudinarySettings en appsettings.json.

Ejecución:

Bash
dotnet run
Swagger: Disponible en https://localhost:7085/swagger.

✨ Funcionalidades
Cuentas: Personales (automáticas) y conjuntas.

Transacciones: Ingresos, gastos y ahorro con soporte para recurrencia.

Deudas: División de gastos automática/manual y sistema de liquidación.

Objetivos: Seguimiento de metas de ahorro con fechas límite.

👥 Usuarios de Prueba
Rol        Email           Password
Admin     admin@gmail.com  Admin1234!
User      user1@gmail.com  Admin1234!
