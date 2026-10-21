using System.Collections;

namespace CineMatch.Estructuras;

// Tabla hash de claves enteras. Resuelve colisiones mediante encadenamiento.
public sealed class TablaHash<T> : IEnumerable<T>
{
    private sealed class Entrada
    {
        public int Clave { get; }
        public T Valor { get; }
        public Entrada? Siguiente { get; set; }

        public Entrada(int clave, T valor, Entrada? siguiente)
        {
            Clave = clave;
            Valor = valor;
            Siguiente = siguiente;
        }
    }

    private Entrada?[] celdas;
    public int Cantidad { get; private set; }

    public TablaHash(int capacidad = 17)
    {
        if (capacidad < 2) throw new ArgumentOutOfRangeException(nameof(capacidad));
        celdas = new Entrada?[capacidad];
    }

    private int Indice(int clave) => (int)((uint)clave % (uint)celdas.Length);

    public bool IntentarObtener(int clave, out T valor)
    {
        for (var actual = celdas[Indice(clave)]; actual is not null; actual = actual.Siguiente)
        {
            if (actual.Clave == clave)
            {
                valor = actual.Valor;
                return true;
            }
        }

        valor = default!;
        return false;
    }

    public void Agregar(int clave, T valor)
    {
        if (IntentarObtener(clave, out _))
            throw new InvalidOperationException($"La clave {clave} ya está registrada.");

        if ((Cantidad + 1.0) / celdas.Length > 0.75)
            Redimensionar();

        var indice = Indice(clave);
        celdas[indice] = new Entrada(clave, valor, celdas[indice]);
        Cantidad++;
    }

    private void Redimensionar()
    {
        var anteriores = celdas;
        celdas = new Entrada?[SiguientePrimo(celdas.Length * 2 + 1)];

        foreach (var entrada in anteriores)
        {
            var actual = entrada;
            while (actual is not null)
            {
                var siguiente = actual.Siguiente;
                var indice = Indice(actual.Clave);
                actual.Siguiente = celdas[indice];
                celdas[indice] = actual;
                actual = siguiente;
            }
        }
    }

    private static int SiguientePrimo(int n)
    {
        while (true)
        {
            var primo = true;
            for (var divisor = 2; divisor <= n / divisor; divisor++)
            {
                if (n % divisor == 0)
                {
                    primo = false;
                    break;
                }
            }
            if (primo) return n;
            n++;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var entrada in celdas)
        {
            for (var actual = entrada; actual is not null; actual = actual.Siguiente)
                yield return actual.Valor;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
