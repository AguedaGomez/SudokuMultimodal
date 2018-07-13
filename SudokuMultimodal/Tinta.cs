using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Threading;

namespace SudokuMultimodal
{
    public class Tinta
    {
        InkAnalyzer m_analyzer;
        DispatcherTimer dispatcherTimer;
        InkCanvas inkCanvas;
        Action<int> solicitudCambioNumero;
        readonly List<string> numeros = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public Tinta(InkCanvas inkCanvas, Action<int> solicitudCambioNumero)
        {
            this.inkCanvas = inkCanvas;
            this.solicitudCambioNumero = solicitudCambioNumero;
            m_analyzer = new InkAnalyzer();
            m_analyzer.AnalysisModes = AnalysisModes.AutomaticReconciliationEnabled;
            m_analyzer.ResultsUpdated += M_analizer_ResultsUpdated;
            inkCanvas.StrokeCollected += InkCanvas_StrokeCollected;
            inkCanvas.StrokeErasing += InkCanvas_StrokeErasing;
            inkCanvas.StylusUp += InkCanvas_StylusUp;
            inkCanvas.StylusMove += InkCanvas_StylusMove;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
        }

        private void InkCanvas_StrokeErasing(object sender, InkCanvasStrokeErasingEventArgs e)
        {
            m_analyzer.RemoveStroke(e.Stroke);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            m_analyzer.BackgroundAnalyze();
            dispatcherTimer.Stop();
        }

        private void InkCanvas_StylusMove(object sender, System.Windows.Input.StylusEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void InkCanvas_StylusUp(object sender, System.Windows.Input.StylusEventArgs e)
        {
            dispatcherTimer.Start();
        }

        private void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            m_analyzer.AddStroke(e.Stroke);
        }

        private void M_analizer_ResultsUpdated(object sender, ResultsUpdatedEventArgs e)
        {
            if (e.Status.Successful)
            {
                ContextNodeCollection leaves = ((InkAnalyzer)sender).FindLeafNodes();
                foreach (ContextNode leaf in leaves)
                {
                    if (leaf is InkWordNode)
                    {
                        // Como palabra
                        InkWordNode t = leaf as InkWordNode;
                        Rect l = t.Location.GetBounds();
                       ReconocerEntrada(t.GetRecognizedString());
                    }

                }
                LimpiarCanvas();
            }
        }

        private void ReconocerEntrada(string entrada)
        {
            Console.WriteLine("Se ha reconocido " + entrada);
            if (numeros.Contains(entrada))
                solicitudCambioNumero(Int32.Parse(entrada));
        }

        private void LimpiarCanvas()
        {
            if(inkCanvas.Strokes.Count() > 0)
            {
                m_analyzer.RemoveStrokes(inkCanvas.Strokes);
                inkCanvas.Strokes.Clear();
            }
        }
    }
}
