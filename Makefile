.PHONY : build build-osx build-linux clean

clean:
	rm -rf app

build-osx:
	dotnet publish FtpSharp.Server/FtpSharp.Server.csproj -c Release -r osx-x64 --self-contained true -o ./app/

build-linux:
	dotnet publish FtpSharp.Server/FtpSharp.Server.csproj -c Release -r linux-x64 --self-contained true -o ./app/

build:
	dotnet publish FtpSharp.Server/FtpSharp.Server.csproj -c Release -o app --no-restore