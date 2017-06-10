@echo off
cd Build
dotnet ComicBoxApi.dll --urls "http://*:8080"
pause