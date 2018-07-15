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
       List<KeyValuePair<int,int>> listaCuadrantesOcupados;

        public MemoriaNumerosIntroducidos()
        {
            listaCuadrantesOcupados = new List<KeyValuePair<int, int>>();
        }

        public void GuardarMovimiento (KeyValuePair<int, int> mov)
        {
            listaCuadrantesOcupados.Add(mov);
            if (listaCuadrantesOcupados.Count == 1)
                finCuadrantesOcupados(true);
        }

        public void GetUltimoMovimiento (out int cuadrante, out int posicion)
        {
            var ultimoMovimiento = listaCuadrantesOcupados.Last();
            cuadrante = ultimoMovimiento.Key;
            posicion = ultimoMovimiento.Value;
        }

        public void EliminarCuadrante(KeyValuePair<int,int> cuadranteBorrado)
        {
            listaCuadrantesOcupados.Remove(cuadranteBorrado);
            if (listaCuadrantesOcupados.Count <= 0)
                finCuadrantesOcupados.Invoke(false);
        }

    }
}
