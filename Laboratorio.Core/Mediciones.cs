using System.Diagnostics;

namespace Laboratorio.Core;

public sealed record Medicion(string Estrategia, double Milisegundos, long Bytes, long Suma);
public static class Mediciones
{
    public static IReadOnlyList<Medicion> Ejecutar(int n)
    {
        if (n < 1 || n > 10000) throw new ArgumentOutOfRangeException(nameof(n), "Usa entre 1 y 10000 elementos para medir.");
        return new[] { Medir("Arreglo preasignado", () => Llenar(n, 0)),
            Medir("Resize por elemento", () => Llenar(n, 1)),
            Medir("List con crecimiento", () => Llenar(n, 2)),
            Medir("List con capacidad inicial", () => Llenar(n, 3)) };
    }
    private static long Llenar(int n, int modo)
    {
        if (modo < 2)
        {
            int[] datos = modo == 0 ? new int[n] : Array.Empty<int>();
            for (int i = 0; i < n; i++) { if (modo == 1) Array.Resize(ref datos, i + 1); datos[i] = i; }
            long suma = 0; foreach (int v in datos) suma += v; return suma;
        }
        var lista = modo == 3 ? new List<int>(n) : new List<int>();
        for (int i = 0; i < n; i++) lista.Add(i);
        long total = 0; foreach (int v in lista) total += v; return total;
    }
    private static Medicion Medir(string nombre, Func<long> operacion)
    {
        long suma = 0;
        for (int i = 0; i < 3; i++) suma = operacion();
        var tiempos = new double[7]; var bytes = new long[7]; var reloj = new Stopwatch();
        for (int i = 0; i < 7; i++)
        {
            long antes = GC.GetAllocatedBytesForCurrentThread();
            reloj.Restart(); suma = operacion(); reloj.Stop();
            bytes[i] = GC.GetAllocatedBytesForCurrentThread() - antes;
            tiempos[i] = reloj.Elapsed.TotalMilliseconds;
        }
        Array.Sort(tiempos); Array.Sort(bytes);
        return new(nombre, tiempos[3], bytes[3], suma);
    }
}
