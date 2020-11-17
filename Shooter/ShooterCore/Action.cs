namespace ShooterCore
{
    public class Action
    {
        public readonly int Dx;
        public readonly int Dy;
        public readonly bool IsShooting;
        public readonly int MouseX;
        public readonly int MouseY;

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