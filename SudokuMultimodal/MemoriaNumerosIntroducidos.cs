using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuMultimodal
{
    public class MemoriaNumerosIntroducidos
    {
        public event Action<bool> finCuadrantesOcupados;
       Stack<KeyValuePair<int,int>> pilaCuadrantesOcupados;

        public MemoriaNumerosIntroducidos()
        {
            pilaCuadrantesOcupados = new Stack<KeyValuePair<int, int>>();
        }

        public void GuardarMovimiento (KeyValuePair<int, int> mov)
        {
            pilaCuadrantesOcupados.Push(mov);
            if (pilaCuadrantesOcupados.Count == 1)
                finCuadrantesOcupados(true);
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
            if (pilaCuadrantesOcupados.Count <= 0)
                finCuadrantesOcupados.Invoke(false);
        }

    }
}
