// Stub interfaces for Decal.Adapter.dll - These allow compilation without the actual DLL
// When the real Decal.Adapter.dll is present, these stubs will be ignored
// Note: These are minimal stubs - actual implementation requires the real Decal.Adapter.dll

#if DECAL_STUBS || (!DECAL_ADAPTER_AVAILABLE && !EXISTS_DECAL_DLL)
namespace Decal.Adapter
{
    namespace Wrappers
    {
        public class PluginHost
        {
            public ActionsWrapper Actions { get; set; }
            public ViewWrapper LoadViewResource(string resource) { return new ViewWrapper(); }
            public ViewWrapper LoadView(string xml) { return new ViewWrapper(); }
        }
        
        public class CoreManager
        {
            public CharacterFilterWrapper CharacterFilter { get; set; }
            public WorldFilterWrapper WorldFilter { get; set; }
            public event System.EventHandler<System.EventArgs> RenderFrame;
            public event System.EventHandler<ChatParserInterceptEventArgs> CommandLineText;
        }
        
        public class ActionsWrapper
        {
            public void AddChatText(string text, int color) { }
            public int CurrentSelection { get; set; }
            public CombatState CombatMode { get; set; }
            public void UseItem(int id, int action) { }
            public void SetCombatMode(CombatState state) { }
            public bool ChatState { get; set; }
        }
        
        public class CharacterFilterWrapper
        {
            public int Id { get; set; }
            public int ServerPopulation { get; set; }
        }
        
        public class WorldFilterWrapper
        {
            public WorldObject this[int id] { get { return null; } }
            public System.Collections.Generic.IEnumerable<WorldObject> GetByContainer(int containerId) 
            { 
                yield break; 
            }
        }
        
        public class WorldObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Icon { get; set; }
            public bool HasIdData { get; set; }
            public int Values(LongValueKey key) { return 0; }
            public int Values(LongValueKey key, int defaultValue) { return defaultValue; }
            public Coordinates Coordinates() { return new Coordinates(); }
        }
        
        public class Coordinates
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }
        
        public enum LongValueKey
        {
            Landcell,
            EquippedSlots,
            Slot,
            CurrentMana
        }
        
        public enum CombatState
        {
            Peace,
            Melee
        }
        
        public class ChatParserInterceptEventArgs : System.EventArgs
        {
            public string Text { get; set; }
            public bool Eat { get; set; }
        }
        
        public class LogoffEventArgs : System.EventArgs { }
        
        public class ViewWrapper : System.IDisposable
        {
            public string Title { get; set; }
            public bool Activated { get; set; }
            public System.Drawing.Rectangle Position { get; set; }
            public ControlCollection Controls { get; set; }
            public void SetIcon(int icon, int iconlibrary) { }
            public void Dispose() { }
        }
        
        public class ControlCollection
        {
            public IControlWrapper this[string name] { get { return null; } }
        }
        
        public interface IControlWrapper : System.IDisposable
        {
            int Id { get; }
        }
        
        public class PushButtonWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public System.Drawing.Color TextColor { get; set; }
            public event System.EventHandler<ControlEventArgs> Hit;
            public event System.EventHandler<ControlEventArgs> Click;
            public void Dispose() { }
        }
        
        public class CheckBoxWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public bool Checked { get; set; }
            public event System.EventHandler<CheckBoxChangeEventArgs> Change;
            public void Dispose() { }
        }
        
        public class TextBoxWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public int Caret { get; set; }
            public event System.EventHandler<TextBoxChangeEventArgs> Change;
            public event System.EventHandler<TextBoxEndEventArgs> End;
            public void Dispose() { }
        }
        
        public class ChoiceWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public int Count { get; set; }
            public int Selected { get; set; }
            public ChoiceTextCollection Text { get; set; }
            public ChoiceDataCollection Data { get; set; }
            public event System.EventHandler<IndexChangeEventArgs> Change;
            public void Add(string text, object data) { }
            public void Remove(int index) { }
            public void Clear() { }
            public void Dispose() { }
        }
        
        public class ChoiceTextCollection
        {
            public string this[int index] { get { return null; } set { } }
        }
        
        public class ChoiceDataCollection
        {
            public object this[int index] { get { return null; } set { } }
        }
        
        public class SliderWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public int SliderPostition { get; set; }
            public int Maximum { get; set; }
            public int Minimum { get; set; }
            public event System.EventHandler<IndexChangeEventArgs> Change;
            public void Dispose() { }
        }
        
        public class ListWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public int RowCount { get; set; }
            public int ColCount { get; set; }
            public int ScrollPosition { get; set; }
            public ListRowCollection this[int row] { get { return null; } }
            public event System.EventHandler<ListSelectEventArgs> Selected;
            public void Clear() { }
            public void Add() { }
            public void Insert(int pos) { }
            public void Delete(int index) { }
            public void Dispose() { }
        }
        
        public class ListRowCollection
        {
            public ListCellCollection this[int col] { get { return null; } }
        }
        
        public class ListCellCollection
        {
            public System.Drawing.Color Color { get; set; }
            public int Width { get; set; }
            public object this[int subval] { get { return null; } set { } }
        }
        
        public class StaticWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public void Dispose() { }
        }
        
        public class NotebookWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public int ActiveTab { get; set; }
            public event System.EventHandler<IndexChangeEventArgs> Change;
            public void Dispose() { }
        }
        
        public class ProgressWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public int Value { get; set; }
            public int MaxValue { get; set; }
            public string PreText { get; set; }
            public void Dispose() { }
        }
        
        public class ButtonWrapper : IControlWrapper
        {
            public int Id { get; set; }
            public event System.EventHandler<ControlEventArgs> Click;
            public void SetImages(int unpressed, int pressed) { }
            public void SetImages(int hmodule, int unpressed, int pressed) { }
            public int Background { set { } }
            public System.Drawing.Color Matte { set { } }
            public void Dispose() { }
        }
        
        public abstract class PluginBase
        {
            protected PluginHost Host { get; set; }
            protected CoreManager Core { get; set; }
            protected virtual void Startup() { }
            protected virtual void Shutdown() { }
        }
        
        [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method)]
        public class WireUpBaseEventsAttribute : System.Attribute { }
        
        [System.AttributeUsage(System.AttributeTargets.Method)]
        public class BaseEventAttribute : System.Attribute
        {
            public BaseEventAttribute(string eventName, string filter) { }
        }
        
        [System.AttributeUsage(System.AttributeTargets.Class)]
        public class FriendlyNameAttribute : System.Attribute
        {
            public FriendlyNameAttribute(string name) { }
        }
    }
    
    // Event args classes in Decal.Adapter namespace (not Wrappers)
    public class ControlEventArgs : System.EventArgs { }
    public class CheckBoxChangeEventArgs : System.EventArgs { }
    public class TextBoxChangeEventArgs : System.EventArgs
    {
        public string Text { get; set; }
    }
    public class TextBoxEndEventArgs : System.EventArgs
    {
        public bool Success { get; set; }
    }
    public class IndexChangeEventArgs : System.EventArgs
    {
        public int Index { get; set; }
    }
    public class ListSelectEventArgs : System.EventArgs
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
#endif
