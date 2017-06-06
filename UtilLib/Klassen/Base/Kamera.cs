using OpenTK;
using OpenTK.Input;
using System;




namespace UtilLib
{
    public class Kamera
    {

        Vector3 camFront;
        Vector3 camPos;
        Vector3 camUp = new Vector3(0, 1, 0);
        float camSpeed;
        float mouseSpeed;

        Matrix4 mView;
        public Matrix4 View { get { return mView; } }

        Matrix4 mProjection;
        public Matrix4 Projection { get { return mProjection; } }

        public Vector3 Position { get { return camPos; } }
        public Vector3 Front { get { return camFront; } }

        bool[] keys;
        public Kamera(GameWindow w, Vector3 pos, Vector3 fistLook, float camSpeed, float mouseSpeed) {
            keys = new bool[1024];

            this.mouseSpeed = mouseSpeed;
            this.camSpeed = camSpeed;

            camPos = pos;
            LookAt(fistLook);

            //so our first few frames look ok
            mView = Matrix4.LookAt(camPos, camPos + camFront, camUp);
            mProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), w.Width / w.Height, 0.1f, 100);

            w.KeyDown += W_KeyDown;
            w.KeyUp += W_KeyUp;
            w.MouseMove += W_MouseMove;
            w.MouseWheel += W_MouseWheel;
            w.UpdateFrame += W_UpdateFrame;
        }

        public void LookAt(Vector3 lookPos) {
            Vector3 lookDir = Vector3.Normalize(lookPos - camPos);
            camFront = lookDir;
            pitch = (float)MathHelper.RadiansToDegrees(Math.Asin(lookDir.Y));
            yaw = (float)MathHelper.RadiansToDegrees(Math.Atan2(-lookDir.X, lookDir.Z)) + 90;
        }

        //gets called every frame
        private void W_UpdateFrame(object sender, FrameEventArgs e) {
            float calcP = (float)MathHelper.RadiansToDegrees(Math.Asin(camFront.Y));
            float calcY = (float)MathHelper.RadiansToDegrees(Math.Atan2(-camFront.X, camFront.Z)) + 90;

            if (keys[(int)Key.E] == true) {
                Console.WriteLine("CameraDebugInfo");
                Console.WriteLine();
                Console.WriteLine("Position=" + camPos);
                Console.WriteLine("Front=   " + camFront);
                Console.WriteLine();
                Console.WriteLine($"act=  p={pitch}  y={yaw}");
                Console.WriteLine($"calc= p={calcP}  y={calcY}");
                Console.WriteLine();
                Console.WriteLine($"diff= p={calcP - pitch}  y={calcY - yaw}");

                keys[(int)Key.E] = false;
            }


            if (keys[(int)Key.W] == true)
                camPos += camFront * camSpeed;
            if (keys[(int)Key.S] == true)
                camPos -= camFront * camSpeed;
            if (keys[(int)Key.A] == true)
                camPos -= Vector3.Normalize(Vector3.Cross(camFront, camUp)) * camSpeed;
            if (keys[(int)Key.D] == true)
                camPos += Vector3.Normalize(Vector3.Cross(camFront, camUp)) * camSpeed;
            if (keys[(int)Key.F] == true)
                camPos += camUp * camSpeed;
            if (keys[(int)Key.C] == true)
                camPos -= camUp * camSpeed;

            mView = Matrix4.LookAt(camPos, camPos + camFront, camUp);
        }



        private void W_MouseWheel(object sender, OpenTK.Input.MouseWheelEventArgs e) {
            //throw new System.NotImplementedException();
        }

        float pitch, yaw;
        private void W_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e) {
            if (!e.Mouse.IsAnyButtonDown)
                return;


            yaw -= e.XDelta * mouseSpeed;
            pitch += e.YDelta * mouseSpeed;

            pitch %= 360;
            yaw %= 360;

            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;

            Vector3 front = new Vector3()
            {
                X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw)),
                Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch)),
                Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw))
            };

            camFront = Vector3.Normalize(front);
        }



        private void W_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e) {
            keys[(int)e.Key] = false;
        }
        private void W_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e) {
            keys[(int)e.Key] = true;
        }




    }
}
