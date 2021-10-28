using System;
using System.Timers;

namespace genus.lib
{
    public class VirtualMachine
    {
        private readonly Chip8Interpreter interpreter;
        private readonly Timer cpuClock;
        private readonly Timer gfxClock;

        public byte[] GfxBuffer => interpreter.gfx;

        public VirtualMachine()
        {
            interpreter = new();

            cpuClock = new(0.5);
            gfxClock = new(16.67);
        }

        public void Initialize(byte[] program)
        {
            interpreter.ResetChip();
            interpreter.LoadGame(program);

            cpuClock.Elapsed += CpuClockCycleElapsed;
            gfxClock.Elapsed += GfxClockCycleElapsed;
        }

        public void Start()
        {
            cpuClock.Start();
            gfxClock.Start();
        }

        private void GfxClockCycleElapsed(object sender, ElapsedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void CpuClockCycleElapsed(object sender, ElapsedEventArgs e)
        {
            interpreter.EmulateCycle();
        }
    }
}