public Program() { 
  Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource) {
  int DOOR_CLOSE_SECONDS = 3;

  var doors = new List<IMyDoor>();
  GridTerminalSystem.GetBlocksOfType<IMyDoor>(doors, x => x.CubeGrid == Me.CubeGrid);

  int nowUnix = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

  foreach (var door in doors) {
    if (door.Status == DoorStatus.Open) {
      if (String.IsNullOrEmpty(door.CustomData)) {
        door.CustomData = nowUnix.ToString();
      } else {
        if (int.Parse(door.CustomData) + DOOR_CLOSE_SECONDS >= nowUnix) {
          door.CloseDoor();
          door.CustomData = "";
        }
      }
    }
  }
}