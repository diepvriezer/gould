for /r %%x in (*.nuspec) do nuget pack "%%x" -o packages