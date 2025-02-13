$from = "C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform\GAFiles\*"
$to = "C:\GAFiles\"
if( (Get-ChildItem C:\inetpub\wwwroot\mxtr-automation\Websites\mxtrAutomation.Websites.Platform\GAFiles | Measure-Object).Count -eq 0)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""*There are no Files in GAFILES Project Folder! Nothing Copied To C:/GAFiles*"", ""icon_emoji"": "":confused:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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"  -UseBasicParsing | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Got Some Files in GAFiles in Project Folder!"", ""icon_emoji"": "":smile:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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"  -UseBasicParsing | Out-Null

Copy-Item $from $to -recurse

if ($?)
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Files Copied to C:/GAFiles!"", ""icon_emoji"": "":confused:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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"  -UseBasicParsing | Out-Null

}
else
{

Invoke-WebRequest `
-Body  "payload={""channel"": ""#mxtr-deployment"", ""username"": ""Deployment"", ""text"": ""Error Copying Files to C:/GAFiles!"", ""icon_emoji"": "":disappointed:""}" `
-Method Post `
-Uri "https://hooks.slack.com/services/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"  -UseBasicParsing | Out-Null

}

}
