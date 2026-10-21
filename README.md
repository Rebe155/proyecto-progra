# CineMatch — Fase 2 (avance funcional)

Proyecto académico de Programación con Estructuras de Datos (PED941).
Implementación en C# de un motor de recomendación de películas mediante filtrado colaborativo basado en usuarios.

## Requisitos

- Visual Studio 2026 con la carga de trabajo **Desarrollo de escritorio de .NET**.
- SDK de **.NET 10** (incluido o instalable desde Visual Studio Installer).

## Cómo abrir

1. Extraer esta carpeta.
2. Abrir `CineMatch.sln` en Visual Studio 2026.
3. En el Explorador de soluciones, hacer clic derecho sobre `CineMatch` > **Establecer como proyecto de inicio**.
4. Ejecutar con **Ctrl+F5**.
5. Alternativamente, desde una terminal con el SDK de .NET:

   ```bash
   dotnet run --project CineMatch/CineMatch.csproj
   ```

## Datos de demostración

Incluye seis películas y cinco usuarios con valoraciones precargadas.
- **Usuario 1 (Ana):** generar 3 recomendaciones para observar filtrado colaborativo. Ya calificó las películas 1 y 2, que no deben recomendarse.
- **Usuario 5 (Sofía):** generar 2 recomendaciones para comprobar el mecanismo de respaldo: no tiene calificaciones.
- **Usuario 4 (Carlos):** calificó una película que nadie más ha calificado y también debe obtener respaldo.

Las calificaciones están en una escala de 1 a 5. La actualización de una calificación reemplaza la anterior.

## Estructuras de datos

| Clase | Función |
|---|---|
| `ListaEnlazada<T>` | Lista simplemente enlazada implementada con nodos propios: historial de calificaciones y lista de recomendaciones. |
| `TablaHash<T>` | Hash modular de claves enteras, encadenamiento para colisiones y redimensionamiento. |
| `ArbolAvl` | Índice de películas ordenado por título e ID, con rotaciones para mantener el balance. |
| `ColaPrioridad<T>` | Montículo máximo con estabilidad ante prioridades iguales. Se usa para vecinos y películas candidatas. |
| `GrafoValoraciones` | Grafo bipartito ponderado Usuario–Película mediante listas de adyacencia. |

## Algoritmo

1. Localizar usuario por tabla hash.
2. Identificar usuarios que calificaron alguna de sus mismas películas mediante grafo.
3. Calcular similitud del coseno usando únicamente calificaciones comunes.
4. Extraer hasta K=3 usuarios similares de una cola de prioridad.
5. Tomar películas que esos usuarios calificaron con 4 o 5 y que el usuario actual no calificó.
6. Calcular puntuación ponderada: suma(similitud * calificación) / suma(similitudes).
7. Priorizar recomendaciones con un montículo máximo.
8. Si no hay candidatos adecuados, utilizar el promedio de calificación del catálogo.

## Pruebas

Las pruebas son ejecutables sin dependencias de NuGet ni paquetes de terceros. Para ejecutarlas:

1. En Visual Studio, clic derecho sobre `CineMatch.Pruebas` > **Establecer como proyecto de inicio**.
2. Presionar **Ctrl+F5**.
3. Revisar las líneas `[OK]` y el conteo final. Registrar resultados reales en `documentacion/CASOS_DE_PRUEBA.md`.

O con terminal:

```bash
dotnet run --project CineMatch.Pruebas/CineMatch.Pruebas.csproj
```

> Las pruebas están listas para ejecutarse, pero no se registran resultados de ejecución en el repositorio hasta haberlas corrido en un entorno con .NET instalado.

## Estructura

```text
CineMatch.sln
CineMatch/
  Program.cs
  Modelos/             # Usuario, Pelicula, Calificacion, Recomendacion
  Estructuras/         # Lista, Hash, AVL, Cola de prioridad, Grafo
  Servicios/           # MotorRecomendacion
  Datos/               # DatosIniciales
CineMatch.Pruebas/     # Casos automatizados en consola
README.md
.gitignore
documentacion/
  CASOS_DE_PRUEBA.md
  BITACORA.md
  GUIA_FASE_2.md
```

## Limitaciones actuales

- Los datos están en memoria: se pierden los cambios al cerrar el programa.
- No cuenta con interfaz gráfica ni persistencia.
- El grafo tiene una implementación inicial; debe revisarse cuando se habilite la unidad correspondiente.
- El catálogo pequeño es una muestra para la demostración, no una base de datos real.
- La similitud del coseno con una sola película compartida puede ser poco discriminante. El diseño aprobado permite esa coincidencia mínima; se documenta como mejora futura.

## Git

```bash
git init
git add .
git commit -m "chore: crear estructura base de CineMatch"
```

Realizar commits sucesivos al implementar, probar y documentar cambios reales. No representar todos los avances con un solo commit ni simular un historial anterior.
