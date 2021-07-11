.PHONY : build build-osx build-linux clean

clean:
	rm -rf build

build-osx:
	dotnet publish FtpSharp.Server/FtpSharp.Server.csproj -c Release -r osx-x64 --self-contained true -o ./build/

build-linux:
	dotnet publish FtpSharp.Server/FtpSharp.Server.csproj -c Release -r linux-x64 --self-contained true -o ./build/

build:
	dotnet publish FtpSharp.Server/FtpSharp.Server.csproj -c Release -o ./build/ --no-restore