using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame.LogTypes
{
    // base log class returned from HattingtonGameEngine
    public abstract class EngineLog
    {
        // If the event did not take place, log is not valid
        public bool IsValid { get; set; }

        // Error is set if IsValid if false (Tells reason why fight is not valid)
        public string Error { get; set; }

        public string CharacterName { get; set; }
    }
}
