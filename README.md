# ICETASK1 – LU1-TaskManager – Change Documentation

## Table of Contents

- [Overview](#overview)
- [Build Configuration](#build-configuration)
- [Code Cleanup](#code-cleanup)
- [Bug Fixes](#bug-fixes)
- [Reusable PID Retrieval](#reusable-pid-retrieval)
- [btnThreads_Click Changes](#btnthreadsclick-changes)
- [btnLoadedModules_Click Changes](#btnloadedmodulesclick-changes)
- [Benefits of Changes](#benefits-of-changes)
- [Conclusion](#conclusion)

---

## Overview

This document summarizes the changes made to **LU1-TaskManager** for improved stability, compatibility, and maintainability.

**Goals of the changes:**

- Fix compatibility issues with 64-bit processes
- Improve error handling to prevent crashes
- Simplify and clean up code
- Reduce duplication with reusable methods

---

## Build Configuration

<details>
<summary>Click to expand</summary>

- Unticked **Prefer 32-bit** in Project Properties → Build
- Ensures correct execution in **64-bit environments**
- Fixes error:
  > _“A 32-bit process cannot access modules of a 64-bit process”_

</details>

---

## Code Cleanup

<details>
<summary>Click to expand</summary>

- Removed all **unnecessary `using` statements** from:
  - `Form1.cs`
  - `Program.cs`

</details>

---

## Bug Fixes

<details>
<summary>Click to expand</summary>

- **Line 41**: Corrected typo in `MessageBox` text → `porcessId` → `processId`
- **Line 43**: Broadened exception handling →
  - From: `catch (InvalidOperationException ex)`
  - To: `catch (Exception ex)`
- **Line 140**: Corrected `MessageBow` → `MessageBox`
  - Changed variable usage from `newId` → `i` to support new PID method

</details>

---

## Reusable PID Retrieval

<details>
<summary>Click to expand</summary>

A reusable method was added to replace duplicated PID parsing logic.

````csharp
private bool tryFindPID(out int pid)
{
    pid = 0;
    string id = listBox1.SelectedItem.ToString();
    string newId = id.Split('\t')[0].Replace("-> PID: ", "").Trim();
    return int.TryParse(newId, out pid);
}

    Added at line 199–205 in Form1.cs

    Now used in both btnThreads_Click and btnLoadedModules_Click

</details>
btnThreads_Click Changes
<details> <summary>Click to expand</summary>

    Old substring-based PID parsing removed

    Replaced with call to tryFindPID:

Before:

string id = listBox1.SelectedItem.ToString().Substring(8, 5);
string newId = new string(id.Where(c => char.IsDigit(c)).ToArray());
int i = Convert.ToInt32(newId);

After:

if (!tryFindPID(out int i))
{
    MessageBox.Show("Invalid PID.");
    return;
}

    Lines 107–119 wrapped in try/catch block

Before (no exception handling):

foreach (ProcessThread thread in process.Threads)
{
    listBox2.Items.Add($"Thread ID: {thread.Id}");
}

After (with exception handling):

try
{
    foreach (ProcessThread thread in process.Threads)
    {
        listBox2.Items.Add($"Thread ID: {thread.Id}");
    }
}
catch (Exception ex)
{
    MessageBox.Show($"Error accessing process threads: {ex.Message}");
}

</details>
btnLoadedModules_Click Changes
<details> <summary>Click to expand</summary>

    Old PID parsing removed → replaced with tryFindPID

    Typo fixed: MessageBow → MessageBox

    Updated to use i instead of newId

Before:

string id = listBox1.SelectedItem.ToString().Substring(8, 5);
string newId = new string(id.Where(c => char.IsDigit(c)).ToArray());
int i = Convert.ToInt32(newId);
MessageBow.Show("Loaded Modules for process: " + newId);

After:

if (!tryFindPID(out int i))
{
    MessageBox.Show("Invalid PID.");
    return;
}
MessageBox.Show("Loaded Modules for process: " + i);

    Lines 154–165 wrapped in try/catch block

Before (no exception handling):

foreach (ProcessModule module in process.Modules)
{
    listBox3.Items.Add(module.ModuleName);
}

After (with exception handling):

try
{
    foreach (ProcessModule module in process.Modules)
    {
        listBox3.Items.Add(module.ModuleName);
    }
}
catch (Exception ex)
{
    MessageBox.Show($"Error accessing modules: {ex.Message}");
}

</details>
Benefits of Changes
<details> <summary>Click to expand</summary>

    ✅ Fixed typos in UI messages

    ✅ Improved exception handling → prevents app crashes

    ✅ Added reusable PID method → reduced code duplication

    ✅ Cleaner, easier-to-maintain code

    ✅ Ensured compatibility with 64-bit systems

</details>
Conclusion
<details> <summary>Click to expand</summary>

The LU1-TaskManager project is now:

    More stable

    More maintainable

    More user-friendly

These changes improve both functionality and code readability.
</details> ```
````
