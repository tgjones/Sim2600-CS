using System.Globalization;
using System.Text;

namespace Sim2600;

public sealed class EmuPIA
{
    public int TimerPeriod;
    public byte TimerValue;
    public int TimerClockCount;
    public bool TimerFinished;

    // 128 bytes of RAM
    public readonly byte[] Ram = new byte[128];

    // I/O and timer registers
    public readonly byte[] Iot = new byte[0x297 - 0x280 + 1];

    public string GetState()
    {
        var result = new StringBuilder();
        result.Append($"{TimerPeriod:X8}{TimerValue:X2}{TimerClockCount:X4}{(TimerFinished ? 1 : 0)}");
        foreach (var value in Ram)
        {
            result.Append($"{value:X2}");
        }
        foreach (var value in Iot)
        {
            result.Append($"{value:X2}");
        }
        return result.ToString();
    }

    public void SetState(string state)
    {
        TimerPeriod = int.Parse(state.Substring(0, 8), NumberStyles.HexNumber);
        TimerValue = byte.Parse(state.Substring(8, 2), NumberStyles.HexNumber);
        TimerClockCount = int.Parse(state.Substring(10, 4), NumberStyles.HexNumber);
        TimerFinished = state.Substring(14, 1) == "1" ? true : false;

        for (var i = 0; i < Ram.Length; i++)
        {
            Ram[i] = byte.Parse(state.Substring(15 + i * 2, 2), NumberStyles.HexNumber);
        }

        for (var i = 0; i < Iot.Length; i++)
        {
            Iot[i] = byte.Parse(state.Substring(15 + Ram.Length * 2 + i * 2, 2), NumberStyles.HexNumber);
        }
    }
}