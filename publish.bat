@echo off
echo Starting to publish single-file executables for all platforms...
echo.

:: Set publish directory
set PUBLISH_DIR="bin\Release\publish"

:: Clean old publish files
if exist %PUBLISH_DIR% rmdir /s /q %PUBLISH_DIR%
mkdir %PUBLISH_DIR%

echo Publishing Windows x64 version...
dotnet publish CryptoWidget.csproj --configuration Release --output "%PUBLISH_DIR%\CryptoWidget_win-x64" --self-contained true --runtime win-x64 --property:PublishSingleFile=true

echo Publishing Windows x86 version...
dotnet publish CryptoWidget.csproj --configuration Release --output "%PUBLISH_DIR%\CryptoWidget_win-x86" --self-contained true --runtime win-x86 --property:PublishSingleFile=true

echo Publishing Windows ARM64 version...
dotnet publish CryptoWidget.csproj --configuration Release --output "%PUBLISH_DIR%\CryptoWidget_win-arm64" --self-contained true --runtime win-arm64 --property:PublishSingleFile=true

echo Publishing Linux x64 version...
dotnet publish CryptoWidget.csproj --configuration Release --output "%PUBLISH_DIR%\CryptoWidget_linux-x64" --self-contained true --runtime linux-x64 --property:PublishSingleFile=true

echo Publishing Linux ARM version...
dotnet publish CryptoWidget.csproj --configuration Release --output "%PUBLISH_DIR%\CryptoWidget_linux-arm" --self-contained true --runtime linux-arm --property:PublishSingleFile=true

echo Publishing Linux ARM64 version...
dotnet publish CryptoWidget.csproj --configuration Release --output "%PUBLISH_DIR%\CryptoWidget_linux-arm64" --self-contained true --runtime linux-arm64 --property:PublishSingleFile=true

echo Publishing macOS x64 version...
dotnet publish CryptoWidget.csproj --configuration Release --output "%PUBLISH_DIR%\CryptoWidget_osx-x64" --self-contained true --runtime osx-x64 --property:PublishSingleFile=true

echo Publishing macOS ARM64 version...
dotnet publish CryptoWidget.csproj --configuration Release --output "%PUBLISH_DIR%\CryptoWidget_osx-arm64" --self-contained true --runtime osx-arm64 --property:PublishSingleFile=true

echo.
echo Publishing completed! Files are located in %PUBLISH_DIR% directory
echo.
echo Executable files for each platform:
echo Windows x64: %PUBLISH_DIR%\CryptoWidget_win-x64\CryptoWidget.exe
echo Windows x86: %PUBLISH_DIR%\CryptoWidget_win-x86\CryptoWidget.exe
echo Windows ARM64: %PUBLISH_DIR%\CryptoWidget_win-arm64\CryptoWidget.exe
echo Linux x64: %PUBLISH_DIR%\CryptoWidget_linux-x64\CryptoWidget
echo Linux ARM: %PUBLISH_DIR%\CryptoWidget_linux-arm\CryptoWidget
echo Linux ARM64: %PUBLISH_DIR%\CryptoWidget_linux-arm64\CryptoWidget
echo macOS x64: %PUBLISH_DIR%\CryptoWidget_osx-x64\CryptoWidget
echo macOS ARM64: %PUBLISH_DIR%\CryptoWidget_osx-arm64\CryptoWidget
echo.
pause
