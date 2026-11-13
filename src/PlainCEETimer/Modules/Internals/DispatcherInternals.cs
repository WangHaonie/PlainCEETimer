using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Threading;

internal static class DispatcherInternals
{
    private static FieldInfo s_fiFrameDepth;

    extension(Dispatcher instance)
    {
        public int _frameDepth
        {
            get
            {
                if (s_fiFrameDepth == null)
                {
                    typeof(Dispatcher).RevealMember(out s_fiFrameDepth);
                }

                return (int)s_fiFrameDepth.GetValue(instance);
            }
        }
    }
}
