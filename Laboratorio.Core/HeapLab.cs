namespace Laboratorio.Core;

/// <summary>Objeto de ejemplo: solo sus operaciones pueden cambiar el saldo.</summary>
public sealed class Cuenta
{
    public int Id { get; }
    public string Titular { get; }
    public decimal Saldo { get; private set; }
    internal Cuenta(int id, string titular, decimal saldo)
    {
        if (string.IsNullOrWhiteSpace(titular)) throw new ArgumentException("Escribe el titular.");
        if (saldo < 0) throw new ArgumentException("El saldo inicial no puede ser negativo.");
        Id = id; Titular = titular.Trim(); Saldo = saldo;
    }
    public void Depositar(decimal importe)
    {
        if (importe <= 0) throw new ArgumentException("El depósito debe ser mayor que cero.");
        Saldo = checked(Saldo + importe);
    }
    public void Retirar(decimal importe)
    {
        if (importe <= 0 || importe > Saldo) throw new ArgumentException("El retiro debe ser positivo y no superar el saldo.");
        Saldo -= importe;
    }
}

public sealed class HeapLab
{
    private readonly Dictionary<string, Cuenta?> referencias = new(StringComparer.OrdinalIgnoreCase);
    private int siguienteId = 1;
    public IEnumerable<string> Nombres => referencias.Keys;
    public int InstanciasAlcanzables => referencias.Values.Where(x => x is not null).Distinct().Count();
    public Cuenta? Consultar(string nombre) => referencias.TryGetValue(nombre, out var c) ? c : null;
    private static string Nombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length > 30)
            throw new ArgumentException("Escribe un nombre de referencia de 1 a 30 caracteres.");
        return nombre.Trim();
    }
    public void Crear(string referencia, string titular, decimal saldo)
    {
        string nombre = Nombre(referencia);
        var nueva = new Cuenta(siguienteId, titular, saldo);
        referencias[nombre] = nueva; siguienteId++;
    }
    public void CrearAlias(string nuevaReferencia, string origen)
    {
        string nombre = Nombre(nuevaReferencia);
        referencias[nombre] = Obtener(origen);
    }
    public Cuenta Obtener(string referencia) => Consultar(referencia)
        ?? throw new InvalidOperationException("La referencia seleccionada no apunta a un objeto.");
    public void Soltar(string referencia)
    {
        if (!referencias.ContainsKey(referencia)) throw new ArgumentException("Selecciona una referencia existente.");
        referencias[referencia] = null;
    }
    public bool MismaInstancia(string primera, string segunda) => ReferenceEquals(Obtener(primera), Obtener(segunda));
}
