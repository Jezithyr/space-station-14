using System.Linq;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared.Medical.SuitSensor;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client.Medical.CrewMonitoring
{
    [GenerateTypedNameReferences]
    public sealed partial class CrewMonitoringWindow : FancyWindow
    {
        private List<Control> _rowsContent = new();
        private List<(DirectionIcon Icon, Vector2 Position)> _directionIcons = new();
        private readonly IEntityManager _entManager;
        private readonly IEyeManager _eye;
        private EntityUid? _stationUid;
        private CrewMonitoringButton? _trackedButton;

        public static int IconSize = 16; // XAML has a `VSeparationOverride` of 20 for each row.

        public CrewMonitoringWindow(EntityUid? mapUid)
        {
            RobustXamlLoader.Load(this);
            _eye = IoCManager.Resolve<IEyeManager>();
            _entManager = IoCManager.Resolve<IEntityManager>();
            _stationUid = mapUid;

            if (_entManager.TryGetComponent<TransformComponent>(mapUid, out var xform))
            {
                NavMap.MapUid = xform.GridUid;
            }
            else
            {
                NavMap.Visible = false;
                SetSize = new Vector2(775, 400);
                MinSize = SetSize;
            }
        }

        public void ShowSensors(List<SuitSensorStatus> stSensors, EntityCoordinates? monitorCoords, bool snap, float precision)
        {
            ClearAllSensors();

            var monitorCoordsInStationSpace = _stationUid != null ? monitorCoords?.WithEntityId(_stationUid.Value, _entManager).Position : null;

            // TODO scroll container
            // TODO filter by name & occupation
            // TODO make each row a xaml-control. Get rid of some of this c# control creation.
            if (stSensors.Count == 0)
            {
                NoServerLabel.Visible = true;
                return;
            }
            NoServerLabel.Visible = false;

            // add a row for each sensor
            foreach (var sensor in stSensors.OrderBy(a => a.Name))
            {
                // add button with username
                var nameButton = new CrewMonitoringButton()
                {
                    SuitSensorUid = sensor.SuitSensorUid,
                    Coordinates = sensor.Coordinates,
                    Text = sensor.Name,
                    Margin = new Thickness(5f, 5f),
                };
                if (sensor.SuitSensorUid == _trackedButton?.SuitSensorUid)
                    nameButton.AddStyleClass(StyleNano.StyleClassButtonColorGreen);
                SensorsTable.AddChild(nameButton);
                _rowsContent.Add(nameButton);

                // add users job
                // format: JobName
                var jobLabel = new Label()
                {
                    Text = sensor.Job,
                    HorizontalExpand = true
                };
                SensorsTable.AddChild(jobLabel);
                _rowsContent.Add(jobLabel);

                // add users status and damage
                // format: IsAlive (TotalDamage)
                var statusText = Loc.GetString(sensor.IsAlive ?
                    "crew-monitoring-user-interface-alive" :
                    "crew-monitoring-user-interface-dead");
                if (sensor.TotalDamage != null)
                {
                    statusText += $" ({sensor.TotalDamage})";
                }
                var statusLabel = new Label()
                {
                    Text = statusText
                };
                SensorsTable.AddChild(statusLabel);
                _rowsContent.Add(statusLabel);

                // add users positions
                // format: (x, y)
                var box = GetPositionBox(sensor.Coordinates, monitorCoordsInStationSpace ?? Vector2.Zero, snap, precision);

                SensorsTable.AddChild(box);
                _rowsContent.Add(box);

                if (sensor.Coordinates != null && NavMap.Visible)
                {
                    NavMap.TrackedCoordinates.TryAdd(sensor.Coordinates.Value,
                        (true, sensor.SuitSensorUid == _trackedButton?.SuitSensorUid ? StyleNano.PointGreen : StyleNano.PointRed));

                    nameButton.OnButtonUp += args =>
                    {
                        if (_trackedButton != null && _trackedButton?.Coordinates != null)
                            //Make previous point red
                            NavMap.TrackedCoordinates[_trackedButton.Coordinates.Value] = (true, StyleNano.PointRed);

                        NavMap.TrackedCoordinates[sensor.Coordinates.Value] = (true, StyleNano.PointGreen);
                        NavMap.CenterToCoordinates(sensor.Coordinates.Value);

                        nameButton.AddStyleClass(StyleNano.StyleClassButtonColorGreen);
                        if (_trackedButton != null)
                        {   //Make previous button default
                            var previosButton = SensorsTable.GetChild(_trackedButton.IndexInTable);
                            previosButton.RemoveStyleClass(StyleNano.StyleClassButtonColorGreen);
                        }
                        _trackedButton = nameButton;
                        _trackedButton.IndexInTable = nameButton.GetPositionInParent();
                    };
                }
            }
            // Show monitor point
            if (monitorCoords != null)
                NavMap.TrackedCoordinates.Add(monitorCoords.Value, (true, StyleNano.PointMagenta));
        }

        private BoxContainer GetPositionBox(EntityCoordinates? coordinates, Vector2 monitorCoordsInStationSpace, bool snap, float precision)
        {
            var box = new BoxContainer() { Orientation = LayoutOrientation.Horizontal };

            if (coordinates == null || _stationUid == null)
            {
                var dirIcon = new DirectionIcon()
                {
                    SetSize = (IconSize, IconSize),
                    Margin = new(0, 0, 4, 0)
                };
                box.AddChild(dirIcon);
                box.AddChild(new Label() { Text = Loc.GetString("crew-monitoring-user-interface-no-info") });
            }
            else
            {
                var local = coordinates.Value.WithEntityId(_stationUid.Value, _entManager).Position;

                var displayPos = local.Floored();
                var dirIcon = new DirectionIcon(snap, precision)
                {
                    SetSize = (IconSize, IconSize),
                    Margin = new(0, 0, 4, 0)
                };
                box.AddChild(dirIcon);
                box.AddChild(new Label() { Text = displayPos.ToString() });
                _directionIcons.Add((dirIcon, local - monitorCoordsInStationSpace));
            }

            return box;
        }

        protected override void FrameUpdate(FrameEventArgs args)
        {
            // the window is separate from any specific viewport, so there is no real way to get an eye-rotation without
            // using IEyeManager. Eventually this will have to be reworked for a station AI with multi-viewports.
            // (From the future: Or alternatively, just disable the angular offset for station AIs?)

            // An offsetAngle of zero here perfectly aligns directions to the station map.
            // Note that the "relative angle" does this weird inverse-inverse thing.
            // Could recalculate it all in world coordinates and then pass in eye directly... or do this.
            var offsetAngle = Angle.Zero;
            if (_entManager.TryGetComponent<TransformComponent>(_stationUid, out var xform))
            {
                // Apply the offset relative to the eye.
                // For a station at 45 degrees rotation, the current eye rotation is -45 degrees.
                // TODO: This feels sketchy. Is there something underlying wrong with eye rotation?
                offsetAngle = -(_eye.CurrentEye.Rotation + xform.WorldRotation);
            }

            foreach (var (icon, pos) in _directionIcons)
            {
                icon.UpdateDirection(pos, offsetAngle);
            }
        }

        private void ClearAllSensors()
        {
            foreach (var child in _rowsContent)
            {
                SensorsTable.RemoveChild(child);
            }
            _rowsContent.Clear();
            _directionIcons.Clear();
            NavMap.TrackedCoordinates.Clear();
        }
    }

    public sealed class CrewMonitoringButton : Button
    {
        public int IndexInTable;
        public EntityUid? SuitSensorUid;
        public EntityCoordinates? Coordinates;
    }
}
