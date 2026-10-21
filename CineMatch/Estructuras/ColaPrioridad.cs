namespace CineMatch.Estructuras;

// Montículo máximo. Si dos prioridades son iguales, conserva el orden de llegada.
public sealed class ColaPrioridad<T>
{
    private sealed class Elemento
    {
        public T Dato { get; }
        public double Prioridad { get; }
        public long Orden { get; }

        public Elemento(T dato, double prioridad, long orden)
        {
            Dato = dato;
            Prioridad = prioridad;
            Orden = orden;
        }
    }

    private readonly List<Elemento> elementos = new();
    private long siguienteOrden;
    public int Cantidad => elementos.Count;
    public bool EstaVacia => elementos.Count == 0;

    public void Encolar(T dato, double prioridad)
    {
        if (double.IsNaN(prioridad) || double.IsInfinity(prioridad))
            throw new ArgumentException("La prioridad debe ser un número finito.", nameof(prioridad));

        elementos.Add(new Elemento(dato, prioridad, siguienteOrden++));
        var indice = elementos.Count - 1;
        while (indice > 0)
        {
            var padre = (indice - 1) / 2;
            if (!TieneMayorPrioridad(elementos[indice], elementos[padre])) break;
            (elementos[indice], elementos[padre]) = (elementos[padre], elementos[indice]);
            indice = padre;
        }
    }

    public T Desencolar()
    {
        if (EstaVacia) throw new InvalidOperationException("La cola está vacía.");

        var resultado = elementos[0].Dato;
        var ultimo = elementos[^1];
        elementos.RemoveAt(elementos.Count - 1);
        if (EstaVacia) return resultado;

        elementos[0] = ultimo;
        var indice = 0;
        while (true)
        {
            var izquierdo = indice * 2 + 1;
            var derecho = izquierdo + 1;
            var mayor = indice;

            if (izquierdo < elementos.Count && TieneMayorPrioridad(elementos[izquierdo], elementos[mayor]))
                mayor = izquierdo;
            if (derecho < elementos.Count && TieneMayorPrioridad(elementos[derecho], elementos[mayor]))
                mayor = derecho;
            if (mayor == indice) break;

            (elementos[indice], elementos[mayor]) = (elementos[mayor], elementos[indice]);
            indice = mayor;
        }
        return resultado;
    }

    private static bool TieneMayorPrioridad(Elemento a, Elemento b) =>
        a.Prioridad > b.Prioridad || (a.Prioridad == b.Prioridad && a.Orden < b.Orden);
}
