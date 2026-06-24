# WindowsDnsSwitcher Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a Windows WinForms DNS mode switcher for physical WLAN/Wi-Fi IPv4 DNS with self-contained x64 publishing.

**Architecture:** Put behavior in a testable core library, keep WinForms as the UI shell, and use a no-dependency console test runner. System commands are represented as explicit executable plus argument arrays and executed without shell command strings.

**Tech Stack:** C# WinForms, .NET 10, Windows Forms, `System.Net.NetworkInformation`, `ProcessStartInfo`, JSON config through `System.Text.Json`.

---

## File Structure

- Create `WindowsDnsSwitcher.slnx`: solution containing all projects.
- Create `src/WindowsDnsSwitcher.Core/WindowsDnsSwitcher.Core.csproj`: core library.
- Create `src/WindowsDnsSwitcher.Core/AdapterInfo.cs`: normalized adapter model.
- Create `src/WindowsDnsSwitcher.Core/AdapterClassifier.cs`: wireless and virtual adapter detection plus sorting.
- Create `src/WindowsDnsSwitcher.Core/DnsMode.cs`: DNS mode enum.
- Create `src/WindowsDnsSwitcher.Core/DnsModeParser.cs`: `netsh show dnsservers` parser.
- Create `src/WindowsDnsSwitcher.Core/CommandSpec.cs`: executable plus argument list model.
- Create `src/WindowsDnsSwitcher.Core/DnsCommandFactory.cs`: command construction for netsh/ipconfig.
- Create `src/WindowsDnsSwitcher.Core/ProcessCommandRunner.cs`: process execution wrapper.
- Create `src/WindowsDnsSwitcher.Core/AppConfig.cs`: config model and JSON persistence.
- Create `src/WindowsDnsSwitcher.Core/NetworkInfoFormatter.cs`: adapter details and IPv6 status helpers.
- Create `src/WindowsDnsSwitcher.WinForms/WindowsDnsSwitcher.WinForms.csproj`: WinForms app.
- Create `src/WindowsDnsSwitcher.WinForms/app.manifest`: `requireAdministrator` manifest.
- Create `src/WindowsDnsSwitcher.WinForms/Program.cs`: startup and elevation check.
- Create `src/WindowsDnsSwitcher.WinForms/MainForm.cs`: UI and event handlers.
- Create `tests/WindowsDnsSwitcher.Tests/WindowsDnsSwitcher.Tests.csproj`: no-dependency console test project.
- Create `tests/WindowsDnsSwitcher.Tests/Program.cs`: assertion-based test runner.
- Create `build.bat`: one-command publish to `dist`.
- Create `README.md`: concise usage, publishing, and FAQ.

## Tasks

### Task 1: Create Project Skeleton

**Files:**
- Create: `WindowsDnsSwitcher.slnx`
- Create: `src/WindowsDnsSwitcher.Core/WindowsDnsSwitcher.Core.csproj`
- Create: `src/WindowsDnsSwitcher.WinForms/WindowsDnsSwitcher.WinForms.csproj`
- Create: `tests/WindowsDnsSwitcher.Tests/WindowsDnsSwitcher.Tests.csproj`
- Create: `build.bat`

- [ ] **Step 1: Add SDK-style project files**

Use `net10.0` for core/tests and `net10.0-windows` with `<UseWindowsForms>true</UseWindowsForms>` for the app. Set app assembly name to `WindowsDnsSwitcher`.

- [ ] **Step 2: Add references**

WinForms references core. Tests reference core.

- [ ] **Step 3: Add build script**

`build.bat` runs:

```bat
dotnet publish src\WindowsDnsSwitcher.WinForms\WindowsDnsSwitcher.WinForms.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o dist
```

### Task 2: Test and Implement Adapter Classification

**Files:**
- Create: `src/WindowsDnsSwitcher.Core/AdapterInfo.cs`
- Create: `src/WindowsDnsSwitcher.Core/AdapterClassifier.cs`
- Modify: `tests/WindowsDnsSwitcher.Tests/Program.cs`

- [ ] **Step 1: Write failing tests**

Tests assert that real wireless adapters rank before virtual adapters, high-risk virtual adapter names are strongly flagged, and saved adapter selection is honored when present.

