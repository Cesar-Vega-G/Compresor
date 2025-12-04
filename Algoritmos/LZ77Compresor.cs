using Compresor.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compresor.Algoritmos
{
    public class LZ77Compresor : ICompressor
        
    {
        public string Nombre => "LZ77";
        private const int WindowSize = 1024;

        public byte[] Comprimir(string texto, out EstadisticasCompresor est)
        {
            est = new EstadisticasCompresor();
            List<byte> output = new List<byte>();

            int pos = 0;

            while (pos < texto.Length)
            {
                int bestLength = 0;
                int bestDistance = 0;

                int start = Math.Max(0, pos - WindowSize);
                string window = texto.Substring(start, pos - start);

                for (int dist = 1; dist <= window.Length; dist++)
                {
                    int length = 0;

                    while (pos + length < texto.Length &&
                           window[window.Length - dist + length] ==
                           texto[pos + length])
                    {
                        length++;
                        if (window.Length - dist + length >= window.Length)
                            break;
                    }

                    if (length > bestLength)
                    {
                        bestLength = length;
                        bestDistance = dist;
                    }
                }

                if (bestLength >= 3)
                {
                    output.Add(1);
                    output.Add((byte)bestDistance);
                    output.Add((byte)bestLength);
                    pos += bestLength;
                }
                else
                {
                    output.Add(0);
                    output.Add((byte)texto[pos]);
                    pos++;
                }
            }

            return output.ToArray();
        }

        public string Descomprimir(byte[] datos, out EstadisticasCompresor est)
        {
            est = new EstadisticasCompresor();
            StringBuilder result = new StringBuilder();

            int pos = 0;

            while (pos < datos.Length)
            {
                byte flag = datos[pos++];
                if (flag == 0)
                {
                    result.Append((char)datos[pos++]);
                }
                else
                {
                    int dist = datos[pos++];
                    int length = datos[pos++];

                    int start = result.Length - dist;

                    for (int i = 0; i < length; i++)
                        result.Append(result[start + i]);
                }
            }

            return result.ToString();
        }
    }
}

