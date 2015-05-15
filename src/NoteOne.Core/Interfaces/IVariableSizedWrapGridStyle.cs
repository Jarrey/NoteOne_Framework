namespace NoteOne_Core.Interfaces
{
    public enum VariableSizedWrapGridStyleKeys
    {
        Style1x1 = 11,
        Style1x2 = 12,
        Style2x1 = 21,
        Style2x2 = 22,
        Style2x3 = 23,
        Style3x2 = 32,
        Style1x3 = 13,
        Style3x1 = 31,
        Style3x3 = 33,
        Style4x2 = 42,
        Style2x4 = 24,
        Style4x4 = 44
    }

    public interface IVariableSizedWrapGridStyle
    {
        VariableSizedWrapGridStyleKeys VariableSizedWrapGridStyleKey { get; set; }
    }
}