# Wanda Backend

API robusta desarrollada en **.NET 8** para la gestión de finanzas personales, cuentas compartidas y seguimiento de objetivos de ahorro.

## 🚀 Tecnologías
* **Framework:** .NET 8 Web API.
* **Base de Datos:** SQL Server.
* **Seguridad:** Autenticación JWT y hasheo de contraseñas con BCrypt.
* **Infraestructura:** Cloudinary para gestión de imágenes y Background Workers para procesos recurrentes.
* **Testing:** xUnit, Moq y FluentAssertions.

## 🛠️ Configuración
1.  **Base de Datos:** Levantar una instancia de SQL Server y ejecutar el script ubicado en `/bbdd/createDb.sql`.
2.  **Variables de Entorno:** Configurar las credenciales de `wandaDb` (SQL Server), `JWT` y `CloudinarySettings` en el archivo `appsettings.json`.

## 📦 Ejecución y Uso
* **Iniciar servidor:**
    ```bash
    dotnet run
    ```
* **Documentación API:** Swagger disponible en `https://localhost:7085/swagger` una vez iniciado el servidor.
* **Tests:** Ejecutar `dotnet test` para validar la suite de pruebas unitarias.

## 👥 Acceso de Prueba (Password: Admin1234!)
* **Administrador:** admin@gmail.com
* **Usuario estándar:** user1@gmail.com

---
*Entregable del servidor de gestión financiera Wanda.*