- [ ] **Step 2: Run tests and confirm failure**

Run:

```bat
dotnet run --project tests\WindowsDnsSwitcher.Tests\WindowsDnsSwitcher.Tests.csproj
```

Expected: compile failure or assertion failure because core classes do not exist yet.

- [ ] **Step 3: Implement minimal classifier**

Add adapter model, virtual marker detection, high-risk virtual adapter detection, likely wireless detection, display filtering, and selection sorting.

- [ ] **Step 4: Run tests and confirm pass**

Run the same test command. Expected: all current tests pass.

### Task 3: Test and Implement DNS Mode Parsing and Commands

**Files:**
- Create: `src/WindowsDnsSwitcher.Core/DnsMode.cs`
- Create: `src/WindowsDnsSwitcher.Core/DnsModeParser.cs`
- Create: `src/WindowsDnsSwitcher.Core/CommandSpec.cs`
- Create: `src/WindowsDnsSwitcher.Core/DnsCommandFactory.cs`
- Modify: `tests/WindowsDnsSwitcher.Tests/Program.cs`

- [ ] **Step 1: Write failing tests**

Tests cover automatic DNS output, target static DNS output, other static DNS output, command failure, and exact argument arrays for `netsh` and `ipconfig`.

- [ ] **Step 2: Run tests and confirm failure**

Run the console test project. Expected: failures because parser and command factory do not exist.

- [ ] **Step 3: Implement parser and command factory**

Parser accepts Chinese and English netsh output patterns where possible and extracts IPv4 addresses. Commands use separate argument entries and never shell strings.

- [ ] **Step 4: Run tests and confirm pass**

Run the console test project. Expected: all parser and command tests pass.

### Task 4: Test and Implement Config and Info Helpers

**Files:**
- Create: `src/WindowsDnsSwitcher.Core/AppConfig.cs`
- Create: `src/WindowsDnsSwitcher.Core/NetworkInfoFormatter.cs`
- Modify: `tests/WindowsDnsSwitcher.Tests/Program.cs`

- [ ] **Step 1: Write failing tests**

Tests cover config save/load, invalid JSON fallback, IPv6 status text, and adapter info formatting from an `AdapterInfo`.

- [ ] **Step 2: Run tests and confirm failure**

Run the console test project. Expected: failures because config and formatter do not exist.

- [ ] **Step 3: Implement helpers**

Use `System.Text.Json` for config. Keep invalid config non-fatal.

- [ ] **Step 4: Run tests and confirm pass**

Run the console test project. Expected: all tests pass.

### Task 5: Implement WinForms UI

**Files:**
- Create: `src/WindowsDnsSwitcher.WinForms/app.manifest`
- Create: `src/WindowsDnsSwitcher.WinForms/Program.cs`
- Create: `src/WindowsDnsSwitcher.WinForms/MainForm.cs`

- [ ] **Step 1: Add manifest**

Set requested execution level to `requireAdministrator`.

- [ ] **Step 2: Build UI**

Create labels, adapter combo, refresh button, show-all checkbox, two main mode buttons, helper buttons, info labels, and recent log list.

- [ ] **Step 3: Wire behavior**

Enumerate adapters, load/save config, refresh selected mode, warn on virtual adapter choices, run DNS commands, flush DNS, open helper links, and show adapter information dialog.

- [ ] **Step 4: Run tests**

Run the console test project. Expected: all tests pass.

### Task 6: README, Build, and Publish Verification

**Files:**
- Create: `README.md`

- [ ] **Step 1: Write README**

Include purpose, run, publish, copying `dist`, and common problems.

- [ ] **Step 2: Build solution**

Run:

```bat
dotnet build WindowsDnsSwitcher.slnx
```

Expected: build succeeds.

- [ ] **Step 3: Run tests**

Run:

```bat
dotnet run --project tests\WindowsDnsSwitcher.Tests\WindowsDnsSwitcher.Tests.csproj
```

Expected: all tests pass.

- [ ] **Step 4: Publish**

Run:

```bat
build.bat
```

Expected: `dist\WindowsDnsSwitcher.exe` exists with self-contained runtime files.



