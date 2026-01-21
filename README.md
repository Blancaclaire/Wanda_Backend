# Wanda_Backend

#Setup local db (Sql server)
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04


# EXPLICACION MODELOS POR TABLAS

USERS

GET /api/User: Consulta todos los Usuarios

POST /api/User: Crea un usuario, y automáticamente se crea su cuenta de tipo personal (ACCOUNTS) y su registro en la tabla intermedia ACCOUNTS_USERS. Al ser el usuario quien crea la cuenta se registra en la tabla intermedia con un rol de tipo 'admin'. El post de usuario y cuenta se hacen con los datos mínimos del registro (Figma), el resto campos vendrán como null en un inicio.

**Recomendable encapsular los metodos de validación necesarios para añadir un usuario. La función es muy larga

GET /api/User/{userId}: Consulta un usuario

PUT /api/User/{userId}: Modifica un usuario

DELETE /api/User/{userId}:****Debe implementarse desde sql un método para que si se borra un usuario se borren tambien sus cuentas y sus registros en tabla intermedia.



ACCOUNTS

GET /api/Account: Consulta todos los Usuarios

POST /api/Account: Se usa específicamente para crear cuentas compartidas, puesto que las personales se crean automáticamente al crear el usuario. Implementa métodos de validación como que la menos la lista de usuarios sea mayor a una persona, y el nombre de la cuneta no sea nulo. Al igual que en Users, cuando se añade una cuenta se llama tambien al repositorio de la tabla intermedia. Estableciendo como admin al dueño de la cuenta y al resto con rol member.

**Recomendable añadir método de comprobación de que los ids que se estan metiendo son de usuarios de wanda

PUT /api/Account/{accountId}: Modifica una cuenta. En el caso de la cuenta conjunta no puede modificarse el campo amount puesto que esta no tendria un valor como tal que se gestione en la aplicación.

DELETE /api/Account/{accountId}: Borra una cuenta y a la vez se borra su registro en la tabla intermedia para no dejar registros huérfanos.


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


POST /api/accounts/{accountId}/transactions: Crea una nueva transacción (ya sea gasto, ingreso o ahorro). 
Cuenta conjunta: Cuando se hace un post de una transaccion en una cuenta de tipo conjunta, este gasto debe aparecer reflejado realmente en el campo amount de la cuenta personal de la persona que hace la transacción, identificandolo por medio de su user_id.
Pasos a seguir: 

1. Identificar la Cuenta Personal del Pagador: Al crear un usuario, ya creas una cuenta personal. Necesitarás una consulta que busque: "Dame la cuenta de tipo 'personal' donde este user_id es el admin".
2. Actualizar Saldo Personal: Restar el amount total de la transacción de esa cuenta personal encontrada.
3. Registrar en la Conjunta: Insertar la transacción con el account_id de la cuenta conjunta. Esto permite que, al consultar los movimientos de la conjunta, aparezca el gasto para todos los miembros.
4. Gestionar el Reparto (Splits): * Si es un Gasto íntegro (split_type = 'contribution'): Simplemente se anota quién lo pagó.
Si es un Gasto dividido (split_type = 'divided'): Creas registros en TRANSACTION_SPLITS indicando cuánto debe cada uno al pagador.
5. En adición a esto si la transaccion es de tipo saving debera actualizarse tambien el campo current_ahorro para actualizar que se ha sumado dicha cantidad al objetivo.

**Ejemplo: Si Ana paga el alquiler de 1000€ en la cuenta conjunta y lo dividen al 50%:
POST a /api/accounts/{jointId}/transactions con user_id = Ana, amount = 1000, split_type = 'divided'.
Servidor:
Resta 1000€ del amount de la Cuenta Personal de Ana.
Registra la transacción en la Cuenta Conjunta (para que Juan vea el gasto).
Crea un Split: Juan debe 500€ a Ana en TRANSACTION_SPLITS. -->El frontend debe reflejar un apartado de "deudas" para que Juan pueda "pagar" a Ana lo que le debe, y entonces: 

- Se resta 500€ de la cuenta personal de Juan
- Se suma 500€ a la cuenta personal de Ana
- Se marca el split como settled

**Añadir en AccountRepository un metodo para obtener la cuenta personal de un usuario: "SELECT a.account_id 
FROM ACCOUNTS a
JOIN ACCOUNT_USERS au ON a.account_id = au.account_id
WHERE au.user_id = @userId AND a.account_type = 'personal';"


**Debe impedirse que se haga una transaccion que deje el saldo negativo, que devuelva un aviso indicando que no puede realizarse la accion.

Si la cuenta es de tipo personal, split_type= individual
Si la cuenta es de tipo conjunta, split_type= contribution o divided

GET /api/transactions/{transactionId}: Consulta el detalle de un movimiento específico

PUT /api/transactions/{transactionId}: Para modificar una transacción existente.

*no puede actualizarse el account_id
*Falta por pensar

DELETE /api/transactions/{transactionId}: Para eliminar una transacción. Si se elimina una transaccion automaticamente se actualiza amount de ACCOUNTS y si es de tipo saving tambien current_ahorro. 
Si el split_type es divided debe eliminarse tambien de la tabla TRANSACTION_SPLITS
*Falta por pensar