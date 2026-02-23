namespace Sim2600.Tests;

public class Tests
{
    [Test]
    public async Task StateMatchesExpectedValues()
    {
        const string expectedFilePath = @"Assets\ExpectedStates.txt";
        const string actualFilePath = "ActualStates.txt";
        {
            var sim = new Sim2600Console(@"C:\Code\Sim2600\roms\Pitfall.bin");

            using var stateWriter = new StreamWriter(actualFilePath);

            while (true)
            {
                if (sim.HalfClkCount % 10_000 == 0)
                {
                    sim.WriteState(stateWriter);

                    if (sim.HalfClkCount == 260_000)
                    {
                        break;
                    }
                }

                sim.AdvanceOneHalfClock();
            }
        }

        await Assert.That(File.ReadAllText(actualFilePath)).IsEqualTo(File.ReadAllText(expectedFilePath));
    }
}
