<# Example: List all Entries of a Keepass Database #>
<# 
    Prerequisites: 
        The Keepass Database is located in D:\temp
        The Keepass Database is named Test.kdbx
        The Keepass Database is secured with a Master Password 123 and is tied to the Current Windows User Account
        The Keepass Binaries are Located in a Folder D:\PortableApps\KeePass\
#>

<# First we need to convert the plain text to a secure string #>
$masterPassword = ConvertTo-SecureString -AsPlainText -Force -String "123"
Find-KeepassEntry -KeepassLocation D:\PortableApps\KeePass\ -InputObject D:\temp\Test.kdbx -MasterPassword $masterPassword -WindowsUserAccount -AsUnprotectedStrings -Title "*#2" -TitleFilterMode Wildcard

#< Alternative by using Regular Expression #>
Find-KeepassEntry -KeepassLocation D:\PortableApps\KeePass\ -InputObject D:\temp\Test.kdbx -MasterPassword $masterPassword -WindowsUserAccount -AsUnprotectedStrings -Title "Entry.*\d" -TitleFilterMode RegularExpression

<# In either case the example data might look like this.

Id                           : 4C4FF16451B63845BA66E7DAB83C0A22
GroupName                    : Test
Path                         : Test
Title                        : Sample Entry #2
UserName                     : Michael321
Password                     : 12345
EstimatedPasswordQualityBits : 6
Expires                      : False
URL                          : https://keepass.info/help/kb/testform.html

#>