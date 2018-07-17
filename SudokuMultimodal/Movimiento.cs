using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuMultimodal
{
    public class Movimiento
    {
        public int numero;
        public int cuadrante;
        public int posicion;

        public Movimiento(int numero, int cuadrante, int posicion)
        {
            this.numero = numero;
            this.cuadrante = cuadrante;
            this.posicion = posicion;
        }
    }
}
