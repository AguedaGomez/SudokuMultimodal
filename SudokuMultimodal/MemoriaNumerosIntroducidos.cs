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
       List<Movimiento> listaCuadrantesOcupados;

        public MemoriaNumerosIntroducidos()
        {
            listaCuadrantesOcupados = new List<Movimiento>();
        }

        public void GuardarMovimiento (int numero, int cuadrante, int posicion)
        {
            Movimiento m = new Movimiento(numero, cuadrante, posicion);
            listaCuadrantesOcupados.Add(m);
            if (listaCuadrantesOcupados.Count == 1)
                finCuadrantesOcupados(true);
        }

        public void GetUltimoMovimiento (out int numero, out int cuadrante, out int posicion)
        {
            var ultimoMovimiento = listaCuadrantesOcupados.Last();
            numero = ultimoMovimiento.numero;
            cuadrante = ultimoMovimiento.cuadrante;
            posicion = ultimoMovimiento.posicion;
            EliminarCuadrante(ultimoMovimiento);
        }

        public void EliminarCuadrante(Movimiento m)
        {
            listaCuadrantesOcupados.Remove(m);
            if (listaCuadrantesOcupados.Count <= 0)
                finCuadrantesOcupados.Invoke(false);
        }
        public void EliminarMovimientos()
        {
            listaCuadrantesOcupados.Clear();
        }

    }
}
