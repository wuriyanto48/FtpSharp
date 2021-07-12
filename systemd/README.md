## Run FtpSharp as a daemon on Linux with `systemd`

- Clone this project

    ```shell
    $ git clone https://github.com/wuriyanto48/FtpSharp.git
    ```

- Build
    ```shell
    $ make build-linux
    ```

- Create `FtpSharp.service`

    ```shell
    $ sudo vi /etc/systemd/system/FtpSharp.service
    ```

- Copy `FtpSharp.service` file from this folder

- Reload daemon

    ```shell
    $ sudo systemctl daemon-reload
    ```

- Start `FtpSharp` service

    ```shell
    $ sudo systemctl start FtpSharp.service
    ```

- Check `FtpSharp` status

    ```shell
    $ sudo systemctl status FtpSharp.service
    ```