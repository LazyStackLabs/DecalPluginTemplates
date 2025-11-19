# DragonMoonNavRecorder

A Decal plugin for Asheron's Call that records player movement in real-time and saves navigation routes to `.nav` files.

## Features

- **Real-time Recording**: Records player position (x, y, z, landcell, timestamp) continuously
- **Background Thread**: Uses a separate background thread so recording never blocks the client
- **Hotkey Support**: Press F9 to toggle recording on/off
- **UI Controls**: VVS (Virindi View Service) interface with buttons for Start/Stop/Save/Clear
- **Command Support**: Alternative command-line controls for all functions
- **Clean Code**: Well-commented, modern C# implementation

## Requirements

- Decal 3.0 or later
- Virindi View Service (VVS) - for the UI interface
- .NET Framework 2.0
- Visual Studio or MSBuild for compilation

## Building the Plugin

1. Open `DragonMoonNavRecorder.sln` in Visual Studio (or use MSBuild from command line)
2. Update the DLL references in `DragonMoonNavRecorder.csproj`:
   - `Decal.Adapter.dll` should point to: `C:\Games\Decal 3.0\Decal.Adapter.dll`
   - `VirindiViewService.dll` should point to: `C:\Games\VirindiPlugins\VirindiViewService\VirindiViewService.dll`
3. Build the solution (Debug or Release configuration)
4. The compiled DLL will be in `bin\Debug\` or `bin\Release\`

## Loading into Decal

1. **Copy the DLL**: Copy `DragonMoonNavRecorder.dll` to your Decal plugins directory:
   - Default location: `C:\Games\Decal 3.0\Plugins\`

2. **Load the Plugin**:
   - Open Decal Agent (the system tray application)
   - Go to the Plugins tab
   - Find "DragonMoonNavRecorder" in the list
   - Check the box to enable it
   - The plugin will load when you log into the game

3. **Access the UI**:
   - Once in-game, the plugin window should appear automatically
   - If not visible, check the Decal plugin menu in-game
   - The window title is "DragonMoon Nav Recorder"

## Usage

### Using the UI Buttons

- **Start Recording**: Begins recording your movement
- **Stop Recording**: Stops recording (data is preserved)
- **Save Route**: Saves all recorded points to a `.nav` file
- **Clear Route**: Clears all recorded points (only when not recording)

### Using Hotkeys

- **F9**: Toggle recording on/off

### Using Commands

Type these commands in the chat window:

- `/navtoggle` or `/navrecord` - Toggle recording on/off
- `/navstart` - Start recording
- `/navstop` - Stop recording
- `/navsave` - Save current route to file
- `/navclear` - Clear all recorded points

### File Output

Routes are saved to:
```
My Documents\Asheron's Call\NavRoutes\NavRoute_YYYY-MM-DD_HH-mm-ss.nav
```

### File Format

The `.nav` file format is CSV with the following structure:
```
# DragonMoonNavRecorder Navigation File
# Format: X, Y, Z, Landcell, Timestamp
# Total Points: [count]

X,Y,Z,Landcell,Timestamp
123.456789,234.567890,45.123456,0x12345678,2024-01-01 12:00:00.000
...
```

## Testing in-Game

1. **Load the Plugin**: Follow the loading instructions above
2. **Verify UI**: Check that the plugin window appears with all buttons visible
3. **Test Recording**:
   - Click "Start Recording" or press F9
   - Move your character around
   - Watch the "Points Recorded" counter increase
   - Click "Stop Recording" or press F9 again
4. **Test Saving**:
   - Click "Save Route"
   - Check the chat for confirmation message
   - Verify the file was created in `My Documents\Asheron's Call\NavRoutes\`
5. **Test Commands**:
   - Type `/navtoggle` in chat to toggle recording
   - Type `/navsave` to save without using the UI
6. **Test Hotkey**:
   - Press F9 to toggle recording
   - Verify it works even when the game window has focus

## Troubleshooting

### Plugin doesn't appear in Decal Agent
- Verify the DLL is in the correct Plugins directory
- Check that all dependencies (Decal.Adapter.dll, VirindiViewService.dll) are available
- Check the Decal error log for loading errors

### UI doesn't show up in-game
- Ensure Virindi View Service (VVS) is installed and running
- Check that the plugin loaded successfully (check Decal Agent)
- Try typing `/plugin list` in-game to see loaded plugins

### F9 hotkey doesn't work
- Ensure the game window has focus when pressing F9
- Try using the command `/navtoggle` instead
- Check that no other plugin is intercepting F9

### Recording doesn't work
- Ensure you're logged into the game (not just at character select)
- Check the chat for error messages
- Verify the plugin status shows "Recording..." when started

### Files aren't being saved
- Check that you have write permissions to `My Documents\Asheron's Call\NavRoutes\`
- Ensure you have recorded points before trying to save
- Check the chat for error messages

## Technical Details

- **Recording Interval**: 100ms (10 times per second)
- **Thread Safety**: All recording operations are thread-safe using locks
- **Background Thread**: Recording runs in a background thread to prevent client blocking
- **Memory Management**: Points are stored in memory until saved or cleared

## License

This plugin is provided as-is. Feel free to modify and use as needed.

## Credits

Based on the SamplePlugin-VVS template from the Decal plugin repository.
