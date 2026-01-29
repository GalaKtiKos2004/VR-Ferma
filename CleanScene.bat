@echo off
echo Очистка missing prefabs из сцены...
echo.

cd /d "%~dp0"

powershell -Command "(Get-Content 'Assets\Scenes\SampleScene.unity') -replace '  - component: \{fileID: \d+, guid: (0fa95c7dd3a63864e9551c0cba2d7d89|7dde1fe2fecfa344b9a1bc69f04bd212|953cf898cb1199544a63c81883234819|beb59ec1097e45d40932b6210ce60bd9|c6bcf27b4e25e9b4ab0ae63fd3008335|ecb88a25571c55046b9cd059c64f82b8|f9183e3c33a442247a3ae78a3ee1571d|fb7605ccab93a4746b152d1227b50aa9), type: \d+\}', '' | Set-Content 'Assets\Scenes\SampleScene.unity'"

echo.
echo Готово! Missing prefabs удалены из scene файла.
echo Откройте Unity и проверьте.
pause
