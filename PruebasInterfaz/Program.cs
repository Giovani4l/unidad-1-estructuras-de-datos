using System.Reflection;
using System.Drawing.Imaging;

// Pruebas de integración dentro del proceso: no controlan otras aplicaciones.
internal static class Program
{
    private static int aprobadas;
    private static T Campo<T>(Form form, string nombre) where T : class =>
        (T)(form.GetType().GetField(nombre, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(form)
            ?? throw new InvalidOperationException(nombre));
    private static void Click(Form form, string nombre)
    {
        var boton = Campo<Button>(form, nombre);
        typeof(Button).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(boton, new object[] { EventArgs.Empty });
        Application.DoEvents();
    }
    private static void Check(bool ok, string nombre)
    {
        if (!ok) throw new InvalidOperationException("FALLO: " + nombre);
        Console.WriteLine("PASS " + nombre); aprobadas++;
    }
    private static void Render(Form form, string archivo)
    {
        form.Show();
        Application.DoEvents();
        form.PerformLayout();
        foreach (var panel in form.Controls.OfType<TableLayoutPanel>().SelectMany(t => t.Controls.OfType<FlowLayoutPanel>()))
            foreach (Control control in panel.Controls)
                Check(control.Right <= panel.ClientSize.Width, "control dentro de fila: " + control.GetType().Name);
        using var bitmap = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
        bitmap.Save(archivo, ImageFormat.Png);
        form.Hide();
    }
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        string carpeta = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(Environment.CurrentDirectory, "renders");
        Directory.CreateDirectory(carpeta);
        using var arreglos = new Arreglos.Visual.MainForm();
        Campo<NumericUpDown>(arreglos, "cantidad").Value = 6;
        Campo<NumericUpDown>(arreglos, "minimo").Value = 13;
        Campo<NumericUpDown>(arreglos, "maximo").Value = 13;
        Click(arreglos, "generar");
        var tabla = Campo<DataGridView>(arreglos, "datos");
        Check(tabla.Rows.Count == 6 && tabla.Rows[0].Cells[1].Value?.ToString() == "13", "botón Generar usa entradas");
        Campo<NumericUpDown>(arreglos, "nuevaLongitud").Value = 9;
        Click(arreglos, "redimensionar");
        Check(tabla.Rows.Count == 9 && tabla.Rows[8].Cells[1].Value?.ToString() == "0", "botón Redimensionar crea tamaño indicado");
        Campo<NumericUpDown>(arreglos, "indice").Value = 2;
        Campo<NumericUpDown>(arreglos, "valor").Value = 91;
        Click(arreglos, "cambiar");
        Check(tabla.Rows[2].Cells[1].Value?.ToString() == "91" && tabla.Rows[2].Cells[2].Value?.ToString() == "13", "edición real independiente del alias anterior");
        Click(arreglos, "agregar");
        Check(tabla.Rows[6].Cells[3].Value?.ToString() == "91", "botón agrega valor ingresado a List");
        Click(arreglos, "quitar");
        Check(tabla.Rows[6].Cells[3].Value?.ToString() == "", "botón elimina último de List");
        Campo<NumericUpDown>(arreglos, "tamanoMedicion").Value = 137;
        Click(arreglos, "medir");
        var resultados = Campo<DataGridView>(arreglos, "mediciones");
        var limite = DateTime.UtcNow.AddSeconds(20);
        while (resultados.Rows.Count != 4 && DateTime.UtcNow < limite) { Application.DoEvents(); Thread.Sleep(10); }
        Check(resultados.Rows.Count == 4 && resultados.Rows[0].Cells[3].Value?.ToString() == "9316", "botón medir calcula las cuatro estrategias");
        Render(arreglos, Path.Combine(carpeta, "Arreglos_formulario.png"));

        using var heap = new Heap.Visual.MainForm();
        Campo<TextBox>(heap, "nombre").Text = "principal";
        Campo<TextBox>(heap, "titular").Text = "Cuenta de demostración";
        Campo<NumericUpDown>(heap, "saldo").Value = 250.50m;
        Click(heap, "crear");
        Campo<TextBox>(heap, "nombre").Text = "compartida";
        Click(heap, "alias");
        Campo<ComboBox>(heap, "origen").SelectedItem = "compartida";
        Campo<NumericUpDown>(heap, "importe").Value = 49.50m;
        Click(heap, "depositar");
        var cuentas = Campo<DataGridView>(heap, "datos");
        Check(cuentas.Rows.Count == 2 && cuentas.Rows[0].Cells[3].Value?.ToString() == cuentas.Rows[1].Cells[3].Value?.ToString(), "depósito actualiza ambas referencias");
        Check(Campo<Label>(heap, "estado").Text.Contains("alcanzables: 1"), "formulario reconoce una instancia compartida");
        Campo<NumericUpDown>(heap, "importe").Value = 20;
        Click(heap, "retirar");
        Check(cuentas.Rows[0].Cells[3].Value?.ToString() == 280m.ToString("N2"), "retiro calculado a partir del estado actual");
        Render(heap, Path.Combine(carpeta, "Heap_formulario.png"));
        Click(heap, "soltar");
        Check(cuentas.Rows[1].Cells[1].Value?.ToString() == "null" && cuentas.Rows[0].Cells[3].Value?.ToString() == 280m.ToString("N2"), "soltar elimina solo la referencia seleccionada");
        Console.WriteLine($"RESULTADO: {aprobadas} comprobaciones de interfaz aprobadas");
        Console.WriteLine("Las imágenes son renders del formulario mediante DrawToBitmap, no capturas del escritorio.");
    }
}
