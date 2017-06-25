
echo Creating nuget packages

nuget pack Source\HelixToolkit
nuget pack Source\HelixToolkit.Wpf
nuget pack Source\HelixToolkit.Wpf.SharpDX

echo Copying packages into package folder
for %%f in (*.nupkg) do copy %%f ..\..\packages\

PAUSE