using System;

namespace Compresor.Core
{
    public interface ICompressor   
    {
        
        string Nombre { get; }

        byte[] Comprimir(string textoOriginal, out EstadisticasCompresor estadisticas);

        string Descomprimir(byte[] datosComprimidos, out EstadisticasCompresor estadisticas);
    }
}
