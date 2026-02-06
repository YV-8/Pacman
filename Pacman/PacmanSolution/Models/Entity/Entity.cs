namespace PacmanSolution.Models;

public abstract class Entity
{
    private int _x;
    private int _y;
    public int X
    {
        get { return _x; }
        set { _x = value; }
    }

    public int Y
    {
        get { return _y; }
        set { _y = value; }
    }
    
    public Entity(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public virtual void Move(int dx, int dy)
    {
        _x += dx;
        _y += dy;
    }
}