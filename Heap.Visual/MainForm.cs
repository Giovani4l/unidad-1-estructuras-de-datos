using Laboratorio.Core;
namespace Heap.Visual;

public partial class MainForm : Form
{
    private readonly HeapLab laboratorio = new();
    public MainForm()
    {
        InitializeComponent();
        crear.Click += (_, _) => Operar(() => {
            laboratorio.Crear(nombre.Text, titular.Text, saldo.Value); Registrar($"{nombre.Text.Trim()} apunta a una nueva cuenta.");
        });
        alias.Click += (_, _) => Operar(() => {
            laboratorio.CrearAlias(nombre.Text, origen.Text); Registrar($"{nombre.Text.Trim()} comparte el objeto de {origen.Text}.");
        });
        depositar.Click += (_, _) => Operar(() => { laboratorio.Obtener(origen.Text).Depositar(importe.Value); Registrar($"Depósito de {importe.Value:N2} mediante {origen.Text}."); });
        retirar.Click += (_, _) => Operar(() => { laboratorio.Obtener(origen.Text).Retirar(importe.Value); Registrar($"Retiro de {importe.Value:N2} mediante {origen.Text}."); });
        soltar.Click += (_, _) => Operar(() => { laboratorio.Soltar(origen.Text); Registrar($"{origen.Text} = null. El GC decide cuándo recuperar objetos inalcanzables."); });
        comparar.Click += (_, _) => Operar(() => {
            bool misma = laboratorio.MismaInstancia(origen.Text, segunda.Text);
            Registrar($"ReferenceEquals({origen.Text}, {segunda.Text}) = {misma}.");
            MessageBox.Show(this, misma ? "Ambas referencias apuntan al mismo objeto." : "Son objetos diferentes.", "Identidad de las instancias");
        });
        Actualizar();
    }
    private void Operar(Action accion)
    {
        try { accion(); Actualizar(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or OverflowException)
        { MessageBox.Show(this, ex.Message, "Revisa los datos", MessageBoxButtons.OK, MessageBoxIcon.Information); }
    }
    private void Registrar(string mensaje) => bitacora.AppendText($"{DateTime.Now:HH:mm:ss}  {mensaje}{Environment.NewLine}");
    private void Actualizar()
    {
        datos.Rows.Clear(); string anterior = origen.Text, otra = segunda.Text;
        origen.Items.Clear(); segunda.Items.Clear();
        foreach (string referencia in laboratorio.Nombres)
        {
            var cuenta = laboratorio.Consultar(referencia);
            datos.Rows.Add(referencia, cuenta?.Id.ToString() ?? "null", cuenta?.Titular ?? "Sin objeto", cuenta?.Saldo.ToString("N2") ?? "—");
            origen.Items.Add(referencia); segunda.Items.Add(referencia);
        }
        if (origen.Items.Contains(anterior)) origen.SelectedItem = anterior;
        else if (origen.Items.Count > 0) origen.SelectedIndex = 0;
        if (segunda.Items.Contains(otra)) segunda.SelectedItem = otra;
        else if (segunda.Items.Count > 0) segunda.SelectedIndex = segunda.Items.Count - 1;
        estado.Text = $"Referencias: {origen.Items.Count}    |    Instancias distintas alcanzables: {laboratorio.InstanciasAlcanzables}";
    }
}
