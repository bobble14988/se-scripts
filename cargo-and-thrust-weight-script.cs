public Program() { 
  Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

double NEWTONS_PER_KG = 9.81d;
int ROUND = 2;

public void Main(string argument, UpdateType updateSource) {
  StringBuilder sb = new StringBuilder();

  var totalGridWeightKg = GetGridMass();
  var totalGridWeightN = totalGridWeightKg * NEWTONS_PER_KG;
  var totalMaxEffectiveThrust = GetGridThrust();

  sb.Append($"T:W Up: {Math.Round(totalMaxEffectiveThrust.Up / totalGridWeightN, ROUND)}\n");
  sb.Append($"T:W Back: {Math.Round(totalMaxEffectiveThrust.Back / totalGridWeightN, ROUND)}\n");

  var cargoContainers = new List<IMyCargoContainer>();
  GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers, x => x.CubeGrid == Me.CubeGrid);

  var cargoContainersSorted = cargoContainers.OrderBy(x => x.DisplayNameText).ToList();

  foreach (var cargoContainer in cargoContainersSorted) {
    var name = cargoContainer.DisplayNameText;
    var currentVolume = cargoContainer.GetInventory().CurrentVolume;
    var maxVolume = cargoContainer.GetInventory().MaxVolume;
    var percentFull = GetPercentage(currentVolume.RawValue, maxVolume.RawValue);

    sb.Append(name + ": " + percentFull + "%\n");
  }

  var panel = GridTerminalSystem.GetBlockWithName("Text Panel (Grinder Ship)") as IMyTextPanel;
  panel.WriteText(sb.ToString(), false);
}

private double GetPercentage(long x, long y) {
  var percentage = ((double)x/(double)y)*100;

  return Math.Round(percentage, ROUND);
}

private double GetTtw(double thrustN, double weightKg) {
  var weightN = weightKg * NEWTONS_PER_KG;

  return Math.Round(thrustN / weightN, ROUND);
}

private double GetGridMass() {
  var cockpits = new List<IMyShipController>();
  GridTerminalSystem.GetBlocksOfType<IMyShipController>(cockpits, x => x.CubeGrid == Me.CubeGrid);

  var gridMass = cockpits.First().CalculateShipMass().TotalMass;

  Echo("Grid's mass is: " + gridMass);

  return Math.Round(gridMass, ROUND);
}

private GridMaxEffectiveThrust GetGridThrust() {
  var thrustersUp = new List<IMyThrust>();
  var thrustersBack = new List<IMyThrust>();

  GridTerminalSystem.GetBlocksOfType<IMyThrust>(
    thrustersUp, 
    x => (x.CubeGrid == Me.CubeGrid) && 
            (Vector3I.GetDominantDirection(x.GridThrustDirection) == CubeFace.Down)
  );

  GridTerminalSystem.GetBlocksOfType<IMyThrust>(
    thrustersBack, 
    x => (x.CubeGrid == Me.CubeGrid) && 
            (Vector3I.GetDominantDirection(x.GridThrustDirection) == CubeFace.Forward)
  );

  var totalMaxEffectiveThrustUp = 0.0d;
  foreach (var thruster in thrustersUp) {
    Echo($"{thruster.DisplayNameText}");

    totalMaxEffectiveThrustUp += thruster.MaxEffectiveThrust;
  }

  var totalMaxEffectiveThrustBack = 0.0d;
  foreach (var thruster in thrustersBack) {
    Echo($"{thruster.DisplayNameText}");

    totalMaxEffectiveThrustBack += thruster.MaxEffectiveThrust;
  }

  return new GridMaxEffectiveThrust {
    Up = totalMaxEffectiveThrustUp,
    Back = totalMaxEffectiveThrustBack
  };
}

public class GridMaxEffectiveThrust {
  public double Up { get; set; }
  public double Back { get; set; }
}
