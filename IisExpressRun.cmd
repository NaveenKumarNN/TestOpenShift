@rem https://stackoverflow.com/questions/5300755/how-to-start-iis-express-manually
@ECHO on
SET CurrentDir="%CD%"
SET IISRoot="%ProgramFiles(x86)%\IIS Express\"
CD /D %IISRoot%
START http://localhost:5000/
@rem IISExpress /trace:error
IISExpress /trace:error /path:%CurrentDir%\IdentityServerHost\ /port:5000 
pause

pause
old
START http://localhost:5000/
IISExpress /path:%CurrentDir% /port:5000 /trace:error
pause