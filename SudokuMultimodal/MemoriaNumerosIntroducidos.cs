using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuMultimodal
{
    public class MemoriaNumerosIntroducidos
    {
       Stack<KeyValuePair<int,int>> pilaCuadrantesOcupados;

        public MemoriaNumerosIntroducidos()
        {
            pilaCuadrantesOcupados = new Stack<KeyValuePair<int, int>>();
        }

        public void GuardarMovimiento (KeyValuePair<int, int> mov)
        {
            pilaCuadrantesOcupados.Push(mov);
        }

        public void GetUltimoMovimiento (out int cuadrante, out int posicion)
        {
            var ultimoMovimiento = pilaCuadrantesOcupados.Peek();
            cuadrante = ultimoMovimiento.Key;
            posicion = ultimoMovimiento.Value;
        }

        public void DeshacerUltimoMovimiento ()
        {
            pilaCuadrantesOcupados.Pop();
            
        }
    }
}
