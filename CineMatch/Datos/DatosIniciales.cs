using CineMatch.Modelos;
using CineMatch.Servicios;

namespace CineMatch.Datos;

public static class DatosIniciales
{
    public static MotorRecomendacion CrearMotor()
    {
        var motor = new MotorRecomendacion();

        motor.RegistrarPelicula(new Pelicula(1, "Interestelar", "Ciencia ficción", 2014));
        motor.RegistrarPelicula(new Pelicula(2, "Matrix", "Ciencia ficción", 1999));
        motor.RegistrarPelicula(new Pelicula(3, "Origen", "Ciencia ficción", 2010));
        motor.RegistrarPelicula(new Pelicula(4, "La llegada", "Ciencia ficción", 2016));
        motor.RegistrarPelicula(new Pelicula(5, "Blade Runner 2049", "Ciencia ficción", 2017));
        motor.RegistrarPelicula(new Pelicula(6, "Coco", "Animación", 2017));

        motor.RegistrarUsuario(new Usuario(1, "Ana"));
        motor.RegistrarUsuario(new Usuario(2, "Luis"));
        motor.RegistrarUsuario(new Usuario(3, "Elena"));
        motor.RegistrarUsuario(new Usuario(4, "Carlos"));
        motor.RegistrarUsuario(new Usuario(5, "Sofía"));

        motor.Calificar(1, 1, 5);
        motor.Calificar(1, 2, 4);
        motor.Calificar(2, 1, 5);
        motor.Calificar(2, 2, 4);
        motor.Calificar(2, 3, 5);
        motor.Calificar(2, 4, 4);
        motor.Calificar(3, 1, 4);
        motor.Calificar(3, 2, 5);
        motor.Calificar(3, 3, 4);
        motor.Calificar(3, 5, 5);
        motor.Calificar(4, 6, 5);

        return motor;
    }
}
