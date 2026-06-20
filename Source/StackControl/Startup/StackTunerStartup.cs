using Verse;

namespace StackTuner
{
    [StaticConstructorOnStartup]
    public static class StackTunerStartup
    {
        static StackTunerStartup()
        {
            StackLimitApplier.Apply(logResult: true);
        }
    }
}