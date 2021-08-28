using Newtonsoft.Json;

using Godot;

using PichaLib;

public partial class CanvasView : VBoxContainer
{
    public void Save()
    {
        if(this.Active != null)
        {
            if(this.FileExists) 
                { this.Active.Save(); }
            else 
                { this.GetTree().CallGroup("gp_filebrowse", "OpenDialog", DialogMode.SAVE_CANVAS_AS_NEW); }
        }
    }

    public void SaveCanvas()
    {
        if(this.Active != null)
        {
            this.GetTree().CallGroup("gp_filebrowse", "OpenDialog", DialogMode.SAVE_CANVAS_AS_NEW);
        }
    }
    
    public void AddCanvas(GenCanvas c)
    {
        var _i = this.Content.GetChildren().Count;
        var _view = new CanvasContainer();
        this.Content.AddChild(_view);
        this.Tabs.AddTab(c.CanvasName);

        _view.Canvas = c;

        if(c.Data == null) { c.Data = new Canvas(); }

        this.Tabs.CurrentTab = _i;
        this.Content.CurrentTab = _i;
        this.Tabs.SetTabTitle(this.Content.CurrentTab, c.CanvasName);
        this.Active.CanvasChanges.Add(this.Active.SaveData());
    }

    public void UndoChange()
    {
        if(this.Active != null)
        {
            var _container = this.Active.GetParent() as CanvasContainer;
            var _new = new GenCanvas();
            int _index = this.Active.CanvasChanges.Count - 2;
            _new.LoadData(this.Active.CanvasChanges[_index]);
            _container.Canvas = _new;
            this.GetTree().CallGroup("gp_canvas_gui", "LoadCanvas", _new);
            this.GetTree().CallGroup("gp_layer_gui", "LoadCanvas", _new);
        }
    }

    public void NameCurrentTab(string s)
    {
        this.Tabs.SetTabTitle(this.Content.CurrentTab, s);
    }

    public void ExportTest()
    {
        GD.Print("EXPORT");
        var _images = new ExportManager() {Canvas = this.Active.SaveData()};
        var _sprite = _images.GetSpriteSheet(1, 3, 30);

        _sprite.SavePng("test.png");
    }

    public void WriteFile(string f)
    {
        if(this.Active != null)
        {
            this.Active.SaveAsFile(f);
        }
    }
    
    public void OpenCanvas(string path)
    {
        var _dat = JsonConvert.DeserializeObject<Canvas>(System.IO.File.ReadAllText(path));
        var _can = new GenCanvas();

        _can.LoadData(_dat);

        _can.PathName = path;
        _can.FileExists = true;
        _can.FileSaved = true;

        this.AddCanvas(_can);
    }

    public void AddLayer(GenLayer l)
    {
        if(this.Active != null)
        {
            this.Active.AddLayer(l);
            this.GetTree().CallGroup("gp_layer_gui", "AddLayer", l);
        }
        else {
            this.AddCanvas(new GenCanvas());
            this.AddLayer(l);
        }
    }
}