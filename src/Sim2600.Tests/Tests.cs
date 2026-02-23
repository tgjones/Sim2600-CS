namespace Sim2600.Tests;

public class Tests
{
    [Test]
    public async Task StateMatchesExpectedValues()
    {
        var expectedFilePath = Path.Combine("Assets", "ExpectedStates.txt");
        const string actualFilePath = "ActualStates.txt";

        {
            var sim = new Sim2600Console(Path.Combine("Assets", "Roms", "Pitfall.bin"));
            var imageWriter = new ImageWriter();

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

                // Get pixel color when TIA clock (~3mHz) is low
                var tia = sim.SimTIA;
                if (tia.IsLow(tia.PadIndClk0))
                {
                    var restartImage = false;
                    if (tia.IsHigh(tia.VSync))
                    {
                        Console.WriteLine($"VSYNC high at TIA halfclock {tia.HalfClkCount}");
                        restartImage = true;
                    }

                    // grayscale
                    // lum = self.simTIA.get3BitLuminance() << 5
                    // rgba = (lum << 24) | (lum << 16) | (lum << 8) | 0xFF

                    // color
                    var rgba = tia.ColorRgba8;

                    if (restartImage)
                    {
                        imageWriter.RestartImage();
                    }
                    imageWriter.SetNextPixel(rgba);

                    if (tia.IsHigh(tia.VBlank))
                    {
                        Console.WriteLine($"VBLANK at TIA halfclock {tia.HalfClkCount}");
                    }
                }
            }
        }

        await Assert.That(File.ReadAllText(actualFilePath)).IsEqualTo(File.ReadAllText(expectedFilePath));
    }
}
