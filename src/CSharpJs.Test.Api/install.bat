
@ECHO OFF
SET /P instancia=Informe a instancia:
@ECHO ON
"%~dp0CSharpJs.Test.Api.exe" install -instance:%instancia%
pause