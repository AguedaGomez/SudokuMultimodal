using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace SudokuMultimodal
{
    public class Speech
    {
        SpeechRecognitionEngine speechRecognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("es-ES"));
        Action<int, int, int> SolicitudCambioNumero;
        Action<int> SolicitudCambioNúmeroPosActual;
        int numero;
        int fila;
        int columna;

        public bool startRecognition { get; set; }

        public Speech(Action<int, int, int> SolicitudCambioNumero, Action<int> SolicitudCambioNúmeroPosActual)
        {
            this.SolicitudCambioNumero = SolicitudCambioNumero;
            this.SolicitudCambioNúmeroPosActual = SolicitudCambioNúmeroPosActual;
            Grammar grammar = new Grammar("Gramatica.xml");
            speechRecognizer.LoadGrammar(grammar);
            speechRecognizer.SpeechRecognized += SpeechRecognizer_SpeechRecognized;
            speechRecognizer.SpeechDetected += SpeechRecognizer_SpeechDetected;
            speechRecognizer.SetInputToDefaultAudioDevice();
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            startRecognition = false;
        }

        private void SpeechRecognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            if(startRecognition)
                Console.WriteLine("Voz detectada");
        }

        private void SpeechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (startRecognition)
            {
                Console.WriteLine(e.Result.Text);
                startRecognition = false;
                numero = Int32.Parse(e.Result.Semantics["Numero"].Value.ToString());
                if (e.Result.Semantics["Fila"].Value.ToString() != "-1" & e.Result.Semantics["Columna"].Value.ToString() != "-1")
                {
                    fila = Int32.Parse(e.Result.Semantics["Fila"].Value.ToString());
                    columna = Int32.Parse(e.Result.Semantics["Columna"].Value.ToString());
                    SolicitudCambioNumero(fila, columna, numero);
                }
                else
                {
                    SolicitudCambioNúmeroPosActual(numero);
                }
                
                
            }
        }
    }
}
