using CineMatch.Datos;
using CineMatch.Estructuras;
using CineMatch.Modelos;
using CineMatch.Servicios;

var pruebas = new (string Nombre, Action Ejecutar)[]
{
    ("Lista enlazada: inserción y búsqueda", ProbarLista),
    ("Tabla hash: colisiones y redimensionamiento", ProbarHash),
    ("Tabla hash: clave duplicada", ProbarHashDuplicada),
    ("Árbol AVL: orden y balanceo", ProbarAvl),
    ("Cola de prioridad: máximo y empate", ProbarCola),
    ("Grafo: vecinos y actualización de arista", ProbarGrafo),
    ("Similitud del coseno entre usuarios", ProbarSimilitud),
    ("Recomendación colaborativa y exclusión", ProbarRecomendaciones),
    ("Recomendación de respaldo para nuevo usuario", ProbarRespaldo),
    ("Cantidad máxima solicitada", ProbarLimite),
    ("Actualización de calificación sin duplicar", ProbarActualizacion),
    ("Calificación fuera de rango", ProbarValorInvalido),
    ("Usuario inexistente", ProbarUsuarioInexistente),
    ("Cantidad inválida", ProbarCantidadInvalida)
};

var aprobadas = 0;
foreach (var prueba in pruebas)
{
    try
    {
        prueba.Ejecutar();
        Console.WriteLine($"[OK] {prueba.Nombre}");
        aprobadas++;
    }
    catch (Exception error)
    {
        Console.WriteLine($"[ERROR] {prueba.Nombre}: {error.Message}");
    }
}
Console.WriteLine($"\nResultados: {aprobadas}/{pruebas.Length} pruebas aprobadas.");
return aprobadas == pruebas.Length ? 0 : 1;

static void Verificar(bool condicion, string mensaje)
{
    if (!condicion) throw new Exception(mensaje);
}

static void DebeLanzar<T>(Action accion) where T : Exception
{
    try { accion(); }
    catch (T) { return; }
    throw new Exception($"Se esperaba la excepción {typeof(T).Name}.");
}

static void ProbarLista()
{
    var lista = new ListaEnlazada<int>();
    lista.Agregar(3);
    lista.Agregar(7);
    Verificar(lista.Cantidad == 2, "Cantidad incorrecta.");
    Verificar(lista.Buscar(x => x == 7) == 7, "No encontró el elemento.");
    Verificar(lista.Buscar(x => x == 9) == 0, "Búsqueda no encontrada incorrecta.");
}

static void ProbarHash()
{
    var tabla = new TablaHash<string>(17);
    tabla.Agregar(1, "uno");
    tabla.Agregar(18, "dieciocho");
    for (var id = 30; id < 60; id++) tabla.Agregar(id, id.ToString());
    Verificar(tabla.IntentarObtener(1, out var uno) && uno == "uno", "Falló al redimensionar.");
    Verificar(tabla.IntentarObtener(18, out var dieciocho) && dieciocho == "dieciocho", "Colisión no resuelta.");
    Verificar(tabla.Cantidad == 32, "Cantidad incorrecta.");
}

static void ProbarHashDuplicada()
{
    var tabla = new TablaHash<int>();
    tabla.Agregar(1, 100);
    DebeLanzar<InvalidOperationException>(() => tabla.Agregar(1, 200));
}

static void ProbarAvl()
{
    var arbol = new ArbolAvl();
    foreach (var titulo in new[] { "A", "B", "C", "D", "E", "F", "G" })
        arbol.Insertar(new Pelicula(titulo[0] - 'A' + 1, titulo, "Prueba", 2020));

    var titulos = string.Join(",", arbol.RecorrerOrdenado().Select(p => p.Titulo));
    Verificar(titulos == "A,B,C,D,E,F,G", "Recorrido en orden incorrecto.");
    Verificar(arbol.Altura == 3, "No se balanceó correctamente.");
}

