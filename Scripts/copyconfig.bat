@echo off
 set src_folder = c:\config\Web.config
 set dst_folder = c:\inetpub\wwwroot\mxtr-automation
 xcopy /S/E/U %src_folder% %dst_folder%



