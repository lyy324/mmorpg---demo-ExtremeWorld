json-excel\json-excel json Tables\ Data\
@powershell -NoProfile -ExecutionPolicy Bypass -File PreserveTeleporterCoordinates.ps1

@copy /Y Data\*.txt ..\Client\Data\
@copy /Y Data\*.txt ..\Server\GameServer\GameServer\bin\Debug\Data\
pause
