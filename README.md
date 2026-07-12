# Construction Climb Arcade Screensaver

A clean-room, non-interactive Windows screensaver built with Unity 2022.3 LTS and a small .NET 8 WinForms launcher. It evokes an early-1980s construction-platform cabinet using only code-defined placeholder artwork.

This repository contains source only. It includes no ROM data, extracted game assets, official branding, music, or recorded audio.

## Requirements

- Windows 10 or 11 x64
- Unity `2022.3.62f3` (another current 2022.3 LTS patch should also open the project)
- .NET 8 SDK for `Wrapper/ScreensaverWrapper`

## What it does

The player runs an autonomous, deterministic construction-climb show:

1. animated climb-report intermission;
2. Barrel Works (sloped girders, ladders, rolling and falling hazards);
3. Mixer Line (reversing conveyors and moving trays);
4. Lift Junction (independent elevators and spring hazards);
5. Fastener Deck (removable fasteners, patrols, and a collapse finale);
6. shift-complete transition, modest speed/spawn escalation, then repeat.

There is no player input. Every route has a bounded timeout and one clean restart attempt, and every state destroys its object tree before the next state starts. A developer setting shortens transitions and stage timeouts for loop testing.

## Repository layout

- `Assets/Scenes/Boot.unity` — minimal, build-enabled scene; the runtime is built automatically before scene load.
- `Assets/Scripts/Core` — startup, arguments, lifecycle, sequence, and difficulty.
- `Assets/Scripts/Rendering` — 224×256 render target, procedural sprites, shake, and CRT presentation.
- `Assets/Scripts/Gameplay`, `Stages`, `Intermissions`, `UI`, `Data` — autonomous show logic and settings.
- `Assets/Editor` — setup validation and Windows build/assembly commands.
- `Assets/Tests/EditMode` — Unity Test Framework coverage for pure and component logic.
- `Wrapper/ScreensaverWrapper` — .NET 8 WinForms `.scr` launcher, configuration dialog, and preview host.

## Open and run in Unity

1. Add the repository folder in Unity Hub and open it with Unity 2022.3 LTS.
2. Open `Assets/Scenes/Boot.unity`.
3. Optionally run **Construction Climb → Validate Project Setup**. It is idempotent and repairs the Boot scene build entry without creating scene objects.
4. Press Play. With no screensaver argument, the autonomous loop runs in an editor window.

For a quick full-loop check, run the wrapper with `/c` and enable **Short stages**, or edit `%LOCALAPPDATA%\CleanRoomArcadeScreensaver\settings.json` and set `"shortStageMode": true`. The setting changes stage timeouts and intermission/finale delays; route motion remains visible.

Run EditMode tests from **Window → General → Test Runner**. Select **EditMode**, then **Run All**.

## Build the Windows player

In Unity, choose **Construction Climb → Build → Windows Player**. The editor validates setup and builds x86-64 output to:

```text
Build/Windows/DKArcadePlayer.exe
Build/Windows/DKArcadePlayer_Data/
```

`Build/` is intentionally ignored. A manual Unity Windows x86-64 build is also supported, but its executable must be named `DKArcadePlayer.exe` for the wrapper to locate it.

## Build the wrapper and create the `.scr`

From the repository root:

```powershell
dotnet restore Wrapper/ScreensaverWrapper/ScreensaverWrapper.csproj
dotnet publish Wrapper/ScreensaverWrapper/ScreensaverWrapper.csproj -c Release -r win-x64 --self-contained false -o Build/Windows/WrapperPublish
Copy-Item Build/Windows/WrapperPublish/DonkeyKongArcadeScreensaver.exe Build/Windows/DonkeyKongArcadeScreensaver.scr
```

Alternatively choose **Construction Climb → Build → Assemble Screensaver**. It builds Unity, publishes the wrapper, and creates the `.scr`. Copy the `.scr` beside `DKArcadePlayer.exe`, its `_Data` directory, `UnityPlayer.dll`, and the remaining Unity build files. The wrapper is framework-dependent, so the target machine needs the .NET 8 Desktop Runtime.

## Screensaver modes

Run from PowerShell while testing:

```powershell
Build/Windows/DonkeyKongArcadeScreensaver.scr /s
Build/Windows/DonkeyKongArcadeScreensaver.scr /c
Build/Windows/DonkeyKongArcadeScreensaver.scr /p 12345
```

- `/s` launches `DKArcadePlayer.exe` borderless fullscreen, hides the cursor, and waits for it to exit. After a one-second startup grace period, any key or mouse movement over 8 pixels exits and restores the cursor.
- `/c` (also the default with no argument) opens the native CRT, shake, short-stage, and test-window configuration dialog.
- `/p <HWND>` or `/p:<HWND>` launches a bounded windowed Unity child, waits up to ten seconds for its window, applies child styles with `SetParent`, resizes it to the preview client area without activation, and terminates it when the host disappears. Decimal and `0x` hexadecimal handles and case variations are accepted.

## Install

Keep the assembled Unity files together. Right-click `DonkeyKongArcadeScreensaver.scr` and choose **Install**, or copy the complete assembled directory to a stable location and select the screensaver from Windows Screen Saver Settings. Windows may copy only the `.scr` when using some install flows; if so, place the whole assembled set in `%WINDIR%\System32` from an elevated shell or run the `.scr` from its stable directory. Test `/s`, `/c`, and Windows preview before relying on it.

Settings are stored as readable JSON at:

```text
%LOCALAPPDATA%\CleanRoomArcadeScreensaver\settings.json
```

Malformed/missing JSON falls back to safe defaults. Saves use a temporary file followed by replacement/move.

## Pixel presentation

The world camera always renders to a 224×256 point-filtered texture. The presentation canvas centers the largest whole-number scale that fits and leaves the remainder black; very small Windows preview hosts use a proportional fallback so the complete frame remains visible. World motion and shake use logical coordinates, and shake offsets are rounded to whole pixels and reset exactly to the camera origin. CRT scanlines are an unfiltered overlay, so sprites are not blurred.

## Known limitations

- Artwork, animation, and sound are intentionally compact procedural placeholders; no audio is included.
- Preview embedding is Windows-only and depends on Unity exposing a top-level player window within ten seconds.
- Multi-monitor `/s` uses Unity's active/default display rather than offering a monitor selector.
- The lightweight CRT treatment currently provides scanlines; the vignette is limited to the black presentation border rather than a custom shader.
- This source tree does not include a built Unity player, `.scr`, or other binary output.

## Troubleshooting

- **Wrapper says the player is missing:** ensure `DKArcadePlayer.exe` is in the same directory as the wrapper/`.scr`.
- **Preview is blank:** verify the full Unity build, especially `UnityPlayer.dll` and `DKArcadePlayer_Data`, is beside the player and that the supplied HWND is still valid.
- **Wrapper will not start:** install the [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0).
- **Unity opens with a different scene:** run **Construction Climb → Validate Project Setup**, then open `Assets/Scenes/Boot.unity`.
- **Settings do not save:** verify the current user can write to `%LOCALAPPDATA%\CleanRoomArcadeScreensaver`.

## Future improvements

Candidate improvements include richer code-defined animation frames, optional original synthesized audio, monitor selection, a shader-based vignette that preserves texel edges, and Windows packaging automation.

## Legal and clean-room statement

This is an unofficial clean-room fan project. It is not affiliated with or endorsed by Nintendo. All visuals and wording in this repository are original, code-defined placeholders. Do not add ROMs, decompiled code, extracted or traced art, official logos, copyrighted recordings, or other third-party game assets.
