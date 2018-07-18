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
        public int fila;
        public int columna;

        public Movimiento(int numero, int fila, int columna)
        {
            this.numero = numero;
            this.fila = fila;
            this.columna = columna;
        }
    }
}
