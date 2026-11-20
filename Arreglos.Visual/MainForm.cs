using Laboratorio.Core;
namespace Arreglos.Visual;

public partial class MainForm : Form
{
    private readonly ArreglosLab laboratorio = new();
    public MainForm()
    {
        InitializeComponent();
        generar.Click += (_, _) => Operar(() => {
            laboratorio.Generar((int)cantidad.Value, (int)minimo.Value, (int)maximo.Value, (int)semilla.Value);
            Registrar($"Generado arreglo de {cantidad.Value} elementos. El alias apunta a ese arreglo; List contiene una copia.");
        });
        redimensionar.Click += (_, _) => Operar(() => {
            laboratorio.Redimensionar((int)nuevaLongitud.Value);
            Registrar($"Redimensionado a {nuevaLongitud.Value}; el alias conserva el objeto anterior.");
        });
        cambiar.Click += (_, _) => Operar(() => {
            laboratorio.Cambiar((int)indice.Value, (int)valor.Value); Registrar($"Arreglo[{indice.Value}] = {valor.Value}.");
        });
        agregar.Click += (_, _) => Operar(() => { laboratorio.Agregar((int)valor.Value); Registrar($"Agregado {valor.Value} a List."); });
        quitar.Click += (_, _) => Operar(() => { laboratorio.QuitarUltimo(); Registrar("Eliminado el último elemento de List."); });
        medir.Click += async (_, _) => await MedirAsync();
        Actualizar();
    }
    private void Operar(Action accion)
    {
        try { accion(); Actualizar(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or OverflowException)
        { MessageBox.Show(this, ex.Message, "Revisa los datos", MessageBoxButtons.OK, MessageBoxIcon.Information); }
    }
    private void Actualizar()
    {
        datos.Rows.Clear();
        var actual = laboratorio.Actual; var alias = laboratorio.Alias; var vector = laboratorio.Vector;
        int total = Math.Max(actual.Count, Math.Max(alias.Count, vector.Count));
        for (int i = 0; i < Math.Min(250, total); i++)
            datos.Rows.Add(i, i < actual.Count ? actual[i].ToString() : "", i < alias.Count ? alias[i].ToString() : "", i < vector.Count ? vector[i].ToString() : "");
        estado.Text = $"Arreglo: {actual.Count}    |    Alias: {alias.Count}    |    List: {vector.Count} / capacidad {laboratorio.Capacidad}";
        identidad.Text = $"Arreglo y alias: {(laboratorio.MismaInstancia ? "misma instancia" : "instancias diferentes")}. Vista: primeros 250 elementos como máximo.";
    }
    private void Registrar(string mensaje) => bitacora.AppendText($"{DateTime.Now:HH:mm:ss}  {mensaje}{Environment.NewLine}");
    private async Task MedirAsync()
    {
        medir.Enabled = false; medir.Text = "Midiendo…";
        try
        {
            int n = (int)tamanoMedicion.Value;
            var resultados = await Task.Run(() => Mediciones.Ejecutar(n));
            mediciones.Rows.Clear();
            foreach (var r in resultados) mediciones.Rows.Add(r.Estrategia, r.Milisegundos.ToString("F4"), r.Bytes.ToString("N0"), r.Suma);
            Registrar($"Medición real con n={n}: 3 calentamientos y mediana de 7 muestras. Los bytes son asignaciones acumuladas del hilo.");
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo medir"); }
        finally { medir.Enabled = true; medir.Text = "Comparar estrategias"; }
    }
}
