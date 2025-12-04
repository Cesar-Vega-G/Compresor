using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compresor.Core
{
    public class EntradaMyZip
    {
        public string NombreArchivoOriginal { get; set; }
        public long TamanoOriginalBytes { get; set; }
        public byte[] DatosComprimidos { get; set; }
    }

    public class MiZipFile
    {
        public byte IdAlgoritmo { get; set; }
        public List<EntradaMyZip> Entradas { get; } = new List<EntradaMyZip>();

        public static MiZipFile CrearVacio(byte idAlgoritmo)
        {
            return new MiZipFile
            {
                IdAlgoritmo = idAlgoritmo
            };
        }

        public void AgregarEntrada(string nombreArchivoOriginal,
                                   long tamanoOriginalBytes,
                                   byte[] datosComprimidos)
        {
            Entradas.Add(new EntradaMyZip
            {
                NombreArchivoOriginal = nombreArchivoOriginal,
                TamanoOriginalBytes = tamanoOriginalBytes,
                DatosComprimidos = datosComprimidos
            });
        }

        public void GuardarEnRuta(string rutaSalida)
        {
            using (var fs = new FileStream(rutaSalida, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write(IdAlgoritmo);
                bw.Write(Entradas.Count);

                foreach (var entrada in Entradas)
                {
                    byte[] nombreBytes = Encoding.UTF8.GetBytes(entrada.NombreArchivoOriginal);
                    bw.Write(nombreBytes.Length);
                    bw.Write(nombreBytes);
                    bw.Write(entrada.TamanoOriginalBytes);
                    bw.Write(entrada.DatosComprimidos.Length);
                    bw.Write(entrada.DatosComprimidos);
                }
            }
        }

        public static MiZipFile CargarDesdeArchivo(string ruta)
        {
            using (var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                byte idAlgoritmo = br.ReadByte();
                int cantidad = br.ReadInt32();

                var zip = new MiZipFile
                {
                    IdAlgoritmo = idAlgoritmo
                };

                for (int i = 0; i < cantidad; i++)
                {
                    int largoNombre = br.ReadInt32();
                    byte[] nombreBytes = br.ReadBytes(largoNombre);
                    string nombreArchivo = Encoding.UTF8.GetString(nombreBytes);
                    long tamanoOriginal = br.ReadInt64();
                    int largoDatos = br.ReadInt32();
                    byte[] datos = br.ReadBytes(largoDatos);

                    zip.Entradas.Add(new EntradaMyZip
                    {
                        NombreArchivoOriginal = nombreArchivo,
                        TamanoOriginalBytes = tamanoOriginal,
                        DatosComprimidos = datos
                    });
                }

                return zip;
            }
        }
    }
}


