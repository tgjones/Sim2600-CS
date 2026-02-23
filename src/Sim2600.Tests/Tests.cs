namespace Sim2600.Tests;

public class Tests
{
    [Test]
    public async Task StateMatchesExpectedValues()
    {
        Console.WriteLine("Current directory: " + Environment.CurrentDirectory);

        var expectedFilePath = Path.Combine("Assets", "ExpectedStates.txt");
        const string actualFilePath = "ActualStates.txt";
        {
            var sim = new Sim2600Console(Path.Combine("Assets", "Roms", "Pitfall.bin"));

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
