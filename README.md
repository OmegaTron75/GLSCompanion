# GLS Companion

**GLS Companion** is a Windows and Linux desktop companion application developed for the Great Lakes Syndicate Star Citizen community.

> **Early Public Beta**
>
> GLS Companion is still in active early development. We are releasing it publicly so Star Citizen players can use the tools, test them with different ships and gameplay situations, and help us improve them.
>
> **Feedback, bug reports, ship-specific cargo observations, and feature suggestions are very welcome.** The Cargo Grid and Cargo Cubing systems in particular will continue to be refined as they are tested across more ships and cargo configurations.

GLS Companion provides quick access to GLS community tools while playing Star Citizen, including:

- GLS Cargo Hauling Optimizer
- GLS Salvage Optimizer
- New Cargo Grid and Cargo Cubing system
- Ship-specific physical cargo grid layouts
- Destination-aware cargo placement and loading plans
- Load Sequence and Unload Mode
- Cargo container sizing and placement assistance
- Full, Compact, Run View, and Park modes
- Always-on-top companion window
- Cargo and Salvage sessions maintained independently while the application is running
- F9 global show/hide hotkey
- Saved Companion settings between sessions
- Live GLS web tools, allowing Cargo and Salvage improvements to appear without requiring a new Companion release
- Windows and Linux support

## Download GLS Companion

### Current Release: v0.1.0 Beta

#### Windows

[**Download GLS Companion v0.1.0 Beta for Windows**](https://github.com/OmegaTron75/GLSCompanion/releases/download/v0.1.0-beta/GLSCompanion-Setup-0.1.0.exe)

Windows 10/11 · 64-bit

#### Linux

Linux packages are available from the GLS Companion release page:

[**GLS Companion Releases**](https://github.com/OmegaTron75/GLSCompanion/releases)

Available Linux packages:

- `GLSCompanion_0.1.0_amd64.deb` — recommended installer for Ubuntu/Debian-based systems
- `GLSCompanion-Linux-x64.zip` — portable/manual Linux package

The Linux build has been tested on **Ubuntu 24.04 LTS x86-64 using Xorg/X11**.

**Important:** The global F9 show/hide hotkey currently requires an **X11/Xorg session on Linux**. The application can run under Wayland, but the global F9 keyboard hook is not currently supported there.

To check your current Linux desktop session:

```bash
echo $XDG_SESSION_TYPE
