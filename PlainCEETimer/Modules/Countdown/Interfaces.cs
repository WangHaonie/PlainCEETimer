using PlainCEETimer.Modules.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Countdown
{
    public delegate void CountdownUpdatedHandler(string Content, Color Fore, Color Back);

    public interface ICountdownProvider : IDisposable
    {
        event CountdownUpdatedHandler CountdownUpdated;
        bool IsRunning { get; }
        void Start();
        void JumpTo(int index);
    }
}
