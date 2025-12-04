using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compresor.Core
{
    // Representa UNA entrada (un archivo) dentro del .myzip
    public class EntradaMyZip
    {
        public string NombreArchivoOriginal { get; set; }
        public long TamanoOriginalBytes { get; set; }
        public byte[] DatosComprimidos { get; set; }
    }

    public class MiZipFile
    {
        // 1 = Huffman, 2 = LZ77, 3 = LZ78
        public byte IdAlgoritmo { get; set; }

        // Lista de archivos que van dentro del .myzip
        public List<EntradaMyZip> Entradas { get; } = new List<EntradaMyZip>();

        // Crear un zip vacío indicando el algoritmo que se usó
        public static MiZipFile CrearVacio(byte idAlgoritmo)
        {
            return new MiZipFile
            {
                IdAlgoritmo = idAlgoritmo
            };
        }

        // Agregar un archivo ya comprimido
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

        // Guarda el .myzip en disco (TODAS las entradas)
        public void GuardarEnRuta(string rutaSalida)
        {
            using (var fs = new FileStream(rutaSalida, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                // 1 byte: idAlgoritmo (global para todo el zip)
                bw.Write(IdAlgoritmo);

                // 4 bytes: cantidad de archivos
                bw.Write(Entradas.Count);

                foreach (var entrada in Entradas)
                {
                    // nombre archivo
                    byte[] nombreBytes = Encoding.UTF8.GetBytes(entrada.NombreArchivoOriginal);
                    bw.Write(nombreBytes.Length);          // 4 bytes: largo del nombre
                    bw.Write(nombreBytes);                 // N bytes: nombre

                    // 8 bytes: tamaño original
                    bw.Write(entrada.TamanoOriginalBytes);

                    // datos comprimidos
                    bw.Write(entrada.DatosComprimidos.Length); // 4 bytes: largo bloque
                    bw.Write(entrada.DatosComprimidos);        // N bytes: datos
                }
            }
        }

        // Lee un .myzip desde disco y devuelve un MiZipFile con TODAS las entradas
        public static MiZipFile CargarDesdeArchivo(string ruta)
        {
            using (var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                // 1 byte: idAlgoritmo
                byte idAlgoritmo = br.ReadByte();

                // 4 bytes: cantidad de archivos
                int cantidad = br.ReadInt32();

                var zip = new MiZipFile
                {
                    IdAlgoritmo = idAlgoritmo
                };

                for (int i = 0; i < cantidad; i++)
                {
                    // nombre archivo
                    int largoNombre = br.ReadInt32();
                    byte[] nombreBytes = br.ReadBytes(largoNombre);
                    string nombreArchivo = Encoding.UTF8.GetString(nombreBytes);

                    // tamaño original
                    long tamanoOriginal = br.ReadInt64();

                    // datos comprimidos
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

