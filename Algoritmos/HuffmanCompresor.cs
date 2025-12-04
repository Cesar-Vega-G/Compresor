using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compresor.Core;

namespace Compresor.Algoritmos
{
    internal class NodoHuffman
    {
        public char? Simbolo { get; set; }
        public int Frecuencia { get; set; }
        public NodoHuffman Izquierda { get; set; }
        public NodoHuffman Derecha { get; set; }

        public bool EsHoja => Izquierda == null && Derecha == null;
    }

    public class HuffmanCompresor : ICompressor
    {
        public string Nombre => "Huffman";

        public byte[] Comprimir(string textoOriginal, out EstadisticasCompresor estadisticas)
        {
            estadisticas = new EstadisticasCompresor();

            if (string.IsNullOrEmpty(textoOriginal))
                return Array.Empty<byte>();

            Dictionary<char, int> frecuencias = ContarFrecuencias(textoOriginal);
            NodoHuffman raiz = ConstruirArbol(frecuencias);
            Dictionary<char, string> tablaCodigos = new Dictionary<char, string>();
            ConstruirTablaCodigos(raiz, "", tablaCodigos);
            string bits = CodificarTexto(textoOriginal, tablaCodigos);
            return Empaquetar(frecuencias, bits);
        }

        public string Descomprimir(byte[] datosComprimidos, out EstadisticasCompresor estadisticas)
        {
            estadisticas = new EstadisticasCompresor();

            if (datosComprimidos == null || datosComprimidos.Length == 0)
                return string.Empty;

            Dictionary<char, int> frecuencias;
            string bits = Desempaquetar(datosComprimidos, out frecuencias);
            NodoHuffman raiz = ConstruirArbol(frecuencias);
            return DecodificarBits(bits, raiz);
        }

        private Dictionary<char, int> ContarFrecuencias(string texto)
        {
            var dict = new Dictionary<char, int>();
            foreach (char c in texto)
            {
                if (!dict.ContainsKey(c))
                    dict[c] = 0;
                dict[c]++;
            }
            return dict;
        }

        private NodoHuffman ConstruirArbol(Dictionary<char, int> frecuencias)
        {
            var cola = new List<NodoHuffman>();

            foreach (var par in frecuencias)
            {
                cola.Add(new NodoHuffman
                {
                    Simbolo = par.Key,
                    Frecuencia = par.Value
                });
            }

            while (cola.Count > 1)
            {
                cola = cola.OrderBy(n => n.Frecuencia).ToList();

                var primero = cola[0];
                var segundo = cola[1];
                cola.RemoveAt(0);
                cola.RemoveAt(0);

                var padre = new NodoHuffman
                {
                    Simbolo = null,
                    Frecuencia = primero.Frecuencia + segundo.Frecuencia,
                    Izquierda = primero,
                    Derecha = segundo
                };

                cola.Add(padre);
            }

            return cola[0];
        }

        private void ConstruirTablaCodigos(NodoHuffman nodo, string prefijo, Dictionary<char, string> tabla)
        {
            if (nodo.EsHoja && nodo.Simbolo.HasValue)
            {
                tabla[nodo.Simbolo.Value] = prefijo.Length == 0 ? "0" : prefijo;
                return;
            }

            if (nodo.Izquierda != null)
                ConstruirTablaCodigos(nodo.Izquierda, prefijo + "0", tabla);

            if (nodo.Derecha != null)
                ConstruirTablaCodigos(nodo.Derecha, prefijo + "1", tabla);
        }

        private string CodificarTexto(string texto, Dictionary<char, string> tabla)
        {
            var sb = new StringBuilder();
            foreach (char c in texto)
            {
                sb.Append(tabla[c]);
            }
            return sb.ToString();
        }

        private byte[] Empaquetar(Dictionary<char, int> frecuencias, string bits)
        {
            List<byte> salida = new List<byte>();

            int N = frecuencias.Count;
            salida.AddRange(BitConverter.GetBytes(N));

            foreach (var par in frecuencias)
            {
                byte[] charBytes = BitConverter.GetBytes(par.Key);
                salida.AddRange(charBytes);

                byte[] freqBytes = BitConverter.GetBytes(par.Value);
                salida.AddRange(freqBytes);
            }

            int cantidadBits = bits.Length;
            salida.AddRange(BitConverter.GetBytes(cantidadBits));

            List<byte> datos = BitsABytes(bits);
            salida.AddRange(datos);

            return salida.ToArray();
        }

        private string Desempaquetar(byte[] datos, out Dictionary<char, int> frecuencias)
        {
            frecuencias = new Dictionary<char, int>();
            int offset = 0;

            int N = BitConverter.ToInt32(datos, offset);
            offset += 4;

            for (int i = 0; i < N; i++)
            {
                char c = BitConverter.ToChar(datos, offset);
                offset += 2;

                int freq = BitConverter.ToInt32(datos, offset);
                offset += 4;

                frecuencias[c] = freq;
            }

            int cantidadBits = BitConverter.ToInt32(datos, offset);
            offset += 4;

            int cantidadBytesDatos = datos.Length - offset;
            byte[] soloDatos = new byte[cantidadBytesDatos];
            Array.Copy(datos, offset, soloDatos, 0, cantidadBytesDatos);

            string bits = BytesABits(soloDatos, cantidadBits);
            return bits;
        }

        private List<byte> BitsABytes(string bits)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < bits.Length; i += 8)
            {
                byte b = 0;
                int max = Math.Min(8, bits.Length - i);
                for (int j = 0; j < max; j++)
                {
                    if (bits[i + j] == '1')
                    {
                        b |= (byte)(1 << (7 - j));
                    }
                }
                bytes.Add(b);
            }

            return bytes;
        }

        private string BytesABits(byte[] bytes, int cantidadBits)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                for (int bit = 7; bit >= 0; bit--)
                {
                    int bitVal = (bytes[i] >> bit) & 1;
                    sb.Append(bitVal == 1 ? '1' : '0');

                    if (sb.Length == cantidadBits)
                        return sb.ToString();
                }
            }

            return sb.ToString();
        }

        private string DecodificarBits(string bits, NodoHuffman raiz)
        {
            StringBuilder sb = new StringBuilder();
            NodoHuffman actual = raiz;

            foreach (char bit in bits)
            {
                if (bit == '0')
                    actual = actual.Izquierda;
                else
                    actual = actual.Derecha;

                if (actual.EsHoja)
                {
                    sb.Append(actual.Simbolo.Value);
                    actual = raiz;
                }
            }

            return sb.ToString();
        }
    }
}



