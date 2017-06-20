# NDashButton

This is a simple listener of Amazon Dash Button requests.  
It fires an event when a button in the same network pressed.  

## How to use

First, you should..

* configure button's wi-fi connection with your phone app.
* check the MAC address of the button.

Then, use like below.

```csharp

//Create a listener.
var listener = new DashButtonListener("192.168.1.2");

//Register dash buttons with MAC address.
listener.Register(new DashButton("button01", "00000000000A"));
listener.Register(new DashButton("button02", "00000000000B"));

//Set options.
listener.DuplicateIgnoreInterval = TimeSpan.FromSeconds(10);
listener.ReadTimeout = TimeSpan.FromSeconds(1);

//Add event handlers.
listener.Pushed += listener_Pushed;

//Start monitoring.
listener.Start();

...

//Stop.
listener.Stop();

```

## Thanks

The basic idea was inspired by [sammessina/Dash-Button](https://github.com/sammessina/Dash-Button)

