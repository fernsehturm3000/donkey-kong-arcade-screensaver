# Construction Climb Arcade Screensaver

A clean-room, non-interactive Windows screensaver built with Unity 2022.3 LTS and a small .NET 8 WinForms launcher. It evokes an early-1980s construction-platform cabinet using only code-defined placeholder artwork.

This repository contains source only. It includes no ROM data, extracted game assets, official branding, music, or recorded audio.

## Requirements

- Windows 10 or 11 x64
- Unity `2022.3.62f3` (another current 2022.3 LTS patch should also open the project)
- .NET 8 SDK for `Wrapper/ScreensaverWrapper`

## Current foundation

- Valid Unity project metadata and a build-enabled `Assets/Scenes/Boot.unity`
- Runtime bootstrap that creates the logical renderer without scene assembly
- 224x256 point-filtered render target with centered integer scaling
- Whole-logical-pixel impulse shake and optional scanline overlay
- LocalAppData JSON settings model with safe defaults and atomic replacement
- Case-insensitive `/s`, `/c`, `/p <HWND>`, and `/p:<HWND>` argument parsing
- Buildable WinForms wrapper skeleton

Implementation, build, test, installation, and troubleshooting instructions will be expanded alongside the remaining stages and Windows integration.
