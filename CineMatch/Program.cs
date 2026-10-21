using System.Text;
using CineMatch.Datos;
using CineMatch.Modelos;

Console.OutputEncoding = Encoding.UTF8;
var motor = DatosIniciales.CrearMotor();

while (true)
{
    Console.WriteLine("\n=== CineMatch ===");
    Console.WriteLine("1. Ver catálogo");
    Console.WriteLine("2. Registrar usuario");
    Console.WriteLine("3. Calificar película");
    Console.WriteLine("4. Ver historial de calificaciones");
    Console.WriteLine("5. Obtener recomendaciones");
    Console.WriteLine("6. Buscar película por ID");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");

    var opcion = Console.ReadLine();
    if (opcion == "0") break;

    try
    {
        switch (opcion)
        {
            case "1":
                foreach (var pelicula in motor.CatalogoOrdenado())
                    Console.WriteLine($"{pelicula.Id}. {pelicula.Titulo} ({pelicula.Anio}) - {pelicula.Genero}");
                break;

            case "2":
                motor.RegistrarUsuario(new Usuario(LeerNumero("ID de usuario: "), LeerTexto("Nombre: ")));
                Console.WriteLine("Usuario registrado.");
                break;

            case "3":
                motor.Calificar(LeerNumero("ID de usuario: "), LeerNumero("ID de película: "), LeerNumero("Calificación (1-5): "));
                Console.WriteLine("Calificación registrada.");
                break;

            case "4":
                var idHistorial = LeerNumero("ID de usuario: ");
                if (!motor.BuscarUsuario(idHistorial, out var usuario))
                {
                    Console.WriteLine("Usuario no encontrado.");
                    break;
                }
                Console.WriteLine($"Calificaciones de {usuario.Nombre}:");
                foreach (var calificacion in usuario.Calificaciones)
                {
                    motor.BuscarPelicula(calificacion.IdPelicula, out var pelicula);
                    Console.WriteLine($"- {pelicula.Titulo}: {calificacion.Valor}/5");
                }
                break;

            case "5":
                var idUsuario = LeerNumero("ID de usuario: ");
                var cantidad = LeerNumero("Cantidad de recomendaciones: ");
                var recomendaciones = motor.GenerarRecomendaciones(idUsuario, cantidad);
                if (recomendaciones.Cantidad == 0) Console.WriteLine("No hay películas disponibles para recomendar.");
                foreach (var r in recomendaciones)
                    Console.WriteLine($"- {r.Pelicula.Titulo} | {r.Puntuacion:F2}/5 | {r.Motivo}");
                break;

            case "6":
                var idPelicula = LeerNumero("ID de película: ");
                Console.WriteLine(motor.BuscarPelicula(idPelicula, out var encontrada)
                    ? $"{encontrada.Titulo} ({encontrada.Anio}) - {encontrada.Genero}"
                    : "Película no encontrada.");
                break;

            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }
    catch (Exception error) when (error is ArgumentException or InvalidOperationException or KeyNotFoundException)
    {
        Console.WriteLine($"Error: {error.Message}");
    }
}

static int LeerNumero(string mensaje)
{
    Console.Write(mensaje);
    if (int.TryParse(Console.ReadLine(), out var valor)) return valor;
    throw new ArgumentException("Ingrese un número entero válido.");
}

static string LeerTexto(string mensaje)
{
    Console.Write(mensaje);
    return Console.ReadLine() ?? string.Empty;
}
