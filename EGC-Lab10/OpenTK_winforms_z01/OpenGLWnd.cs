using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK.Platform;
using OpenTK.Graphics.OpenGL;
using OpenTK;

/**
    Aplicația utilizează biblioteca OpenTK v2.0.0 (stable) oficială și OpenTK. GLControl v2.0.0
    (unstable) neoficială. Aplicația fiind scrisă în modul GUI (WinForms) vom utiliza controlul WinForms
    oferit de OpenTK, pe acre îl vom importa in Toolbox! Acest lucru se poate face doar dacă copiem
    local packetul OpenTK.GLControl.dll oferit de NuGet, apoi îl aducem ca referință în Toolbox.
    Tipul de ferestră utilizat: FORM. Se demmonstrează modul imediat de randare (vezi comentariu!)...
**/
namespace OpenTK_winforms_z01 {
    public partial class OpenGLWnd : Form {

        /// <summary>
        /// Constante utilizate în aplicație.
        /// </summary>
        private const int XYZ_SIZE = 75;

        /// <summary>
        /// Variabile de stare pentru partea de interacțiune/randare 3D.
        /// </summary>
        private int eyePosX = 100;
        private int eyePosY = 100;
        private int eyePosZ = 50;
        private Point mousePos;

        private int pointX_1 = 35;
        private int pointY_1 = 55;
        private int pointZ_1 = 55;
        private int pointX_2 = 45;
        private int pointY_2 = 60;
        private int pointZ_2 = 55;

        private int lineX_1 = 35;
        private int lineY_1 = 25;
        private int lineZ_1 = 20;
        private int lineX_2 = 70;
        private int lineY_2 = 25;
        private int lineZ_2 = 40;

        private int[] triangle_1 = { 35, 25, 20 };
        private int[] triangle_2 = { 70, 25, 40 };
        private int[] triangle_3 = { 30, 60, 50 };

        private int[,] objVertices = {{25, 50, 25,
                                      50, 25, 50,
                                      25, 50, 25,
                                      50, 25, 50,
                                      25, 25, 25,
                                      25, 25, 25,
                                      25, 50, 25,
                                      50, 50, 25,
                                      50, 50, 50,
                                      50, 50, 50,
                                      25, 50, 25,
                                      50, 50, 25},
                                     {25, 25, 60,
                                      25, 60, 60,
                                      25, 25, 25,
                                      25, 25, 25,
                                      25, 60, 25,
                                      60, 25, 60,
                                      60, 60, 60,
                                      60, 60, 60,
                                      25, 60, 25,
                                      60, 25, 60,
                                      25, 25, 60,
                                      25, 60, 60},
                                     {30, 30, 30,
                                      30, 30, 30,
                                      30, 30, 60,
                                      30, 60, 60,
                                      30, 30, 60,
                                      30, 60, 60,
                                      30, 30, 60,
                                      30, 60, 60,
                                      30, 30, 60,
                                      30, 60, 60,
                                      60, 60, 60,
                                      60, 60, 60}};
        private Color[] colorVertices = { Color.White, Color.LawnGreen, Color.WhiteSmoke, Color.Tomato, Color.Turquoise, Color.OldLace, Color.Olive, Color.MidnightBlue, Color.PowderBlue, Color.PeachPuff, Color.LavenderBlush, Color.MediumAquamarine };

        /// <summary>
        /// Variabile de control pentru partea de interacțiune/randare 3D.
        /// </summary>
        private bool axesControl = true;
        private bool mouse2DControl = false;
        private bool mouse3DControl = false;

        private bool pointsControl = false;
        private bool linesControl = false;
        private bool triangleControl = false;
        private bool objectControl = false;

        private bool translateTriangleControl = false;
        private int translateTriangleVal = 0;

        /// <summary>
        /// Constructor default.
        /// </summary>
        public OpenGLWnd() {
            InitializeComponent();
        }

        /// <summary>
        /// Setare mediu OpenGL și încarcarea resurselor (dacă e necesar) - de exemplu culoarea de
        /// fundal a controlului 3D.
        /// Atenție! Acest cod se execută înainte de desenarea efectivă a scenei 3D.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            base.DoubleBuffered = true;
            btnTranslateCurrentObject.Text = "T" + Environment.NewLine + "R" + Environment.NewLine + "A" + Environment.NewLine + "N" + Environment.NewLine + "S" + Environment.NewLine + "L" + Environment.NewLine + "A" + Environment.NewLine + "T" + Environment.NewLine + "E";

            // Ne asigurăm că viewport-ul este setat corect - nu uitați că prima desenare a ferestrei
            // va permite trecerea de la fereastra de mărime (0,0) la cea de mărime specificată - fără
            // un resize vom avea un control de mărime (0,0).
            glControlDemo_Resize(this, EventArgs.Empty);    // Setare viewport.
            SetGLControlDefaultBackground();                // Setare culoare fundal control GL.
        }

