$sourcePath = Join-Path $PSScriptRoot 'Data\TeleporterDefine.txt'
$clientPath = Join-Path $PSScriptRoot '..\Client\Data\TeleporterDefine.txt'

if (!(Test-Path -LiteralPath $sourcePath) -or !(Test-Path -LiteralPath $clientPath)) {
    exit 0
}

$generated = Get-Content -Raw -LiteralPath $sourcePath | ConvertFrom-Json
$existing = Get-Content -Raw -LiteralPath $clientPath | ConvertFrom-Json

foreach ($entry in $generated.PSObject.Properties) {
    $oldEntry = $existing.PSObject.Properties[$entry.Name]
    if ($null -ne $oldEntry -and $null -ne $oldEntry.Value.Position) {
        $entry.Value | Add-Member -MemberType NoteProperty -Name Position -Value $oldEntry.Value.Position -Force
        if ($null -ne $oldEntry.Value.Direction) {
            $entry.Value | Add-Member -MemberType NoteProperty -Name Direction -Value $oldEntry.Value.Direction -Force
        }
    }
}

$generated | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $sourcePath -Encoding UTF8
