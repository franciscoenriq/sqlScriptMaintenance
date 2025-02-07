$serverName = $env:COMPUTERNAME
$Counters = @(
    ("\\$serverName" + "\Process(sqlservr*)\% User Time"), ("\\$serverName" + "\Process(sqlservr*)\% Privileged Time")
)
Get-Counter -Counter $Counters -MaxSamples 30 | ForEach {
    $_.CounterSamples | ForEach {
        [pscustomobject]@{
            TimeStamp = $_.TimeStamp
            Path = $_.Path
            Value = ([Math]::Round($_.CookedValue, 3))
        }
        Start-Sleep -s 2
    }
}