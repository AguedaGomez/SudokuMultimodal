using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace SudokuMultimodal
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            InitializeComponent();
        }
        #region private

        Sudoku _s;
        Cuadrante[] _cuadrantes;
        UniformGrid _ug;
        int _filaActual, _columnaActual;
        bool mostrarPosibles;
        Speech speech;
        WiimoteFunctionality wmF;
        MemoriaNumerosIntroducidos memoria;

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mostrarPosibles = false;
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            _ug = new UniformGrid() { Rows = Sudoku.Tamaño / 3, Columns = Sudoku.Tamaño / 3, Background = Brushes.WhiteSmoke };
            mainGrid.Children.Add(_ug);
            _cuadrantes = new Cuadrante[Sudoku.Tamaño];

            NuevaPartida();
            InicializarFuncionMultimodal();
            memoria = new MemoriaNumerosIntroducidos();
            memoria.finCuadrantesOcupados += Memoria_finCuadrantesOcupados;
        }

        private void Memoria_finCuadrantesOcupados(bool obj)
        {
            BotonDeshacer.IsEnabled = obj;
            speech.hayMovimientos = obj;
        }

        private void InicializarFuncionMultimodal()
        {
            speech = new Speech(SolicitudCambioNúmero, SolicitudCambioNúmeroPosActual, PonSelecciónEn, DeshacerMovimiento);
            speech.TerminaEscucha += Speech_TerminaEscucha;
            wmF = new WiimoteFunctionality(SolicitudCambioNúmeroPosActual, MoverSeleccion, MostrarMensaje);
            
        }

        private void Speech_TerminaEscucha()
        {
            BotonEscuchar.IsEnabled = true;
            BotonEscuchar.Content = "Escuchar";
        }

        void NuevaPartida() //Mientras no cambiemos el constructor de Sudoku siempre es la misma partida
        {
            _filaActual = _columnaActual = -1;
            _s = new Sudoku();
            _s.CeldaCambiada += CuandoCeldaCambiada;

            ActualizarVistaSudoku();

            //Copio a mano algunos números de la solución: el sudoku elegido es dificil ;)
            _s[0, 0] = 2;
            _s[4, 4] = 1;
            _s[7, 7] = 5;
            _s[1, 7] = 8;
            _s[7, 1] = 4;
            _s[3, 2] = 6;
            _s[5, 6] = 9;
        }

        void ReiniciarPartida()
        {
            _s.Reiniciar();
            ActualizarVistaSudoku();
        }

        void ActualizarVistaSudoku()
        {
            _ug.Children.Clear();

            for (int cuad = 0; cuad < Sudoku.Tamaño; ++cuad)
            {
                var cuadrante = new Cuadrante(_s, cuad, SolicitudCambioNúmero, SolicitudSeleccionada, EliminarDeMemoria);
                _cuadrantes[cuad] = cuadrante;
                _ug.Children.Add(cuadrante.UI);
            }

            ActualizaPosibles();

            PonSelecciónEn(0, 0);
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                case Key.Back:
                    {
                        _s[_filaActual, _columnaActual] = 0;
                        ActualizaPosibles();
                    }
                    break;
                case Key.Up:
                    if (_filaActual > 0)
                        PonSelecciónEn(_filaActual - 1, _columnaActual);
                    break;
                case Key.Down:
                    if (_filaActual < Sudoku.Tamaño - 1)
                        PonSelecciónEn(_filaActual + 1, _columnaActual);
                    break;
                case Key.Left:
                    if (_columnaActual > 0)
                        PonSelecciónEn(_filaActual, _columnaActual - 1);
                    break;
                case Key.Right:
                    if (_columnaActual < Sudoku.Tamaño - 1)
                        PonSelecciónEn(_filaActual, _columnaActual + 1);
                    break;
                case Key.D1:
                case Key.NumPad1:
                case Key.D2:
                case Key.NumPad2:
                case Key.D3:
                case Key.NumPad3:
                case Key.D4:
                case Key.NumPad4:
                case Key.D5:
                case Key.NumPad5:
                case Key.D6:
                case Key.NumPad6:
                case Key.D7:
                case Key.NumPad7:
                case Key.D8:
                case Key.NumPad8:
                case Key.D9:
                case Key.NumPad9:
                    {
                        SolicitudCambioNúmero(_filaActual, _columnaActual, int.Parse(new string(e.Key.ToString()[1], 1)));
                       // _s[_filaActual, _columnaActual] = int.Parse(new string(e.Key.ToString()[1], 1));
                        ActualizaPosibles();
                        e.Handled = true;
                    }
                    break;
                case Key.Space:
                    ComenzarEscucha();
                    break;
                default:
                    break;
            }
        }

        void PonSelecciónEn(int fil, int col)
        {
            if (_filaActual >= 0 && _columnaActual >= 0)
            {
                int cuad, pos;
                Sudoku.FilaColumnaACuadrantePosicion(_filaActual, _columnaActual, out cuad, out pos);
                _cuadrantes[cuad].DeseleccionaCelda(pos);
            }

            int cuad2, pos2;
            Sudoku.FilaColumnaACuadrantePosicion(fil, col, out cuad2, out pos2);
            _cuadrantes[cuad2].SeleccionaCelda(pos2);
            _filaActual = fil;
            _columnaActual = col;
        }

        void MoverSeleccion(string direccion)
        {
            int fila = _filaActual;
            int columna = _columnaActual;
            switch (direccion)
            {
                case "derecha":
                    if (columna < Sudoku.Tamaño-1)
                        columna += 1;
                    break;
                case "izquierda":
                    if (columna > 0)
                        columna -= 1;
                    break;
                case "arriba":
                    if (fila > 0)
                        fila -= 1;
                    break;
                case "abajo":
                    if (fila < Sudoku.Tamaño-1)
                    fila += 1;
                    break;
                default:
                    break;
            }
            PonSelecciónEn(fila, columna);

        }

        void ActualizaPosibles()
        {
            for (int cuad = 0; cuad < Sudoku.Tamaño; ++cuad)
                _cuadrantes[cuad].QuitaTodosPosibles();

            if (!mostrarPosibles) return;

            for (int f = 0; f < Sudoku.Tamaño; ++f)
                for (int c = 0; c < Sudoku.Tamaño; ++c)
                {
                    int cuad, pos;
                    Sudoku.FilaColumnaACuadrantePosicion(f, c, out cuad, out pos);
                    foreach (var num in _s.PosiblesEnCelda(f, c))
                        _cuadrantes[cuad].PonerPosibleEnPos(pos, num);
                }
        }

        void CuandoCeldaCambiada(int fila, int columna, int nuevoNúmero)
        {
            int cuad, pos;
            Sudoku.FilaColumnaACuadrantePosicion(fila, columna, out cuad, out pos);
            //añade el número
            if (nuevoNúmero > 0)
                _cuadrantes[cuad].PonerNúmeroEnPos(pos, nuevoNúmero);
            else
                _cuadrantes[cuad].QuitarNúmeroEnPos(pos);
            //Actualiza posibles
            ActualizaPosibles();
        }

        void SolicitudCambioNúmero(int fila, int col, int número)
        {
            _s[fila, col] = número;
            GuardarMovimiento();
        }

        private void GuardarMovimiento()
        {
            int cuadrante, posición;
            Sudoku.FilaColumnaACuadrantePosicion(_filaActual, _columnaActual, out cuadrante, out posición);
            memoria.GuardarMovimiento(new KeyValuePair<int, int>(cuadrante, posición));
        }

        private void DeshacerMovimiento()
        {
            if (BotonDeshacer.IsEnabled)
            {
                memoria.GetUltimoMovimiento(out int cuadrante, out int posicion);
                Sudoku.CuadrantePosicionAFilaColumna(cuadrante, posicion, out int fila, out int columna);
                PonSelecciónEn(fila, columna);
                _cuadrantes[cuadrante].QuitarNúmeroEnPos(posicion);
            }

        }

        private void EliminarDeMemoria()
        {
            Sudoku.FilaColumnaACuadrantePosicion(_filaActual, _columnaActual, out int cuadrante, out int posicion);
            memoria.EliminarCuadrante(new KeyValuePair<int, int>(cuadrante, posicion));

        }

        void SolicitudSeleccionada(int fila, int col)
        {
            PonSelecciónEn(fila, col);
        }

        void SolicitudCambioNúmeroPosActual(int numero)
        {
            SolicitudCambioNúmero(_filaActual, _columnaActual, numero);
        }

        void botónNuevoClick(object sender, RoutedEventArgs e)
        {
            NuevaPartida();
        }

        void botónReiniciarClick(object sender, RoutedEventArgs e)
        {
            ReiniciarPartida();
        }

        private void numero_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            SolicitudCambioNúmero(_filaActual, _columnaActual, int.Parse(b.Content.ToString()));
        }

        private void ComenzarEscucha()
        {
            speech.StartRecognition = true;
            BotonEscuchar.Content = "Escuchando";
            BotonEscuchar.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComenzarEscucha();
        }

        private void BotonDeshacer_Click(object sender, RoutedEventArgs e)
        {
            DeshacerMovimiento();
        }

        private void MostrarMensaje(string mensaje)
        {
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            var resultado = MessageBox.Show(mensaje, "No se encuentra Wiimote", buttons);
            if(resultado == MessageBoxResult.No)
            {
                this.Close();
            }
        }

        void checkboxVerPosiblesClick(object sender, RoutedEventArgs e)
        {
            mostrarPosibles = (sender as CheckBox).IsChecked == true;
            ActualizaPosibles();
        }
            
        #endregion
    }
}
