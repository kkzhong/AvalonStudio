using AvalonStudio.Projects;
using AvaloniaEdit.Document;
using Avalonia.Media;

namespace AvalonStudio.Languages
{
    public enum DiagnosticLevel
    {
        Hidden = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4
    }

    public enum DiagnosticCategory
    {
        Style,
        Compiler,
        EditAndContinue
    }

    public class Diagnostic : TextSegment
    {
        public Diagnostic(int offset, int length, IProject project, string file, int line, string message, DiagnosticLevel level, DiagnosticCategory category)
        {
            StartOffset = offset;
            Length = length;
            Project = project;
            File = file;
            Line = line;
            Spelling = message;
            Level = level;
            Category = category;
        }

        public static readonly Color ErrorBrush = Color.FromRgb(253, 45, 45);
        public static readonly Color WarningBrush = Color.FromRgb(255, 207, 40);
        public static readonly Color DefaultBrush = Color.FromRgb(0, 42, 74);

        public IProject Project { get; }
        public int Line { get; }
        public string File { get; }
        public string Spelling { get; }
        public DiagnosticLevel Level { get; }
        public DiagnosticCategory Category { get; }
    }
}