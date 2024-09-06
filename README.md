### Read me ###

Web Config keys:
    <add key="GAFilePath" value="~/GAFiles/" />
    <add key="MinnerRunTimeHour" value="00" />
    <add key="MinnerRunTimeMinute" value="00" />
    <add key="MinnerRunTimeZone" value="India Standard Time" />
    <add key="SharpspringBaseUrl" value="https://api.sharpspring.com/pubapi/v1/" />
    <add key="MinnerRunMultipleTimes" value="5:50,13:00,13:30" /> <!--Time should be interval of 24 (0-23) and must be separated by ","-->
    <add key="EZShredMinerRunInterval" value="45"/>
    <add key="EZShredMinerStopAfter" value="180"/>
    
    <add key="SharpspringRunMiner" value="True" />
    <add key="BullseyeRunMiner" value="True" />
    <add key="GoogleAnalyticsRunMiner" value="True" />
    <add key="EZShredRunMiner" value="True" />
    <add key="ShawKPISharpspringRunMiner" value="True"/>



Google OAuth keys in web config (sourcefuse domain):
ClientId 481417419299-artns3e3rchbotkqvi9d55r64k596dv3.apps.googleusercontent.com
ClientSecret LemYzTUjSQHXmdC9gQuoLkDL

   <add key="ClientId" value="481417419299-artns3e3rchbotkqvi9d55r64k596dv3.apps.googleusercontent.com" />
   <add key="ClientSecret" value="LemYzTUjSQHXmdC9gQuoLkDL" />

Dev AWS KEY
    <add key="AmazoneS3AccessKey" value="" />
    <add key="AmazoneS3SecretAccessKey" value="tN62uRfg/iJn042M0aTnWyDN5zWThdzHuSp7CNgM" />
    <add key="AmazoneS3BucketName" value="mxtrlogobucket-test" />
    <add key="AmazoneS3ServiceURL" value="http://s3-external-1.amazonaws.com" />
Production AWS KEY
    <add key="AmazoneS3AccessKey" value="" />
    <add key="AmazoneS3SecretAccessKey" value="ljRRYlZcf1Fg78IWuwssOwCCMT9O46uokgfPftw1" />
    <add key="AmazoneS3BucketName" value="mxtrlogobucket-producation" />
    <add key="AmazoneS3ServiceURL" value="http://s3-external-1.amazonaws.com" />
Stagging AWS KEY
    <add key="AmazoneS3AccessKey" value="" />
    <add key="AmazoneS3SecretAccessKey" value="ljRRYlZcf1Fg78IWuwssOwCCMT9O46uokgfPftw1" />
    <add key="AmazoneS3BucketName" value="mxtrlogobucket-stagging" />
    <add key="AmazoneS3ServiceURL" value="http://s3-external-1.amazonaws.com" />


GA Keys for  http://platform.mxtrautomation.com   
Client Id  481417419299-hb86l452cv02n835beoac4iaac6sq9pp.apps.googleusercontent.com
Client Secret  2mAABo93GxyfIoI9EJe3ba4_


Google OAuth keys in web config (https://mxtrstaging.sourcefuse.com)
ClientId 481417419299-eqqt8ffkqbadc79p9fr29loamn2t7s0m.apps.googleusercontent.com
ClientSecret fIyeNv8Cs7husZBz8qDLrMkH

<add key="ClientId" value="481417419299-eqqt8ffkqbadc79p9fr29loamn2t7s0m.apps.googleusercontent.com" />
   <add key="ClientSecret" value="fIyeNv8Cs7husZBz8qDLrMkH" />


<add key="FileVersion" value="1.1.23102017.1" /><!--1.1.PresentDate.NumberOfBuild-->
// Url Rewrite from http to https

<rewrite>
      <rules>
        <rule name="Force Https" stopProcessing="true">
         <match url="healthcheck.html" negate="true" />
         <conditions>
            <add input="{HTTP_X_FORWARDED_PROTO}" pattern="https" negate="true" />
         </conditions>
         <action type="Redirect" url="https://{HTTP_HOST}{REQUEST_URI}" redirectType="Permanent" />
      </rule>
      </rules>
    </rewrite>

App config keys for EZShred
<add key="EZShredGetCustomerDataAfter" value="45" /> <!-- In minutes-->
<add key="EZShredGetBuildingDataAfter" value="45" /> <!-- In minutes-->
<add key="EZShredGetServiceDataAfter" value="480" /> <!-- In minutes-->
<add key="EZShredGetMiscDataAfter" value="480" /><!-- In minutes-->
 <add key="IsCreateUpdateOnEZShred" value="false"/>
<add key="ActivateJIRABugRepoter" value="false"/>

