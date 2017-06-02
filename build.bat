@echo off
cd ComicBoxApi\ComicBoxApi
dotnet restore
dotnet publish --configuration Release --output .\..\..\Build\
cd ..\..
CALL npm install -g @angular/cli
cd ComicBoxDisplay\comic-box-display
CALL npm install
CALL npm run ng build --prod
md .\..\..\Build\wwwroot\
move /Y dist\* .\..\..\Build\wwwroot\
cd ..\..
cd Build
dotnet ComicBoxApi.dll --urls "http://*:8080"
pause