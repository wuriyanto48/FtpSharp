# FTP Sharp (FTP Server)

## An experimental FTP Server written `from scratch` in `C#`. 


> status: on going


Based on Spec https://datatracker.ietf.org/doc/html/rfc959

### Requirements

- Dotnet `V5.0` https://dotnet.microsoft.com/download


### Build

#### Non self contained build
```shell
$ make build
```

#### Self contained build

Linux
```
$ make build-linux
```

MacOS
```
$ make build-osx
```

### Running

> Make sure to add `config.json` param

#### Non self contained `run`
```shell
$ dotnet build/FtpSharp.Server.dll config.json
```

#### Self contained `run`
```shell
$ ./build/FtpSharp.Server config.json
```

### Test with FTP Client

```shell
$ ftp localhost 8777
```

#

#### Wuriyanto 2021