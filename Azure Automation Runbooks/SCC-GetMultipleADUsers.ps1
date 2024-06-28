param (
    [Parameter(Mandatory=$true)]
    [string]$InputUserIds
)

# Convert the comma-separated input to an array of samAccountNames
$UserIdsArray = $InputUserIds -split ','

# Initialize an empty array to store the results
$ResultsArray = @()

# Import the Active Directory module (make sure you have the module installed)
Import-Module ActiveDirectory

# Loop through each samAccountName and check if it exists in Active Directory
foreach ($UserId in $UserIdsArray) {
    $user = Get-ADUser -Filter {samAccountName -eq $UserId} -ErrorAction SilentlyContinue
    if ($user) {
        $ResultsArray += @{
            "UserId" = $UserId
            "Exists" = $true
        }
    } else {
        $ResultsArray += @{
            "UserId" = $UserId
            "Exists" = $false
        }
    }
}

# Convert the results to JSON
$OutputJson = $ResultsArray | ConvertTo-Json

# Always return an array, even if there's only one result
if ($ResultsArray.Count -eq 1) {
    Write-Output "[$OutputJson]"
} else {
    Write-Output $OutputJson
}
