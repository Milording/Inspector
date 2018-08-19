using System;

namespace Inspector
{
    public class SourceState
    {
        public int Id { get; set; }
        public DateTime FixationTime { get; set; }
        public string SourceCode { get; set; }

        public SourceState()
        {

        }

        public SourceState(string sourceCode)
        {
            SourceCode = sourceCode;
            FixationTime = DateTime.UtcNow;
        }
    }

    public class InspectedLink
    {
        public int Id { get; set; }
        public string InspectionLink { get; set; }
    }

    public class LightSourceState
    {
        public int Id { get; set; }
        public DateTime FixationTime { get; set; }

        public LightSourceState(int id, DateTime fixationTime)
        {
            Id = id;
            FixationTime = fixationTime;
        }
    }
}
