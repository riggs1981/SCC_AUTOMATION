param(
[parameter(Mandatory=$true)]
[string]$username,
[parameter(Mandatory=$true)]
[string]$groups)

# Get Stored Credentials
$cred = Get-AutomationPSCredential -Name "PowerAutomateGeneric"

# Specify the username (SAMAccountName) of the member
$member = $username

# Specify the comma-separated list of AD group names
$groupNames = $groups

# Split the group names into an array
$groupArray = $groupNames -split ","

# Add the member to each group
foreach ($group in $groupArray) {
    Add-ADGroupMember -Credential $cred -Identity $group -Members $member
}
