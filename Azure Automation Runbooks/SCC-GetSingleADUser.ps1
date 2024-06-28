# Param for Power Automate Connector
param($username = "MR96805")
# Connect to Active Directory
Import-Module ActiveDirectory

# Retrieve user details based on SamAccountName
try {
    $User = Get-ADUser -Filter {SamAccountName -eq $username} -Properties *
    if ($User) {
        $UserDetails = @{
            Name = $User.Name
            UserPrincipalName = $User.UserPrincipalName
            DistinguishedName = $User.DistinguishedName
            Email = $User.EmailAddress
            # Add more properties as needed
        }
        $UserDetails | ConvertTo-Json
    } else {
        Write-Host "User with SamAccountName '$username' not found."
    }
} catch {
    Write-Host "Error retrieving user details: $_"
}
