// Stub interfaces for VirindiViewService.dll - These allow compilation without the actual DLL
// When the real VirindiViewService.dll is present, these stubs will be ignored
// Note: These are minimal stubs - actual implementation requires the real VirindiViewService.dll

#if VVS_STUBS || (!VVS_AVAILABLE && VVS_REFERENCED)
namespace VirindiViewService
{
    public static class Service
    {
        public static bool Running { get { return false; } }
        public static bool PortalBitmapExists(int id) { return false; }
    }
    
    public class HudView : System.IDisposable
    {
        public static HudControl FocusControl { get; set; }
        public string Title { get; set; }
        public bool Visible { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public System.Drawing.Point Location { get; set; }
        public System.Drawing.Size ClientArea { get; set; }
        public ACImage Icon { get; set; }
        
        public HudView(ViewProperties props, ControlGroup group) { }
        public HudView(ViewProperties props, ControlGroup group, string windowKey) { }
        
        public Controls.HudControl this[string name] { get { return null; } }
        public void Dispose() { }
    }
    
    public class HudControl
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public bool HasFocus { get; set; }
        public event System.EventHandler LostFocus;
    }
    
    public class ACImage
    {
        public int PortalImageID { get; set; }
        public ACImage() { }
        public ACImage(int id) { PortalImageID = id; }
        public ACImage(int id, eACImageDrawOptions options) { PortalImageID = id; }
        public ACImage(System.Drawing.Color color) { }
        public static ACImage FromIconLibrary(int icon, int library) { return new ACImage(); }
        
        // Implicit conversion from int to ACImage
        public static implicit operator ACImage(int id) { return new ACImage(id); }
        
        public enum eACImageDrawOptions
        {
            DrawStretch
        }
    }
    
    namespace XMLParsers
    {
        public class Decal3XMLParser
        {
            public void ParseFromResource(string resource, out ViewProperties props, out ControlGroup group)
            {
                props = new ViewProperties();
                group = new ControlGroup();
            }
            public void Parse(string xml, out ViewProperties props, out ControlGroup group)
            {
                props = new ViewProperties();
                group = new ControlGroup();
            }
        }
    }
    
    public class ViewProperties { }
    public class ControlGroup
    {
        public Controls.HudControl HeadControl { get; set; }
        public Controls.HudControl ParentOf(string name) { return null; }
    }
    
    namespace Controls
    {
        public class HudButton : HudControl
        {
            public string Text { get; set; }
            public event System.EventHandler<ControlMouseEventArgs> MouseEvent;
        }
        
        public class HudCheckBox : HudControl
        {
            public string Text { get; set; }
            public bool Checked { get; set; }
            public event System.EventHandler Change;
        }
        
        public class HudTextBox : HudControl
        {
            public string Text { get; set; }
            public event System.EventHandler Change;
        }
        
        public class HudCombo : HudControl
        {
            public int Count { get; set; }
            public int Current { get; set; }
            public event System.EventHandler Change;
            public void AddItem(string text, object data) { }
            public void InsertItem(int index, string text, object data) { }
            public void DeleteItem(int index) { }
            public void Clear() { }
            public HudComboItem this[int index] { get { return new HudComboItem(); } }
        }
        
        public class HudComboItem : HudStaticText { }
        
        public class HudHSlider : HudControl
        {
            public int Position { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }
            public event LinearPositionControl.delScrollChanged Changed;
        }
        
        public class HudList : HudControl
        {
            public int RowCount { get; set; }
            public int ColumnCount { get; set; }
            public int ScrollPosition { get; set; }
            public event delClickedControl Click;
            public void ClearRows() { }
            public void AddRow() { }
            public void InsertRow(int pos) { }
            public void RemoveRow(int index) { }
            public HudListRow this[int row] { get { return new HudListRow(); } }
            public delegate void delClickedControl(object sender, int row, int col);
        }
        
        public class HudListRow
        {
            public HudControl this[int col] { get { return new HudStaticText(); } }
        }
        
        public class HudStaticText : HudControl
        {
            public string Text { get; set; }
            public System.Drawing.Color TextColor { get; set; }
            public System.Drawing.Rectangle ClipRegion { get; set; }
            public void ResetTextColor() { }
        }
        
        public class HudTabView : HudControl
        {
            public int CurrentTab { get; set; }
            public event System.EventHandler OpenTabChange;
            public void Invalidate() { }
        }
        
        public class HudProgressBar : HudControl
        {
            public int Position { get; set; }
            public int Max { get; set; }
            public string PreText { get; set; }
        }
        
        public class HudImageButton : HudControl
        {
            public ACImage Image_Up { get; set; }
            public ACImage Image_Up_Pressing { get; set; }
            public ACImage Image_Background { get; set; }
            public ACImage Image_Background2 { get; set; }
        }
        
        public class HudFixedLayout : HudControl
        {
            public System.Drawing.Rectangle GetControlRect(HudControl control) { return new System.Drawing.Rectangle(); }
            public void SetControlRect(HudControl control, System.Drawing.Rectangle rect) { }
        }
        
        public class HudPictureBox : HudControl
        {
            public ACImage Image { get; set; }
        }
        
        public class HudListCell : HudControl { }
        
        public class HudControl : System.IDisposable
        {
            public string Name { get; set; }
            public bool Visible { get; set; }
            public bool HasFocus { get; set; }
            public int XMLID { get; set; }
            public ControlGroup Group { get; set; }
            public event System.EventHandler LostFocus;
            public event System.EventHandler<ControlMouseEventArgs> MouseEvent;
            public void Dispose() { }
        }
        
        public class ControlMouseEventArgs : System.EventArgs
        {
            public MouseEventType EventType { get; set; }
            public enum MouseEventType
            {
                MouseHit,
                MouseDown
            }
        }
        
        public class LinearPositionControl
        {
            public delegate void delScrollChanged(int min, int max, int pos);
        }
    }
    
    public static class TooltipSystem
    {
        public class cTooltipInfo
        {
            public string Text { get; set; }
        }
        
        public static cTooltipInfo AssociateTooltip(Controls.HudControl control, string text) { return new cTooltipInfo(); }
        public static void RemoveTooltip(cTooltipInfo info) { }
    }
}
#endif
