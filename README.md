# FTP Sharp (FTP Server)

## an experimental FTP Server written `from scratch` in `C#`. 


> status: on going


Based on Spec https://datatracker.ietf.org/doc/html/rfc959

### Requirements

- Dotnet `V5.0` https://dotnet.microsoft.com/download


### Build

```shell
$ make build
```

### Running

Make sure to add `config.json` param

```shell
$ dotnet app/FtpSharp.Server.dll config.json
```

### Test with FTP Client

```shell
$ ftp localhost 8777
```

#

#### Wuriyanto 2021