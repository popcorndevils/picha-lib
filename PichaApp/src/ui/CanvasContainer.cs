using Godot;

public class CanvasContainer : Control
{
    private bool _Dragging;
    private RulerGrid _Grid = new RulerGrid();

    private GenCanvas _Canvas;
    public GenCanvas Canvas {
        get => this._Canvas;
        set {
            
            if(this._Canvas != null)
                { this.RemoveChild(value); }
            this._Canvas = value;
            this.AddChild(value);
            this.MoveChild(value, 0);
            value.Scale = new Vector2(20, 20);
            value.Position = (this.RectSize / 2) - ((value.Size / 2) * value.Scale);
        }
    }

    public override void _Ready()
    {
        this.RectClipContent = true;
        this.Connect("visibility_changed", this, "OnVisibleChanged");

        this.AddChild(this._Grid);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if(@event is InputEventMouseButton btn)
        {
            if(btn.ButtonIndex == (int)ButtonList.Right)
            {
                if(btn.Pressed)
                {
                    this._Dragging = true;
                }
                else
                {
                    this._Dragging = false;
                }
            }
            else if(btn.ButtonIndex == (int)ButtonList.WheelDown)
            {
                this.Canvas.Scale *= new Vector2(.95f, .95f);
            }
            else if(btn.ButtonIndex == (int)ButtonList.WheelUp)
            {
                this.Canvas.Scale *= new Vector2(1.05f, 1.05f);
            }
        }

        if(@event is InputEventMouseMotion mtn && this._Dragging && this.Visible)
        {
            this.Canvas.Position += mtn.Relative;
        }
    }

    public void OnVisibleChanged()
    {
        if(this.Visible) 
        { 
            this.GetTree().CallGroup("gp_canvas_gui", "LoadCanvas", this.Canvas);
        }
    }
}
