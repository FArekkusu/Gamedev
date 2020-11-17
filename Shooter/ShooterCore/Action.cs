namespace ShooterCore
{
    public class Action
    {
        public int Dx;
        public int Dy;
        public bool IsShooting;
        public int MouseX;
        public int MouseY;

        public Action(int dx, int dy, bool isShooting, int mouseX, int mouseY)
        {
            Dx = dx;
            Dy = dy;
            IsShooting = isShooting;
            MouseX = mouseX;
            MouseY = mouseY;
        }
    }
}