static void ProbarCola()
{
    var cola = new ColaPrioridad<string>();
    cola.Encolar("primero", 5);
    cola.Encolar("bajo", 1);
    cola.Encolar("segundo", 5);
    Verificar(cola.Desencolar() == "primero", "No respetó el empate.");
    Verificar(cola.Desencolar() == "segundo", "No respetó la prioridad.");
    Verificar(cola.Desencolar() == "bajo" && cola.EstaVacia, "La cola no quedó vacía.");
    DebeLanzar<InvalidOperationException>(() => cola.Desencolar());
}

static void ProbarGrafo()
{
    var grafo = new GrafoValoraciones();
    grafo.Registrar(1, 10, 5);
    grafo.Registrar(2, 10, 4);
    grafo.Registrar(3, 11, 4);
    Verificar(grafo.UsuariosRelacionados(1).Single() == 2, "No encontró vecinos correctos.");
    grafo.Registrar(1, 10, 3);
    Verificar(grafo.CalificacionesDePelicula(10).Count() == 2, "Duplicó la arista.");
    Verificar(grafo.CalificacionesDeUsuario(1).Single().Valor == 3, "No actualizó la arista.");
}

static void ProbarSimilitud()
{
    var motor = DatosIniciales.CrearMotor();
    Verificar(Math.Abs(motor.CalcularSimilitud(1, 2) - 1) < 0.000001, "La similitud esperada era 1.");
    Verificar(motor.CalcularSimilitud(1, 4) == 0, "No debería haber películas compartidas.");
}

static void ProbarRecomendaciones()
{
    var motor = DatosIniciales.CrearMotor();
    var recomendaciones = motor.GenerarRecomendaciones(1, 3).ToArray();
    Verificar(recomendaciones.Length == 3, "Cantidad incorrecta de recomendaciones.");
    Verificar(recomendaciones.All(r => r.Pelicula.Id is not (1 or 2)), "Incluyó películas ya calificadas.");
    Verificar(recomendaciones.Any(r => r.Pelicula.Id == 5), "No incluyó la película 5.");
    Verificar(recomendaciones.All(r => r.Motivo.Contains("Usuarios")), "No aplicó filtrado colaborativo.");
    Verificar(recomendaciones[0].Puntuacion >= recomendaciones[1].Puntuacion, "El resultado no está priorizado.");
}

static void ProbarRespaldo()
{
    var motor = DatosIniciales.CrearMotor();
    var recomendaciones = motor.GenerarRecomendaciones(5, 2).ToArray();
    Verificar(recomendaciones.Length == 2, "Cantidad incorrecta en respaldo.");
    Verificar(recomendaciones.All(r => r.Motivo.Contains("respaldo")), "No utilizó el mecanismo de respaldo.");
    Verificar(recomendaciones[0].Puntuacion >= recomendaciones[1].Puntuacion, "No ordenó por promedio.");
}

static void ProbarLimite()
{
    var motor = DatosIniciales.CrearMotor();
    Verificar(motor.GenerarRecomendaciones(1, 1).Cantidad == 1, "Ignoró el límite solicitado.");
}

static void ProbarActualizacion()
{
    var motor = DatosIniciales.CrearMotor();
    motor.Calificar(1, 1, 3);
    motor.BuscarUsuario(1, out var usuario);
    Verificar(usuario.Calificaciones.Cantidad == 2, "Duplicó la calificación.");
    Verificar(usuario.BuscarCalificacion(1)!.Valor == 3, "No actualizó la calificación.");
    Verificar(motor.CalcularSimilitud(1, 2) < 1, "El motor no reflejó la actualización.");
}

static void ProbarValorInvalido()
{
    var motor = DatosIniciales.CrearMotor();
    DebeLanzar<ArgumentOutOfRangeException>(() => motor.Calificar(1, 1, 6));
}

static void ProbarUsuarioInexistente()
{
    var motor = DatosIniciales.CrearMotor();
    DebeLanzar<KeyNotFoundException>(() => motor.GenerarRecomendaciones(999, 3));
}

static void ProbarCantidadInvalida()
{
    var motor = DatosIniciales.CrearMotor();
    DebeLanzar<ArgumentOutOfRangeException>(() => motor.GenerarRecomendaciones(1, 0));
}
