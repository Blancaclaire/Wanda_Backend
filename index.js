export const handler = async (event) => {
  // Leemos la configuración del entorno de AWS
  const apiUrl = process.env.API_URL;
  const secret = process.env.CRON_SECRET;

  // Validación simple para evitar errores tontos
  if (!apiUrl || !secret) {
    console.error("Faltan variables de entorno");
    return { statusCode: 500, body: "Error de configuración en Lambda" };
  }

  try {
    console.log(`Llamando a: ${apiUrl}`);

    // Hacemos la petición POST a tu servidor .NET
    const response = await fetch(apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "X-Cron-Secret": secret // Enviamos la contraseña en la cabecera
      }
    });

    if (!response.ok) {
      throw new Error(`La API respondió con error: ${response.status}`);
    }

    const data = await response.json();
    console.log("Éxito:", data);

    return { statusCode: 200, body: JSON.stringify(data) };

  } catch (error) {
    console.error("Fallo al ejecutar el Cron:", error);
    // Lanzamos el error para que AWS marque la ejecución como "Failed" (Rojo)
    throw error; 
  }
};