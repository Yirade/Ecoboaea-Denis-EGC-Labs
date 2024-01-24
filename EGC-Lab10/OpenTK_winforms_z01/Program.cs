using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTK_winforms_z01 {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // Utilizarea cuvântului-cheie "using" va permite dealocarea memoriei o dată ce obiectul nu mai este
            // în uz (vezi metoda "Dispose()").
            // Deoarece scena 3D este desenată pe suprafața unui control WinForms, al cărui "painter" este administrat
            // de .NET, nu putem "forța" redesenarea ferestrei la un framerate prestabilit. În schimb vom invoca
            // metoda "Invalidate()" asupra controlului GL (OpenGL) ori de câte ori cadrul 3D a fost actualizat și este
            // nevoie să desenăm modificările pe canvasul controlului GL ("on-demand" rendering).
            using (OpenGLWnd sample = new OpenGLWnd()) {
                sample.ShowDialog();
            }

        }
    }
}