        /// <summary>
        /// Inițierea afișării și setarea viewport-ului grafic.
        /// Viewport-ul va fi dimensionat conform mărimii ferestrei active (cele 2 obiecte pot avea și mărimi 
        /// diferite).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControlDemo_Resize(object sender, EventArgs e) {
            if (glControlDemo.ClientSize.Height == 0) {
                glControlDemo.ClientSize = new System.Drawing.Size(glControlDemo.ClientSize.Width, 1);
            }

            SetGLDefaultViewport(glControlDemo);
        }

        /// <summary>
        /// Secțiunea de randare a scenei 3D pe controlul GL. Este declanșată de invalidarea acestuia la
        /// utilizarea metodei "Invalidate()" (printre altele). Randarea aceasta se poate face programatic
        /// (de către sistem) când acesta are nevoie, sau comandat dacă am făcut vreo actualizate în mod
        /// explicit în modelul 3D.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControlDemo_Paint(object sender, PaintEventArgs e) {
            glControlDemo.MakeCurrent();

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 1024);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
            Matrix4 lookat = Matrix4.LookAt(eyePosX, eyePosY, eyePosZ, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            //Corecții de adâncime de culoare.
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            //Resetează buffer-ele la valori default.
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            // Camera control - rotatie 2D/3D cu ajutorul mouse-ului.
            if (mouse2DControl == true) {
                // Doar după Ox.
                GL.Rotate(mousePos.X, 1, 0, 0);
            }
            if (mouse3DControl == true) {
                // Doar după Ox.
                GL.Rotate(mousePos.X, 1, 1, 1);
            }

            // Scena 3D!
            if (axesControl) {
                DrawAxes();
            }

            if (pointsControl) {
                DrawPoints();
            }

            if (linesControl) {
                DrawLine();
            }

            if (triangleControl) {
                if (translateTriangleControl) {
                    GL.Translate(1.0f + translateTriangleVal, 1.0f + translateTriangleVal, 1.0f + translateTriangleVal);
                    DrawTriangle();
                    translateTriangleControl = false;
                } else {
                    DrawTriangle();
                }
            }

            if (objectControl) {
                DrawObject();
            }

            glControlDemo.SwapBuffers();
        }

        /**********************************************************************************************
         * 3D scene area!
         **********************************************************************************************/
        /// <summary>
        /// Desenează axele de coordonate. Dimensiunea absolută implicită a acestora este 150 (modificabil 
        /// din constanta XYZ_SIZE declarată).
        /// </summary>
        private void DrawAxes() {
            // Desenează axa Ox (cu roșu).
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(XYZ_SIZE, 0, 0);
            GL.End();

            // Desenează axa Oy (cu galben).
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, XYZ_SIZE, 0); ;
            GL.End();

            // Desenează axa Oz (cu verde).
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, XYZ_SIZE);
            GL.End();
        }

        private void DrawPoints() {
            GL.Begin(PrimitiveType.Points);
            GL.Color3(Color.White);
            GL.Vertex3(pointX_1, pointY_1, pointZ_1);
            GL.End();
            GL.Begin(PrimitiveType.Points);
            GL.Color3(Color.Red);
            GL.Vertex3(pointX_2, pointY_2, pointZ_2);
            GL.End();
        }

        private void DrawLine() {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.White);
            GL.Vertex3(lineX_1, lineY_1, lineZ_1);
            GL.Vertex3(lineX_2, lineY_2, lineZ_2);
            GL.End();
        }

        private void DrawTriangle() {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(Color.Magenta);
            GL.Vertex3(triangle_1[0], triangle_1[1], triangle_1[2]);
            GL.Vertex3(triangle_2[0], triangle_2[1], triangle_2[2]);
            GL.Vertex3(triangle_3[0], triangle_3[1], triangle_3[2]);
            GL.End();
        }

        private void DrawObject() {
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < 35; i = i + 3) {
                //For i As Integer = 0 To 35 Step 3
                GL.Color3(colorVertices[i / 3]);
                GL.Vertex3(objVertices[0, i], objVertices[1, i], objVertices[2, i]);
                GL.Vertex3(objVertices[0, i + 1], objVertices[1, i + 1], objVertices[2, i + 1]);
                GL.Vertex3(objVertices[0, i + 2], objVertices[1, i + 2], objVertices[2, i + 2]);
            }
            GL.End();
        }

        /**********************************************************************************************
         * OpenGL/OpenTK helper functions area!
         **********************************************************************************************/
        /// <summary>
        /// Setează culoarea de fundal specificată pentru controlul OpenTK. Culoarea default preferată este
        /// negrul...
        /// </summary>
        /// <param name="col"></param>
        private void SetGLControlBackground(Color col) {
            GL.ClearColor(col);
        }

        /// <summary>
        /// Setează culoarea de fundal deafult pentru controlul OpenTK. Culoarea default preferată este
        /// negrul...
        /// </summary>
        private void SetGLControlDefaultBackground() {
            GL.ClearColor(Color.Black);
        }

        /// <summary>
        /// Setează dimensiunea default a viewport-ului, bazată pe cea a zonei-client a controlului OpenTK.
        /// </summary>
        /// <param name="ctrl"></param>
        private void SetGLDefaultViewport(GLControl ctrl) {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, glControlDemo.ClientSize.Width, 0, glControlDemo.ClientSize.Height, -1, 1);
            GL.Viewport(0, 0, glControlDemo.ClientSize.Width, glControlDemo.ClientSize.Height);
        }


        /**********************************************************************************************
         * CONTROLS area!
         **********************************************************************************************/
        private void btnBgWhite_Click(object sender, EventArgs e) {
            SetGLControlBackground(Color.White);

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnBgBlue_Click(object sender, EventArgs e) {
            SetGLControlBackground(Color.MidnightBlue);

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnBgRed_Click(object sender, EventArgs e) {
            SetGLControlBackground(Color.IndianRed);

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnBgReset_Click(object sender, EventArgs e) {
            SetGLControlDefaultBackground();

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        /// <summary>
        /// Controlează afișarea axelor de coordonate!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAxes_CheckedChanged(object sender, EventArgs e) {
            if (((CheckBox)sender).Checked == true) {
                axesControl = true;
            } else {
                axesControl = false;
            }

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        /// <summary>
        /// Controlează mișcarea universului 3D cu ajutorul mouse-ului după 2 axe (a 3-a e fixă).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMouse2D_Click(object sender, EventArgs e) {
            if (mouse2DControl == false) {
                unset3Dmouse();
                set2Dmouse();
            } else {
                unset2Dmouse();
            }

            // Forțează redesenarea canvasului!
            //glControlDemo.Invalidate();
        }

        /// <summary>
        /// Controlează mișcarea universului 3D cu ajutorul mouse-ului după 3 axe.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMouse3D_Click(object sender, EventArgs e) {
            if (mouse3DControl == false) {
                unset2Dmouse();
                set3Dmouse();
            } else {
                unset3Dmouse();
            }

            // Forțează redesenarea canvasului!
            //glControlDemo.Invalidate();
        }

        private void set2Dmouse() {
            mouse2DControl = true;
            btnMouse2D.Font = new Font(btnMouse2D.Font, FontStyle.Regular);
        }

        private void unset2Dmouse() {
            mouse2DControl = false;
            btnMouse2D.Font = new Font(btnMouse2D.Font, FontStyle.Strikeout);
        }

        private void set3Dmouse() {
            mouse3DControl = true;
            btnMouse3D.Font = new Font(btnMouse3D.Font, FontStyle.Regular);
        }

        private void unset3Dmouse() {
            mouse3DControl = false;
            btnMouse3D.Font = new Font(btnMouse3D.Font, FontStyle.Strikeout);
        }

        private void glControlDemo_MouseMove(object sender, MouseEventArgs e) {
            mousePos = new Point(e.X, e.Y);
            lblPointerDisplay.Text = e.X.ToString() + " , " + e.Y.ToString();

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void glControlDemo_MouseLeave(object sender, EventArgs e) {
            lblPointerDisplay.Text = "";
        }

        /// <summary>
        /// Manipulează adâncimea perspectivei pentru axa Ox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spinnerXposView_ValueChanged(object sender, EventArgs e) {
            eyePosX = (int)((NumericUpDown)sender).Value;

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        /// <summary>
        /// Manipulează adâncimea perspectivei pentru axa Oy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spinnerYposView_ValueChanged(object sender, EventArgs e) {
            eyePosY = (int)((NumericUpDown)sender).Value;

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        /// <summary>
        /// Manipulează adâncimea perspectivei pentru axa Oz.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spinnerZposView_ValueChanged(object sender, EventArgs e) {
            eyePosZ = (int)((NumericUpDown)sender).Value;

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnDrawPoints_Click(object sender, EventArgs e) {
            UnsetAllObjects();
            SetPoints();

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnDrawLine_Click(object sender, EventArgs e) {
            UnsetAllObjects();
            SetLine();

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnDrawTriangle_Click(object sender, EventArgs e) {
            UnsetAllObjects();
            SetTriangle();

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnDrawObject_Click(object sender, EventArgs e) {
            UnsetAllObjects();
            SetObject();

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnTranslateCurrentObject_Click(object sender, EventArgs e) {
            // Momentan doar triunghiul!!!
            translateTriangleControl = true;
            translateTriangleVal++;

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void btnResetObjects_Click(object sender, EventArgs e) {
            UnsetAllObjects();

            // Forțează redesenarea canvasului!
            glControlDemo.Invalidate();
        }

        private void SetPoints() {
            pointsControl = true;
        }

        private void UnsetPoints() {
            pointsControl = false;
        }

        private void SetLine() {
            linesControl = true;
        }

        private void UnsetLine() {
            linesControl = false;
        }

        private void SetTriangle() {
            triangleControl = true;
        }

        private void UnsetTriangle() {
            triangleControl = false;
        }

        private void SetObject() {
            objectControl = true;
        }

        private void UnsetObject() {
            objectControl = false;
        }

        private void UnsetAllObjects() {
            translateTriangleVal = 0;

            UnsetPoints();
            UnsetLine();
            UnsetTriangle();
            UnsetObject();
        }

    }
}
