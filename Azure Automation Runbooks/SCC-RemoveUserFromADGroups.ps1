Param(
[parameter(Mandatory=$true)]
    [string] $userName

)

$cred = Get-AutomationPSCredential -Name "PowerAutomateGeneric"

$result = [PSCustomObject]@{
    outcome = $null
}

Try{ 

Get-ADUser -Identity $userName -Properties MemberOf | ForEach-Object {
    $_.MemberOf | Remove-ADGroupMember -Credential $cred  -Members $_.DistinguishedName -Confirm:$false
}

$result.outcome = "Success"
}

catch {
    # If an exception occurs, set the outcome property to "Error"
    $result.outcome = "Error: $($_.Exception.Message)"
}

# Convert the custom object to JSON
$jsonResult = $result | ConvertTo-Json

# Output the JSON result
Write-Output $jsonResult


