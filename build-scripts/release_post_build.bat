rmdir /S /Q "..\Hearthstone Deck Tracker"
mkdir "..\Hearthstone Deck Tracker"
mkdir "..\Hearthstone Deck Tracker/Images"
xcopy /E /Y /Q "Images\*.*" "..\Hearthstone Deck Tracker\Images"
for /D %%a in (*-*) do (
	mkdir "..\Hearthstone Deck Tracker\%%a"
	xcopy "%%a\*.*" "..\Hearthstone Deck Tracker\%%a"
)
xcopy /Y "HearthstoneDeckTracker.exe" "..\Hearthstone Deck Tracker"
xcopy /Y "HearthstoneDeckTracker.exe.config" "..\Hearthstone Deck Tracker"
ren "..\Hearthstone Deck Tracker\HearthstoneDeckTracker.exe" "Hearthstone Deck Tracker.exe"
ren "..\Hearthstone Deck Tracker\HearthstoneDeckTracker.exe.config" "Hearthstone Deck Tracker.exe.config"
xcopy /Y "*.dll" "..\Hearthstone Deck Tracker"
xcopy /Y "..\..\..\..\HDTUpdate\bin\x86\Release\HDTUpdate.exe" "..\Hearthstone Deck Tracker"
xcopy /Y "..\..\..\..\HDTUninstaller\bin\x86\Release\HDTUninstaller.exe" "..\Hearthstone Deck Tracker"