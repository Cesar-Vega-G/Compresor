using Compresor.Algoritmos;
using Compresor.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Compresor
{
    public partial class Form1 : Form
    {
        // lista con todos los archivos seleccionados para comprimir
        private List<string> _archivosSeleccionados = new List<string>();

        public Form1()
        {
            InitializeComponent();
            CargarAlgoritmos();
        }

        // ================================================
        // CARGAR ALGORITMOS EN EL COMBO
        // ================================================
        private void CargarAlgoritmos()
        {
            comboAlgoritmo.Items.Clear();
            comboAlgoritmo.Items.Add("Huffman");
            comboAlgoritmo.Items.Add("LZ77");
            comboAlgoritmo.Items.Add("LZ78");
            comboAlgoritmo.SelectedIndex = 0;
        }

        // ================================================
        // DEVOLVER OBJETO COMPRESOR SEGÚN COMBO
        // ================================================
        private ICompressor ObtenerCompresorSeleccionado()
        {
            string nombre = comboAlgoritmo.SelectedItem.ToString();

            switch (nombre)
            {
                case "Huffman": return new HuffmanCompresor();
                case "LZ77": return new LZ77Compresor();
                case "LZ78": return new LZ78Compresor();
                default: throw new Exception("Algoritmo inválido.");
            }
        }

        // ================================================
        // DEVOLVER ID DE ALGORITMO PARA .MYZIP
        // ================================================
        private byte ObtenerIdAlgoritmo()
        {
            string nombre = comboAlgoritmo.SelectedItem.ToString();

            switch (nombre)
            {
                case "Huffman": return 1;
                case "LZ77": return 2;
                case "LZ78": return 3;
                default: return 0;
            }
        }

        // ID -> objeto compresor (para descomprimir)
        private ICompressor ObtenerCompresorPorId(byte id)
        {
            switch (id)
            {
                case 1: return new HuffmanCompresor();
                case 2: return new LZ77Compresor();
                case 3: return new LZ78Compresor();
                default: throw new Exception("ID de algoritmo inválido.");
            }
        }

        // ================================================
        // BOTÓN BUSCAR ARCHIVO  (archBuscar)
        // AHORA PERMITE SELECCIONAR VARIOS .TXT
        // ================================================
        private void archBuscar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog selector = new OpenFileDialog())
            {
                selector.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                selector.Multiselect = true;

                if (selector.ShowDialog() == DialogResult.OK)
                {
                    _archivosSeleccionados = selector.FileNames.ToList();

                    if (_archivosSeleccionados.Count == 1)
                    {
                        ArchivoSeleccionado.Text = _archivosSeleccionados[0];
                    }
                    else
                    {
                        // solo para que se vea algo en el TextBox
                        ArchivoSeleccionado.Text =
                            $"{_archivosSeleccionados.Count} archivos seleccionados";
                    }
                }
            }
        }

        // ================================================
        // EJECUTAR COMPRESIÓN CON ESTADÍSTICAS
        // ================================================
        private byte[] EjecutarCompresionConEstadisticas(
            ICompressor compresor,
            string texto,
            long tamanoOriginal,
            out EstadisticasCompresor est)
        {
            est = new EstadisticasCompresor();

            long memAntes = GC.GetTotalMemory(true);
            var reloj = System.Diagnostics.Stopwatch.StartNew();

            byte[] resultado = compresor.Comprimir(texto, out est);

            reloj.Stop();
            long memDespues = GC.GetTotalMemory(true);

            est.TiempoTranscurrido = reloj.Elapsed;
            est.MemoriaUsada = memDespues - memAntes;
            est.RadioDeCompresion = resultado.Length == 0
                ? 0
                : tamanoOriginal / (double)resultado.Length;

            return resultado;
        }

        // ================================================
        // EJECUTAR DESCOMPRESIÓN CON ESTADÍSTICAS
        // ================================================
        private string EjecutarDescompresionConEstadisticas(
            ICompressor compresor,
            byte[] datos,
            out EstadisticasCompresor est)
        {
            est = new EstadisticasCompresor();

            long memAntes = GC.GetTotalMemory(true);
            var reloj = System.Diagnostics.Stopwatch.StartNew();

            string resultado = compresor.Descomprimir(datos, out est);

            reloj.Stop();
            long memDespues = GC.GetTotalMemory(true);

            est.TiempoTranscurrido = reloj.Elapsed;
            est.MemoriaUsada = memDespues - memAntes;
            est.RadioDeCompresion = 0;

            return resultado;
        }

        // ================================================
        // MOSTRAR ESTADÍSTICAS EN LABELS
        // ================================================
        private void MostrarEstadisticas(EstadisticasCompresor est)
        {
            Tiempo.Text = $"Tiempo: {est.TiempoTranscurrido.TotalMilliseconds:F2} ms";
            Memoria.Text = $"Memoria: {est.MemoriaUsada} bytes";
            Tasa.Text = $"Tasa: {est.RadioDeCompresion:F2}:1";
        }

        // ================================================
        // BOTÓN COMPRIMIR  (Compresor)
        // AHORA COMPRIME TODOS LOS .TXT SELECCIONADOS
        // EN UN SOLO .MYZIP
        // ================================================
        private void Compresor_Click_1(object sender, EventArgs e)
        {
            if (_archivosSeleccionados == null || _archivosSeleccionados.Count == 0)
            {
                MessageBox.Show("Seleccione al menos un archivo de texto.");
                return;
            }

            ICompressor compresor = ObtenerCompresorSeleccionado();
            byte idAlg = ObtenerIdAlgoritmo();

            // Creamos el zip vacío
            MiZipFile zip = MiZipFile.CrearVacio(idAlg);

            long totalOriginal = 0;
            long totalComprimido = 0;
            long maxMemoria = 0;
            var tiempoTotal = TimeSpan.Zero;

            foreach (var ruta in _archivosSeleccionados)
            {
                if (!File.Exists(ruta)) continue;

                string textoOriginal = File.ReadAllText(ruta);
                long tamanoOriginal = new FileInfo(ruta).Length;

                EstadisticasCompresor estArchivo;
                byte[] datos = EjecutarCompresionConEstadisticas(
                    compresor, textoOriginal, tamanoOriginal, out estArchivo);

                zip.AgregarEntrada(
                    Path.GetFileName(ruta),
                    tamanoOriginal,
                    datos);

                totalOriginal += tamanoOriginal;
                totalComprimido += datos.Length;
                tiempoTotal += estArchivo.TiempoTranscurrido;
                if (estArchivo.MemoriaUsada > maxMemoria)
                    maxMemoria = estArchivo.MemoriaUsada;
            }

            // Pedimos al usuario dónde guardar el .myzip
            string salida;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Archivo MyZip (*.myzip)|*.myzip";
                dlg.FileName = "archivos.myzip";

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                salida = dlg.FileName;
            }

            zip.GuardarEnRuta(salida);

            // Estadísticas globales
            var estGlobal = new EstadisticasCompresor
            {
                TiempoTranscurrido = tiempoTotal,
                MemoriaUsada = maxMemoria,
                RadioDeCompresion = (totalComprimido == 0)
                    ? 0
                    : totalOriginal / (double)totalComprimido
            };

            MostrarEstadisticas(estGlobal);
            MessageBox.Show("Archivos comprimidos en: " + salida);
        }

        // ================================================
        // BOTÓN DESCOMPRIMIR  (Descompresor)
        // DESCOMPRIME TODAS LAS ENTRADAS DEL .MYZIP
        // ================================================
        private void Descompresor_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog selector = new OpenFileDialog())
            {
                selector.Filter = "Archivo MyZip (*.myzip)|*.myzip";

                if (selector.ShowDialog() != DialogResult.OK)
                    return;

                // Cargar todas las entradas del zip
                MiZipFile zip = MiZipFile.CargarDesdeArchivo(selector.FileName);

                ICompressor compresor = ObtenerCompresorPorId(zip.IdAlgoritmo);

                string carpetaSalida = Path.GetDirectoryName(selector.FileName);

                long maxMemoria = 0;
                var tiempoTotal = TimeSpan.Zero;

                foreach (var entrada in zip.Entradas)
                {
                    EstadisticasCompresor estArchivo;
                    string texto = EjecutarDescompresionConEstadisticas(
                        compresor,
                        entrada.DatosComprimidos,
                        out estArchivo);

                    string rutaSalida = Path.Combine(
                        carpetaSalida,
                        entrada.NombreArchivoOriginal + ".uncompressed.txt");

                    File.WriteAllText(rutaSalida, texto);

                    tiempoTotal += estArchivo.TiempoTranscurrido;
                    if (estArchivo.MemoriaUsada > maxMemoria)
                        maxMemoria = estArchivo.MemoriaUsada;
                }

                var estGlobal = new EstadisticasCompresor
                {
                    TiempoTranscurrido = tiempoTotal,
                    MemoriaUsada = maxMemoria,
                    RadioDeCompresion = 0 // en descompresión no aplica
                };

                MostrarEstadisticas(estGlobal);
                MessageBox.Show("Archivos descomprimidos en la misma carpeta del .myzip.");
            }
        }

        // HANDLERS VACÍOS QUE PIDE EL DISEÑADOR
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
    }
}




