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
        private List<string> _archivosSeleccionados = new List<string>();

        public Form1()
        {
            InitializeComponent();
            CargarAlgoritmos();
        }

        private void CargarAlgoritmos()
        {
            comboAlgoritmo.Items.Clear();
            comboAlgoritmo.Items.Add("Huffman");
            comboAlgoritmo.Items.Add("LZ77");
            comboAlgoritmo.Items.Add("LZ78");
            comboAlgoritmo.SelectedIndex = 0;
        }

        private ICompressor ObtenerCompresorSeleccionado()
        {
            string nombre = comboAlgoritmo.SelectedItem.ToString();

            switch (nombre)
            {
                case "Huffman": return new HuffmanCompresor();
                case "LZ77":    return new LZ77Compresor();
                case "LZ78":    return new LZ78Compresor();
                default: throw new Exception("Algoritmo no válido.");
            }
        }

        private byte ObtenerIdAlgoritmo()
        {
            string nombre = comboAlgoritmo.SelectedItem.ToString();
            switch (nombre)
            {
                case "Huffman": return 1;
                case "LZ77":    return 2;
                case "LZ78":    return 3;
                default:        return 0;
            }
        }

        private ICompressor ObtenerCompresorPorId(byte id)
        {
            switch (id)
            {
                case 1: return new HuffmanCompresor();
                case 2: return new LZ77Compresor();
                case 3: return new LZ78Compresor();
                default: throw new Exception("ID de algoritmo no válido.");
            }
        }

        private void archBuscar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog selector = new OpenFileDialog())
            {
                selector.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                selector.Multiselect = true;

                if (selector.ShowDialog() == DialogResult.OK)
                {
                    _archivosSeleccionados = selector.FileNames.ToList();

                    ArchivoSeleccionado.Text = _archivosSeleccionados.Count == 1
                        ? _archivosSeleccionados[0]
                        : $"{_archivosSeleccionados.Count} archivos seleccionados";
                }
            }
        }

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
            est.MemoriaUsada       = memDespues - memAntes;

            return resultado;
        }

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
            est.MemoriaUsada       = memDespues - memAntes;

            return resultado;
        }

        private int CalcularTamanoDatosHuffman(byte[] paquete)
        {
            if (paquete == null || paquete.Length < 4)
                return 0;

            int offset = 0;

            int N = BitConverter.ToInt32(paquete, offset);
            offset += 4;

            offset += N * (2 + 4);

            if (offset + 4 > paquete.Length)
                return 0;

            int cantidadBits = BitConverter.ToInt32(paquete, offset);
            offset += 4;

            if (offset > paquete.Length)
                return 0;

            int bytesDatos = paquete.Length - offset;
            return bytesDatos;
        }

        private void MostrarEstadisticas(EstadisticasCompresor est, string operacion)
        {
            Tiempo.Text  = $"{operacion} - Tiempo: {est.TiempoTranscurrido.TotalMilliseconds:F4} ms";
            Memoria.Text = $"{operacion} - Memoria: {est.MemoriaUsada} bytes";
            Tasa.Text    = $"{operacion} - Tasa: {est.RadioDeCompresion:F2}:1";
        }

        private void Compresor_Click_1(object sender, EventArgs e)
        {
            if (_archivosSeleccionados.Count == 0)
            {
                MessageBox.Show("Seleccione al menos un archivo.");
                return;
            }

            ICompressor compresor = ObtenerCompresorSeleccionado();
            bool esHuffman = compresor is HuffmanCompresor;
            byte idAlg = ObtenerIdAlgoritmo();
            MiZipFile zip = MiZipFile.CrearVacio(idAlg);

            long totalOriginal   = 0;
            long totalComprimido = 0;
            long maxMemoria      = 0;
            TimeSpan tiempoTotal = TimeSpan.Zero;

            foreach (var ruta in _archivosSeleccionados)
            {
                string textoOriginal = File.ReadAllText(ruta);
                long tamanoOriginal  = new FileInfo(ruta).Length;

                EstadisticasCompresor estArchivo;
                byte[] paquete = EjecutarCompresionConEstadisticas(
                    compresor, textoOriginal, tamanoOriginal, out estArchivo);

                zip.AgregarEntrada(Path.GetFileName(ruta), tamanoOriginal, paquete);

                int tamanoComprimido;
                if (esHuffman)
                    tamanoComprimido = CalcularTamanoDatosHuffman(paquete);
                else
                    tamanoComprimido = paquete.Length;

                totalOriginal   += tamanoOriginal;
                totalComprimido += tamanoComprimido;
                tiempoTotal     += estArchivo.TiempoTranscurrido;

                if (estArchivo.MemoriaUsada > maxMemoria)
                    maxMemoria = estArchivo.MemoriaUsada;
            }

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

            double ratio = totalComprimido == 0 ? 0 : totalOriginal / (double)totalComprimido;

            var estGlobal = new EstadisticasCompresor
            {
                TiempoTranscurrido = tiempoTotal,
                MemoriaUsada       = maxMemoria,
                RadioDeCompresion  = ratio
            };

            MostrarEstadisticas(estGlobal, "Compresión");
            MessageBox.Show("Archivos comprimidos correctamente.");
        }

        private void Descompresor_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog selector = new OpenFileDialog())
            {
                selector.Filter = "Archivo MyZip (*.myzip)|*.myzip";

                if (selector.ShowDialog() != DialogResult.OK)
                    return;

                MiZipFile zip = MiZipFile.CargarDesdeArchivo(selector.FileName);
                ICompressor compresor = ObtenerCompresorPorId(zip.IdAlgoritmo);
                bool esHuffman = compresor is HuffmanCompresor;

                string carpetaSalida = Path.GetDirectoryName(selector.FileName);

                long totalOriginal   = 0;
                long totalComprimido = 0;
                long maxMemoria      = 0;
                TimeSpan tiempoTotal = TimeSpan.Zero;

                foreach (var entrada in zip.Entradas)
                {
                    EstadisticasCompresor estArchivo;
                    string texto = EjecutarDescompresionConEstadisticas(
                        compresor, entrada.DatosComprimidos, out estArchivo);

                    string rutaSalida = Path.Combine(
                        carpetaSalida,
                        entrada.NombreArchivoOriginal + ".Descomprimido.txt");

                    File.WriteAllText(rutaSalida, texto);

                    int tamanoComprimido;
                    if (esHuffman)
                        tamanoComprimido = CalcularTamanoDatosHuffman(entrada.DatosComprimidos);
                    else
                        tamanoComprimido = entrada.DatosComprimidos.Length;

                    totalOriginal   += entrada.TamanoOriginalBytes;
                    totalComprimido += tamanoComprimido;
                    tiempoTotal     += estArchivo.TiempoTranscurrido;

                    if (estArchivo.MemoriaUsada > maxMemoria)
                        maxMemoria = estArchivo.MemoriaUsada;
                }

                double ratio = totalComprimido == 0 ? 0 : totalOriginal / (double)totalComprimido;

                var estGlobal = new EstadisticasCompresor
                {
                    TiempoTranscurrido = tiempoTotal,
                    MemoriaUsada       = maxMemoria,
                    RadioDeCompresion  = ratio
                };

                MostrarEstadisticas(estGlobal, "Descompresión");
                MessageBox.Show("Archivos descomprimidos correctamente.");
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
    }
}


