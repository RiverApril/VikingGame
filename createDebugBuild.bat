echo off

set tempVar="DebugBuild"
mkdir %tempVar%

xcopy /y VikingGame\bin\Debug\VikingGame.exe %tempVar%
xcopy /y VikingGame\bin\Debug\OpenTK.dll %tempVar%
xcopy /y VikingGame\textures.png %tempVar%