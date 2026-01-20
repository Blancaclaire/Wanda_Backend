# Wanda_Backend

#Setup local db (Sql server)
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04


# EXPLICACION MODELOS POR TABLAS

USERS

GET /api/User -->Consulta todos Usuarios

POST /api/User --> Crea un usuario, y automaticamente se crea su cuenta de tipo personal (ACCOUNTS) y su registro en la tabla intermedia ACCOUNTS_USERS. Al ser el usaurio quien crea la cuenta se registra en la tabla intermedia con un rol de tipo 'admin'. El post de usuario y cuenta se hacen con los datos minimos del registro (Figma), el resto vendrán de campos vendrán como null en un inicio.

**Recomendable encapsular los metodos de validacion necesarios para añadir un usuario. La funcion es muy larga

GET /api/User/{userId} -->Consulta un usuario

PUT /api/User/{userId} -->Modifica un usuario

DELETE /api/User/{userId} -->******Debe implementarse desde sql un metodo para que si se borra un usuario se borren tambien sus cuentas y sus registros en tabla intermedia



ACCOUNTS

GET /api/Account -->Consulta todos los Usuarios

POST /api/Account -->Se usa especificamente para crear cuentas compartidas, puesto que las personales se crean automaticamente al crear el usuario. Implementa metodos de validación como que la menos la lista de usuarios sea mayor a una persona, y el nombre de la cuneta no sea nulo. Al igual que en Users, cuando se añade una cuenta se llama tambien al repositorio de la tabla intermedia. Estableciendo como admin al dueño de la cuenta y al resto con rol member.

**Recomendable añadir metodo de comprobacion de que los ids que se esatn metiendo son de usuarios de wanda

PUT /api/Account/{accountId}--> Modifica un cuenta. En el caso de la cuenta conjunta especificamente, no puede modificarse el campo amount puesto que esta no tendria un valor como tal.

DELETE /api/Account/{accountId}-->Borra una cuentay a la vez se borrasu registro en la tabla intermedia para no dejar registros huerfanos.


OBJECTIVES

GET /api/accounts/{accountId}/objectives --> Recupera todos los objetivos asociados a esa cuenta específica.

POST /api/accounts/{accountId}/objectives: Crea un nuevo objetivo dentro de esa cuenta.

GET /api/objectives/{objectiveId}: Una vez creado el recurso, puedes acceder a él directamente por su ID único para detalles específicos.

PUT /api/objectives/{objectiveId}: Para actualizar los datos del objetivo

DELETE /api/objectives/{objectiveId}: Para eliminar el objetivo.


**Crear un ObjetivesService y objectives controller que aunque se haga referencia a cosas de accounts, encapsule toda la logica relacionado con objectives


TRANSACTIONS

GET /api/accounts/{accountId}/transactions --> Lista el historial de movimientos de una cuenta específica.
*Deben añadirse filtros para transaction_type, isRecurring, split_type, ordenacion descendente,fecha,categorias


POST /api/accounts/{accountId}/transactions: Crea una nueva transacción (ya sea gasto, ingreso o ahorro). Automaticamente debe restarse o sumarse (Update) el campo amount de la tabla ACCOUNTS. Y si es de tipo saving ademas tiene que sumarse al campo current_ahorro de la tabla OBJECTIVES.

Debe impedirse que se haga una transaccionque deje el saldo negativo, que devuelva un aviso indicando que no puede realizarse la accion.

Si la cuenta es de tipo personal, split_type= individual
Si la cuenta es de tipo conjunta, split_type= contribution o divided

GET /api/transactions/{transactionId}: Consulta el detalle de un movimiento específico

PUT /api/transactions/{transactionId}: Para modificar una transacción existente.

*no puede actualizarse el account_id
*Debe sumarse o restarse la cantidad al saldo original de la cuenta (ACCOUNT.amount)

*Si es de tipo 'saving' debe hacerse un update del current_ahorro

DELETE /api/transactions/{transactionId}: Para eliminar una transacción. Si se elimina una transaccion automaticamente se actualiza amount de ACCOUNTS y si es de tipo saving tambien current_ahorro. 
Si el split_type es divided debe eliminarse tambien de la tabla TRANSACTION_SPLITS