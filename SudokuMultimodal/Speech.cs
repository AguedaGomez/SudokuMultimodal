﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SudokuMultimodal
{
    public class Speech
    {
        public bool hayMovimientos;
        public bool deshaciendoVariosMovimientos;
        public bool StartRecognition { get; set; }
        public event Action TerminaEscucha;

        #region private
        SpeechRecognitionEngine speechRecognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("es-ES"));
        Action<int, int, int> SolicitudCambioNumero;
        Action<int> SolicitudCambioNúmeroPosActual;
        Action<int, int> PonSelecciónEn;
        Action DeshacerMovimiento;
        int numero;
        int fila;
        int columna;
        int mvtosDeshechos, mvtosADeshacer;
        DispatcherTimer dispatcherTimer;
        #endregion

        public Speech(Action<int, int, int> SolicitudCambioNumero, Action<int> SolicitudCambioNúmeroPosActual, Action<int, int> PonSelecciónEn, Action DeshacerMovimiento)
        {
            this.SolicitudCambioNumero = SolicitudCambioNumero;
            this.SolicitudCambioNúmeroPosActual = SolicitudCambioNúmeroPosActual;
            this.PonSelecciónEn = PonSelecciónEn;
            this.DeshacerMovimiento = DeshacerMovimiento;
            Grammar grammar = new Grammar("Gramatica.xml");
            speechRecognizer.LoadGrammar(grammar);
            speechRecognizer.SpeechRecognized += SpeechRecognizer_SpeechRecognized;
            speechRecognizer.SpeechDetected += SpeechRecognizer_SpeechDetected;
            speechRecognizer.SetInputToDefaultAudioDevice();
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            StartRecognition = false;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 550);
            mvtosDeshechos = 0;
            deshaciendoVariosMovimientos = false;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
           
            if (mvtosADeshacer > mvtosDeshechos & hayMovimientos)
            {
                DeshacerMovimiento();
                mvtosDeshechos++;
            }
            else
            {
                dispatcherTimer.Stop();
                mvtosDeshechos = 0;
                mvtosADeshacer = 0;
                deshaciendoVariosMovimientos = false;
            }

        }

        private void SpeechRecognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            if(StartRecognition)
                Console.WriteLine("Voz detectada");
        }

        private void SpeechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (StartRecognition)
            {
                Console.WriteLine(e.Result.Text);
                StartRecognition = false;
                if (e.Result.Text.Contains("Deshacer"))
                {
                    mvtosADeshacer = Int32.Parse(e.Result.Semantics["Cantidad"].Value.ToString());
                    deshaciendoVariosMovimientos = true;
                    dispatcherTimer.Start();
                    
                }
                else
                {
                    numero = Int32.Parse(e.Result.Semantics["Numero"].Value.ToString());
                    if (e.Result.Semantics["Fila"].Value.ToString() != "-1" & e.Result.Semantics["Columna"].Value.ToString() != "-1")
                    {
                        fila = Int32.Parse(e.Result.Semantics["Fila"].Value.ToString()) - 1;
                        columna = Int32.Parse(e.Result.Semantics["Columna"].Value.ToString()) - 1;
                        SolicitudCambioNumero(fila, columna, numero);
                        PonSelecciónEn(fila, columna);
                    }
                    else
                    {
                        SolicitudCambioNúmeroPosActual(numero);
                    }
                }
                TerminaEscucha();
            }
        }
    }
}
