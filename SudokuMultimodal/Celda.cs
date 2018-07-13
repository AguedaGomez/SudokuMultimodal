using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Ink;

namespace SudokuMultimodal
{
    public class Celda
    {
        public Border UI { get; private set; }

        public bool EstáSeleccionada
        {
            get { return _estáSeleccionado; }
            set
            {
                _estáSeleccionado = value;
                selecciónBorde.Visibility = _estáSeleccionado ? Visibility.Visible : Visibility.Hidden;
                inkCanvas.Visibility = _estáSeleccionado ? Visibility.Visible : Visibility.Hidden;
            }
        }

        // solicitudCambioNúmero: Cuando la celda quiere cambiar el número que contiene (p.e. la tinta reconocida)
        // solicitudSeleccionada: cuando la celda solicita ser la seleccionada
        public Celda(int número, Action<int> solicitudCambioNúmero, Action solicitudSeleccionada)
        {
            _solicitudCambioNúmero = solicitudCambioNúmero;
            _solicitudSeleccionada = solicitudSeleccionada;
            UI = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(0.5), Background=Brushes.Transparent };
            UI.MouseDown += new System.Windows.Input.MouseButtonEventHandler(UI_MouseDown);
            var grid = new Grid();
            UI.Child = grid;
            Binding bindingAltura = new Binding("ActualHeight") { Source = grid };
            BindingOperations.SetBinding(inkCanvas, FrameworkElement.HeightProperty, bindingAltura);
            inkCanvas.DefaultDrawingAttributes = tintaDA;
            grid.Children.Add(inkCanvas);
            grid.Children.Add(_uniformGrid);
            for (int i = 0; i < Sudoku.Tamaño; ++i)
                _uniformGrid.Children.Add(new TextBlock()
                {
                    FontFamily = _fuente,
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });
            
            grid.Children.Add(_textBlock);
            grid.Children.Add(selecciónBorde);
           
            _modificable = número == 0;
            _textBlock.Foreground = _modificable ? Brushes.Blue : Brushes.Black;
            if (número != 0)
                ForzarPonerNúmero(número);
            tinta = new Tinta(inkCanvas, _solicitudCambioNúmero);
        }

        void UI_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _solicitudSeleccionada();
        }

        #region public

        public void PonerNúmero(int número)
        {
            if (!_modificable) return;
            ForzarPonerNúmero(número);
        }

        public void QuitarNúmero()
        {
            if (!_modificable) return;
            _textBlock.Text = "";
            _textBlock.Visibility = Visibility.Hidden;
            _uniformGrid.Visibility = Visibility.Visible;
        }

        public void PonerPosible(int número)
        {
            if (!_modificable) return;
            (_uniformGrid.Children[número - 1] as TextBlock).Text = número.ToString();
        }

        public void QuitarPosible(int número)
        {
            if (!_modificable) return;
            (_uniformGrid.Children[número - 1] as TextBlock).Text = "";
        }

        public void QuitarTodosPosibles()
        {
            for (int número = 1; número <= Sudoku.Tamaño; ++número)
                QuitarPosible(número);
        }

        #endregion

        #region private

        Action<int> _solicitudCambioNúmero;
        Action _solicitudSeleccionada;
        static FontFamily _fuente = new FontFamily("Comic Sans MS");
        bool _estáSeleccionado;
        Tinta tinta;

        Border selecciónBorde = new Border() { BorderBrush = Brushes.Red, BorderThickness = new Thickness(2), Visibility = Visibility.Hidden };

        InkCanvas inkCanvas = new InkCanvas() { Visibility = Visibility.Hidden};

        DrawingAttributes tintaDA = new DrawingAttributes() { Color = Colors.Blue, Height = 4, Width = 4 };

        UniformGrid _uniformGrid = new UniformGrid() { Rows = Sudoku.Tamaño / 3, Columns = Sudoku.Tamaño / 3 };

        TextBlock _textBlock = new TextBlock()
        {
            Visibility = Visibility.Hidden,
            FontFamily = _fuente,
            FontSize = 40,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        bool _modificable;

        void ForzarPonerNúmero(int número)
        {
            _textBlock.Visibility = Visibility.Visible;
            _textBlock.Text = número.ToString();
            _uniformGrid.Visibility = Visibility.Hidden;
        }

        #endregion
    }
}
