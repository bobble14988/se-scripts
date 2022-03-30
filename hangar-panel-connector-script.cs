private const string LINE_SEPARATOR = "------------------------";
private const string VACANT = "VACANT";
private const string PH_START = "<";
private const string PH_END = ">";

public Program() { 
  Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource) {
  var bindingsList = new List<ConnectorPanelBinding> {
    {
      new ConnectorPanelBinding {
        PanelName = "Panel - Hangar Utility Bay 1/Depo",
        ConnectorTopName = "Connector (Utility Bay 1)",
        ConnectorBottomName = "Connector (Depository)",
      }
    },
    {
      new ConnectorPanelBinding {
        PanelName = "Panel - Hangar Utility Bay 2/4",
        ConnectorTopName = "Connector (Utility Bay 2)",
        ConnectorBottomName = "Connector (Utility Bay 4)",
      }
    },
    {
      new ConnectorPanelBinding {
        PanelName = "Panel - Hangar Utility Bay 3/5",
        ConnectorTopName = "Connector (Utility Bay 3)",
        ConnectorBottomName = "Connector (Utility Bay 5)",
      }
    },
    {
      new ConnectorPanelBinding {
        PanelName = "Panel - Hangar Fighter Bay 1/Ammo Load",
        ConnectorTopName = "Connector (Fighter Bay 1)",
        ConnectorBottomName = "Connector (Ammo Loading Bay)",
      }
    },
    {
      new ConnectorPanelBinding {
        PanelName = "Panel - Hangar Fighter Bay 2/4",
        ConnectorTopName = "Connector (Fighter Bay 2)",
        ConnectorBottomName = "Connector (Fighter Bay 4)",
      }
    },
    {
      new ConnectorPanelBinding {
        PanelName = "Panel - Hangar Fighter Bay 3/5",
        ConnectorTopName = "Connector (Fighter Bay 3)",
        ConnectorBottomName = "Connector (Fighter Bay 5)",
      }
    }
  };

  foreach(ConnectorPanelBinding binding in bindingsList) {
    Echo("**************************");

    var panel = GridTerminalSystem.GetBlockWithName(binding.PanelName) as IMyTextPanel;
    var connectorTop = GridTerminalSystem.GetBlockWithName(binding.ConnectorTopName) as IMyShipConnector;
    var connectorBottom = GridTerminalSystem.GetBlockWithName(binding.ConnectorBottomName) as IMyShipConnector;

    Echo("Found panel: " + panel.DisplayNameText);
    Echo("With top connector: " + connectorTop.DisplayNameText + ", with status: " + connectorTop.Status);
    Echo("With bottom connector: " + connectorBottom.DisplayNameText + ", with status: " + connectorBottom.Status);

    var panelTextSplit = panel.GetText().Split(new string[] { LINE_SEPARATOR }, StringSplitOptions.None);
    var topText = panelTextSplit[0];
    var bottomText = panelTextSplit[1];

    Echo("Top text:\n" + topText + "\n");
    Echo("Bottom text:\n" + bottomText + "\n");

    if (connectorTop.Status == MyShipConnectorStatus.Connected) {
      var shipName = connectorTop.OtherConnector.CubeGrid.CustomName;
      topText = ReplaceTextBetweenPlaceholders(topText, shipName);
    } else {
      topText = ReplaceTextBetweenPlaceholders(topText, VACANT);
    }

    if (connectorBottom.Status == MyShipConnectorStatus.Connected) {
      var shipName = connectorBottom.OtherConnector.CubeGrid.CustomName;
      bottomText = ReplaceTextBetweenPlaceholders(bottomText, shipName);
    } else {
      bottomText = ReplaceTextBetweenPlaceholders(bottomText, VACANT);
    }

    panel.WriteText(topText + LINE_SEPARATOR + bottomText);

    Echo("**************************");
  }
}

private string ReplaceTextBetweenPlaceholders(string textContainingPlaceholders, string replacementText) {
  int startIndex = textContainingPlaceholders.IndexOf(PH_START);
  int endIndex = textContainingPlaceholders.IndexOf(PH_END);
  string textToReplace = textContainingPlaceholders.Substring(startIndex, endIndex - startIndex + 1);
  
  return textContainingPlaceholders.Replace(textToReplace, PH_START + replacementText + PH_END);
}

public class ConnectorPanelBinding {
  public String PanelName { get; set; }
  public String ConnectorTopName { get; set; }
  public String ConnectorBottomName { get; set; }
}