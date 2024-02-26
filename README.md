This application is intended to be used as a mirror to log traffic sent to another application.

It emits the following header values from the original request:
- Host
- X-Original-URI

It can be configured to warn when the original URI matches a pattern.  This is controlled by `requestWarningPattern` in the `appsettings.json` file. An empty pattern (i.e. "") will warn on all values.

The log output will be similar to the following:

```json
{
  "Message": "",
  "MessageTemplate": "",
  "EventId": {
    "Id": 91876066,
    "Name": "RequestWarn"
  },
  "SourceContext": "Program",
  "RequestId": "0HN1MSI6G38K1:00000001",
  "RequestPath": "/",
  "ConnectionId": "0HN1MSI6G38K1",
  "XOriginalURI": "/login",
  "Host": "localhost:5122",
  "level": "warning"
}
```

To run the app locally, 
- update the `proxy_pass` value for `location /` to a value that makes sense to you
  - this is the application for which traffic is being mirrored
- run the `start-nginx.sh` script
- run this app
- open a browser to http://localhost:8080
  - 8080 is the port set in the nginx.conf file
- examine the log output
- run the `stop-nginx.sh` script when done