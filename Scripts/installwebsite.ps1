Import-Module WebAdministration
$iisAppPoolName = "mxtr-pool"
$iisAppPoolDotNetVersion = "v4.0"
$iisAppName = "mxtr-automation"
$directoryPath = "C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform"
$to = "C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform\GAFiles\"
$from = "C:\GAFiles\*"
$DIRE = "C:\EZShred"

#copy webconfig 
Copy-Item -Path C:\webconfig\Web.config -Destination C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform
if ($?)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Copied WebConfig To Project Directory"", ""icon_emoji"": "":slightly_smiling_face:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Error Copying WebConfig to Project Folder!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}

#copyshawkpi
Copy-Item -Path C:\ShawKPISharpSpring\mxtrAutomation.ShawKPI.Sharpspring.exe.config -Destination C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform\MinerExecutableFiles\ShawKPISharpSpring
if ($?)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Copied Shawkpi To Project Directory"", ""icon_emoji"": "":slightly_smiling_face:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Error shawkpi WebConfig to Project Folder!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}


#copy sharpspring
Copy-Item -Path C:\sharpspring\mxtrAutomation.Sharpspring.exe.config -Destination C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform\MinerExecutableFiles\SharpSpring

if ($?)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Copied SharpSpring Config File To Project Directory"", ""icon_emoji"": "":slightly_smiling_face:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Error Copying SharpSpring Config File to Project Folder!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}

#copy googleAnalytics
Copy-Item -Path C:\ga\GoogleAnalyticsApp.exe.config -Destination C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform\MinerExecutableFiles\GoogleAnalytics

if ($?)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Copied GoogleAnalytics Config File To Project Directory"", ""icon_emoji"": "":slightly_smiling_face:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Error Copying GoogleAnalytics Config File to Project Folder!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}

#copy Bullseye
Copy-Item -Path C:\bullseye\mxtrAutomation.Bullseye.exe.config -Destination C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform\MinerExecutableFiles\BullEye

if ($?)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Copied Bullseye Config File To Project Directory"", ""icon_emoji"": "":slightly_smiling_face:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Error Copying Bullseye Config File to Project Folder!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing  | Out-Null

}

#copy EZShred
if ( Test-Path $DIRE ) {
 Copy-Item -Path C:\EZShred\mxtrAutomation.EZShred.exe.config -Destination C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform\MinerExecutableFiles\EZShred

if ($?)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Copied EZShred Config File To Project Directory"", ""icon_emoji"": "":slightly_smiling_face:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Error Copying EZShred Config File to Project Folder!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}
} 
else {
Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""No EZShred Folder Exists Here!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing | Out-Null

}


#copy GAFiles 
if( (Get-ChildItem C:\GAFiles\ | Measure-Object).Count -eq 0)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""*There are no Files in C:\GAFiles Folder! Nothing Copied To GAFiles Project Folder*"", ""icon_emoji"": "":confused:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing  | Out-Null

}
else
{

Copy-Item $from $to -recurse

if ($?)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""GAFiles Copied to Project Folder!"", ""icon_emoji"": "":slightly_smiling_face:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing  | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Error Copying GAFiles to Project Folder!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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" -UseBasicParsing  | Out-Null

}

}

#setting permissions 
icacls "C:\inetpub\wwwroot\" /q /c /t /grant IIS_IUSRS:f

#stop the default web site so we can use port :80
Stop-WebSite 'Default Web Site'

#set the autostart property so we don't have the default site kick back on after a reboot
cd IIS:\Sites\
Set-ItemProperty 'Default Web Site' serverAutoStart False

#navigate to the app pools root
cd IIS:\AppPools\

#check if the app pool exists
if (!(Test-Path $iisAppPoolName -pathType container))
{
    #create the app pool
    $appPool = New-Item $iisAppPoolName
    $appPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value $iisAppPoolDotNetVersion
}

#navigate to the sites root
cd IIS:\Sites\

#check if the site exists
if (Test-Path $iisAppName -pathType container)
{
    return
}

#create the site
$iisApp = New-Item $iisAppName -bindings @{protocol="http";bindingInformation=":80:"} -physicalPath $directoryPath
$iisApp | Set-ItemProperty -Name "applicationPool" -Value $iisAppPoolName
Set-ItemProperty $iisAppName serverAutoStart True
&"$env:windir\system32\inetsrv\appcmd" set APPPOOL $iisAppPoolName /processModel.idleTimeout:0.00:00:00
