using GestureLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WiimoteLib;

namespace SudokuMultimodal
{
    public class WiimoteFunctionality
    {
        Wiimote wm;
        GestureCapturer gestureCapturer;
        GestureRecognizer gestureRecognizer;
        Action<int> SolicitudCambioNúmeroPosActual;
        Action<string> MoverSeleccion;
        Action<string> MostrarMensaje;
        DateTime ultimoTiempo = DateTime.MaxValue;
        float intervaloTiempo = 300f;

        public WiimoteFunctionality(Action<int> SolicitudCambioNúmeroPosActual, Action<string> MoverSeleccion, Action<string> MostrarMensaje)
        {
            wm = new Wiimote();
            this.MostrarMensaje = MostrarMensaje;
            this.SolicitudCambioNúmeroPosActual = SolicitudCambioNúmeroPosActual;
            this.MoverSeleccion = MoverSeleccion;
            gestureCapturer = new GestureCapturer();
            gestureRecognizer = new GestureRecognizer();
            CargarGestos();
            wm.WiimoteChanged += Wm_WiimoteChanged;
            try
            {
                wm.Connect();
            }
            catch (Exception)
            {
                MostrarMensaje("El dispositivo Wiimote no está conectado. ¿Quieres seguir jugando sin él?");
            }
            
            wm.SetReportType(InputReport.ButtonsAccel, true);
            gestureCapturer.GestureCaptured += GestureCapturer_GestureCaptured;
            gestureRecognizer.GestureRecognized += GestureRecognizer_GestureRecognized;
        }

        private void GestureRecognizer_GestureRecognized(string obj)
        {
            //llamar a la funcion que cambia numero
            Console.WriteLine("se ha reconocido el gesto " + obj);
            Application.Current.Dispatcher.BeginInvoke(SolicitudCambioNúmeroPosActual, Int32.Parse(obj));
        }

        private void GestureCapturer_GestureCaptured(Gesture obj)
        {
            gestureRecognizer.OnGestureCaptured(obj);
        }

        private void Wm_WiimoteChanged(object sender, WiimoteChangedEventArgs e)
        {
            var direccion = "";
            if (Math.Abs(DateTime.Now.Subtract(ultimoTiempo).Milliseconds) > intervaloTiempo)
            {
                if (e.WiimoteState.ButtonState.Down)
                    direccion = "abajo";
                else if (e.WiimoteState.ButtonState.Up)
                    direccion = "arriba";
                else if (e.WiimoteState.ButtonState.Left)
                    direccion = "izquierda";
                else if (e.WiimoteState.ButtonState.Right)
                    direccion = "derecha";
                if (direccion != "")
                {
                    Application.Current.Dispatcher.BeginInvoke(MoverSeleccion, direccion);
                    ultimoTiempo = DateTime.Now;
                }   
                direccion = "";
            }
            gestureCapturer.OnWiimoteChanged(e.WiimoteState);
        }

        private void CargarGestos()
        {
            Stream stream = new FileStream("Gestos.txt", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            List<Gesture> prototypes = (List<Gesture>)bf.Deserialize(stream);
            foreach (var p in prototypes)
            {
                gestureRecognizer.AddPrototype(p);
            }
        }
    }
}
