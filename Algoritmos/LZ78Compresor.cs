using Compresor.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compresor.Algoritmos
{
    public class LZ78Compresor : ICompressor
    {
        public string Nombre => "LZ78";
        public byte[] Comprimir(string texto, out EstadisticasCompresor est)
        {
            est = new EstadisticasCompresor();

            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<byte> output = new List<byte>();

            string actual = "";
            int index = 1;

            foreach (char c in texto)
            {
                string temp = actual + c;

                if (dic.ContainsKey(temp))
                {
                    actual = temp;
                }
                else
                {
                    if (actual == "")
                    {
                        output.Add(0);
                    }
                    else
                    {
                        output.Add((byte)dic[actual]);
                    }
                    output.Add((byte)c);

                    dic[temp] = index++;
                    actual = "";
                }
            }

            if (actual != "")
            {
                output.Add((byte)dic[actual]);
                output.Add(0);
            }

            return output.ToArray();
        }

        public string Descomprimir(byte[] datos, out EstadisticasCompresor est)
        {
            est = new EstadisticasCompresor();

            List<string> dic = new List<string>();
            dic.Add("");

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < datos.Length; i += 2)
            {
                int idx = datos[i];
                char c = (char)datos[i + 1];

                string entry = dic[idx] + (c == 0 ? "" : c.ToString());
                result.Append(entry);
                dic.Add(entry);
            }

            return result.ToString();
        }
    }
}

